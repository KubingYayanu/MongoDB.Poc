using MongoDB.Driver;

namespace MongodB.Poc.Concurrency
{
    public class AddPointRecordHandler
    {
        private const string MemberId = "649d39e57a838f1a21139aef";
        private readonly List<Func<IClientSessionHandle, Task>> _commands = new();

        private MongoClient _client;
        private IMongoDatabase _database;
        private IMongoCollection<Member> _member;
        private IMongoCollection<PointExpiration> _pointExpiration;
        private IMongoCollection<PointRecord> _pointRecord;

        public AddPointRecordHandler()
        {
            _client = new MongoClient("mongodb://localhost:3003");
            _database = _client.GetDatabase("concurrency");
            _member = _database.GetCollection<Member>("member");
            _pointRecord = _database.GetCollection<PointRecord>("point_record");
            _pointExpiration = _database.GetCollection<PointExpiration>("point_expiration");
        }

        public async Task Add(int gain, int balance)
        {
            AddPointRecord(gain, balance);
            UpdateMemberPoint(balance);
            await HandlePointExpiration(gain);
            await Complete();
        }

        private void AddPointRecord(int gain, int balance)
        {
            var newPointRecord = new PointRecord
            {
                Member = new PointRecordMember { MemberId = MemberId },
                Point = new PointRecordPoint
                {
                    Gain = gain,
                    Balance = balance
                }
            };
            _commands.Add(s => _pointRecord.InsertOneAsync(s, newPointRecord));
        }

        private void UpdateMemberPoint(int balance)
        {
            var filter = Builders<Member>.Filter
                .Where(x => x.Id == MemberId);
            var update = Builders<Member>.Update
                .Set(x => x.Point, new MemberPoint { Balance = balance });
            _commands.Add(s => _member.UpdateOneAsync(s, filter, update));
        }

        private async Task HandlePointExpiration(int gain)
        {
            var pointExpiration = await GetPointExpiration();
            if (pointExpiration == null)
            {
                AddPointExpiration(gain);
            }
            else
            {
                pointExpiration.Point.Balance += gain;
                UpdateExpirationBalance(pointExpiration);
            }
        }

        private async Task<PointExpiration> GetPointExpiration()
        {
            var filter = Builders<PointExpiration>.Filter
                .Where(x => x.Member.MemberId == MemberId);
            var pointExpirations = await _pointExpiration.Find(filter)
                .ToListAsync();
            var pointExpiration = pointExpirations.FirstOrDefault();
            return pointExpiration;
        }

        private void AddPointExpiration(int gain)
        {
            var newPointExpiration = new PointExpiration
            {
                Member = new PointExpirationMember
                {
                    MemberId = MemberId
                },
                Point = new PointExpirationPoint
                {
                    Balance = gain
                }
            };
            _commands.Add(s => _pointExpiration.InsertOneAsync(s, newPointExpiration));
        }

        private void UpdateExpirationBalance(PointExpiration pointExpiration)
        {
            var filter = Builders<PointExpiration>.Filter
                .Where(x => x.Member.MemberId == MemberId);
            _commands.Add(s => _pointExpiration.ReplaceOneAsync(s, filter, pointExpiration));
        }

        private async Task Complete(MongoDBIsolationLevel level = MongoDBIsolationLevel.ReadCommitted)
        {
            var transactionOptions = GetTransactionOptions(level);
            using (var session = await _client.StartSessionAsync())
            {
                session.StartTransaction(transactionOptions);

                foreach (var command in _commands)
                {
                    await command(session);
                }

                await session.CommitTransactionAsync();
            }

            _commands.Clear();
        }

        private TransactionOptions GetTransactionOptions(MongoDBIsolationLevel level)
        {
            // 預設 ReadCommitted 
            var readConcern = ReadConcern.Majority;
            if (level == MongoDBIsolationLevel.RepeatableRead)
            {
                readConcern = ReadConcern.Snapshot;
            }

            return new TransactionOptions(
                readConcern: readConcern,
                readPreference: ReadPreference.Primary,
                writeConcern: WriteConcern.WMajority,
                maxCommitTime: TimeSpan.FromSeconds(30));
            ;
        }
    }
}