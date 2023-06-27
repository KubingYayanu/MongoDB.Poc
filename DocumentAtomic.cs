using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace MongodB.Poc
{
    public class DocumentAtomic
    {
        private const string Id1 = "649031f3fa4567a2487ffb95";
        private const string Id2 = "649057d2a4a33c0393e658f0";
        private const string Id3 = "6490fd82d66481504b125ba6";
        private const string Id4 = "649103e8d66481504b125ba7";
        private const string Id5 = "649a946a677a2ecd7f9273bf";
        private IMongoCollection<Record> _collection;

        private int count1;
        private int count2;
        private int count3;
        private int count4;
        private int count5;

        public DocumentAtomic()
        {
            var client = new MongoClient("mongodb://localhost:2000");
            var database = client.GetDatabase("gift");
            _collection = database.GetCollection<Record>("records");
        }

        private async Task ResetPoint(string id)
        {
            var filter = Builders<Record>.Filter
                .Where(x => x.Id == id);
            var update = Builders<Record>.Update
                .Set(x => x.Point, 0);
            await _collection.UpdateOneAsync(filter, update);
        }

        /// <summary>
        ///     UpdateOneAsync;
        ///     Query and Modify: $set;
        ///     Lost update
        /// </summary>
        public async Task Version1()
        {
            await ResetPoint(Id1);

            var tasks = new List<Task>();
            for (int i = 0; i < 300; i++)
            {
                tasks.Add(Task.Run(async () =>
                {
                    var record = await _collection
                        .Find(x => x.Id == Id1)
                        .SingleAsync();
                    record.Point++;
                    var filter = Builders<Record>.Filter
                        .Where(x => x.Id == Id1);
                    var update = Builders<Record>.Update
                        .Set(x => x.Point, record.Point);
                    count1 = count1 + 1;
                    await _collection.UpdateOneAsync(filter, update);
                }));
            }

            await Task.WhenAll(tasks);

            var record = await _collection
                .Find(x => x.Id == Id1)
                .SingleAsync();
            Console.WriteLine($"Version 1. Count1: {count1}, Id: {Id1}, Point: {record.Point}.");
        }

        /// <summary>
        ///     UpdateOneAsync;
        ///     Atomic Operations: $inc
        /// </summary>
        public async Task Version2()
        {
            await ResetPoint(Id2);

            var tasks = new List<Task>();
            for (int i = 0; i < 300; i++)
            {
                tasks.Add(Task.Run(async () =>
                {
                    var filter = Builders<Record>.Filter
                        .Where(x => x.Id == Id2);
                    var update = Builders<Record>.Update
                        .Inc(x => x.Point, 1);
                    count2 = count2 + 1;
                    await _collection.UpdateOneAsync(filter, update);
                }));
            }

            await Task.WhenAll(tasks);

            var record = await _collection
                .Find(x => x.Id == Id2)
                .SingleAsync();
            Console.WriteLine($"Version 2. Count2: {count2}, Id: {Id2}, Point: {record.Point}.");
        }

        /// <summary>
        ///     FindOneAndUpdateAsync;
        ///     Atomic Operations: $inc
        /// </summary>
        public async Task Version3()
        {
            await ResetPoint(Id3);

            var tasks = new List<Task>();
            for (int i = 0; i < 300; i++)
            {
                tasks.Add(Task.Run(async () =>
                {
                    var filter = Builders<Record>.Filter
                        .Where(x => x.Id == Id3);
                    var update = Builders<Record>.Update
                        .Inc(x => x.Point, 1);
                    count3 = count3 + 1;
                    await _collection.FindOneAndUpdateAsync(filter, update);
                }));
            }

            await Task.WhenAll(tasks);

            var record = await _collection
                .Find(x => x.Id == Id3)
                .SingleAsync();
            Console.WriteLine($"Version 3. Count3: {count2}, Id: {Id3}, Point: {record.Point}.");
        }

        /// <summary>
        ///     FindOneAndUpdateAsync;
        ///     Query and Modify: $set;
        ///     Lost update
        /// </summary>
        public async Task Version4()
        {
            await ResetPoint(Id4);

            var tasks = new List<Task>();
            for (int i = 0; i < 300; i++)
            {
                tasks.Add(Task.Run(async () =>
                {
                    var record = await _collection
                        .Find(x => x.Id == Id4)
                        .SingleAsync();
                    record.Point++;
                    var filter = Builders<Record>.Filter
                        .Where(x => x.Id == Id4);
                    var update = Builders<Record>.Update
                        .Set(x => x.Point, record.Point);
                    count4 = count4 + 1;
                    await _collection.FindOneAndUpdateAsync(filter, update);
                }));
            }

            await Task.WhenAll(tasks);

            var record = await _collection
                .Find(x => x.Id == Id4)
                .SingleAsync();
            Console.WriteLine($"Version 4. Count4: {count4}, Id: {Id4}, Point: {record.Point}.");
        }

        /// <summary>
        ///     Standalone servers do not support transactions.
        /// </summary>
        public async Task Version5()
        {
            await ResetPoint(Id5);

            var tasks = new List<Task>();
            for (int i = 0; i < 300; i++)
            {
                var task = Task.Run(async () =>
                {
                    var client = new MongoClient("mongodb://localhost:2000");
                    var database = client.GetDatabase("gift");
                    var collection = database.GetCollection<Record>("records");

                    using (var session = await client.StartSessionAsync())
                    {
                        session.StartTransaction();

                        var record = await collection
                            .Find(x => x.Id == Id5)
                            .SingleAsync();
                        record.Point++;
                        var filter = Builders<Record>.Filter
                            .Where(x => x.Id == Id5);
                        var update = Builders<Record>.Update
                            .Set(x => x.Point, record.Point);
                        count5 = count5 + 1;
                        await collection.UpdateOneAsync(filter, update);
                        await session.CommitTransactionAsync();
                    }
                });
                tasks.Add(task);
            }

            await Task.WhenAll(tasks);
            var record = await _collection
                .Find(x => x.Id == Id5)
                .SingleAsync();
            Console.WriteLine($"Version 5. Count5: {count5}, Id: {Id5}, Point: {record.Point}.");
        }
    }

    public class Record
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("point")]
        public int Point { get; set; }
    }
}