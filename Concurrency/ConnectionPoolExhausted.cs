using MongoDB.Driver;

namespace MongodB.Poc.Concurrency
{
    public class ConnectionPoolExhausted
    {
        private const string MemberId = "649d39e57a838f1a21139aef";
        
        private int count;
        
        public async Task GetMember()
        {
            var tasks = new List<Task>();
            for (int i = 0; i < 1000; i++)
            {
                var task = Task.Run(async () =>
                {
                    var client = new MongoClient("mongodb://localhost:2000");
                    var database = client.GetDatabase("concurrency");
                    var member = database.GetCollection<Member>("member");
                    var members = await member.Find(x => x.Id == MemberId)
                        .ToListAsync();
                    count += 1;
                    Console.WriteLine($"Get Member count: {members.Count}.");
                });
                tasks.Add(task);
            }

            await Task.WhenAll(tasks);
            Console.WriteLine($"Count: {count}.");
        }
    }
}