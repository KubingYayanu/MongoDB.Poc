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
            var client = new MongoClient("mongodb://localhost:3001");
            var database = client.GetDatabase("concurrency");
            _member = database.GetCollection<Member>("member");
        }

        public async Task Go()
        {
            var tasks = new List<Task>();
            for (int i = 0; i < 5; i++)
            {
                var task = Task.Run(async () =>
                {
                    try
                    {
                        var members = await _member.Find(x => x.Id == MemberId)
                            .ToListAsync();
                        var member = members.FirstOrDefault();
                        var balance = member.Point.Balance;

                        var handler = new AddPointRecordHandler();
                        await handler.Add(10, balance + 10);
                        count += 1;
                    }
                    catch (Exception ex)
                    {
                        // 當發生 WriteConflict，依照需求執行 retry 或是中斷執行
                        // Command update failed: WriteConflict error: this operation conflicted with another operation.
                        // Please retry your operation or multi-document transaction..
                        Console.WriteLine(ex.Message);
                    }
                });
                tasks.Add(task);
            }

            await Task.WhenAll(tasks);
            Console.WriteLine($"Count: {count}.");
        }
    }
}