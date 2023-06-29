using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MongodB.Poc.Concurrency
{
    public class Member
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("point")]
        public MemberPoint Point { get; set; }
    }

    public class MemberPoint
    {
        [BsonElement("balance")]
        public int Balance { get; set; }
    }
}