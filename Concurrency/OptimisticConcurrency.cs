using MongoDB.Driver;

namespace MongodB.Poc.Concurrency
{
    public class OptimisticConcurrency
    {
        private const string MemberId = "649d39e57a838f1a21139aef";

        private IMongoCollection<Member> _member;

        private int count;

        public OptimisticConcurrency()
        {
            var client = new MongoClient("mongodb://localhost:2000");
            var database = client.GetDatabase("concurrency");
            _member = database.GetCollection<Member>("member");
        }

        public async Task Go()
        {
            var tasks = new List<Task>();
            for (int i = 0; i < 10; i++)
            {
                var task = Task.Run(async () =>
                {
                    var members = await _member.Find(x => x.Id == MemberId)
                        .ToListAsync();
                    var member = members.FirstOrDefault();
                    var balance = member.Point.Balance;

                    var handler = new AddPointRecordHandler();
                    await handler.Add(10, balance + 10);
                    count += 1;
                });
                tasks.Add(task);
            }

            await Task.WhenAll(tasks);
            Console.WriteLine($"Count: {count}.");
        }
    }
}