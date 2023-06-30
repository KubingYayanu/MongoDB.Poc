using MongoDB.Driver;

namespace MongodB.Poc.Concurrency
{
    public class AddPointRecordHandler
    {
        private const string MemberId = "649d39e57a838f1a21139aef";
        private readonly List<Func<Task<long>>> _commands = new();

        private IMongoCollection<Member> _member;
        private IMongoCollection<PointExpiration> _pointExpiration;
        private IMongoCollection<PointRecord> _pointRecord;

        public AddPointRecordHandler()
        {
            var client = new MongoClient("mongodb://localhost:2000");
            var database = client.GetDatabase("concurrency");
            _member = database.GetCollection<Member>("member");
            _pointRecord = database.GetCollection<PointRecord>("point_record");
            _pointExpiration = database.GetCollection<PointExpiration>("point_expiration");
        }

        public async Task Add(int gain, int balance)
        {
            AddPointRecord(gain, balance);
            UpdateMemberPoint(balance);
            await HandlePointExpiration(gain);
            var count = await Complete();
            Console.WriteLine($"Effect count: {count}.");
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
            _commands.Add(async () =>
            {
                await _pointRecord.InsertOneAsync(newPointRecord);
                return 1;
            });
        }

        private void UpdateMemberPoint(int balance)
        {
            var filter = Builders<Member>.Filter
                .Where(x => x.Id == MemberId);
            var update = Builders<Member>.Update
                .Set(x => x.Point, new MemberPoint { Balance = balance });
            _commands.Add(async () =>
            {
                var result = await _member.UpdateOneAsync(filter, update);
                return result.ModifiedCount;
            });
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
                UpdateExpirationBalance(gain);
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
            _commands.Add(async () =>
            {
                await _pointExpiration.InsertOneAsync(newPointExpiration);
                return 1;
            });
        }

        private void UpdateExpirationBalance(int gain)
        {
            var filter = Builders<PointExpiration>.Filter
                .Where(x => x.Member.MemberId == MemberId);
            var update = Builders<PointExpiration>.Update
                .Inc(x => x.Point.Balance, gain);
            _commands.Add(async () =>
            {
                var result = await _pointExpiration.UpdateOneAsync(filter, update);
                return result.ModifiedCount;
            });
        }

        private async Task<long> Complete()
        {
            long count = 0;
            foreach (var command in _commands)
            {
                var amount = await command();
                count += amount;
            }

            _commands.Clear();
            return count;
        }
    }
}