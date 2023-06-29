using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MongodB.Poc.Gifts
{
    public class Record
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("point")]
        public int Point { get; set; }
    }
}