using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MongodB.Poc.Concurrency
{
    public class PointRecord
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("member")]
        public PointRecordMember Member { get; set; }

        [BsonElement("point")]
        public PointRecordPoint Point { get; set; }
    }

    public class PointRecordMember
    {
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("member_id")]
        public string MemberId { get; set; }
    }

    public class PointRecordPoint
    {
        [BsonElement("gain")]
        public int Gain { get; set; }

        [BsonElement("balance")]
        public int Balance { get; set; }
    }
}