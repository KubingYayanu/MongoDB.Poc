using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MongodB.Poc.Concurrency
{
    public class PointExpiration
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("member")]
        public PointExpirationMember Member { get; set; }

        [BsonElement("point")]
        public PointExpirationPoint Point { get; set; }
    }

    public class PointExpirationMember
    {
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("member_id")]
        public string MemberId { get; set; }
    }

    public class PointExpirationPoint
    {
        [BsonElement("balance")]
        public int Balance { get; set; }
    }
}