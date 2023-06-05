using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MongoDBSetup.Models
{
    [BsonIgnoreExtraElements]
    public class Course
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("name")]
        public string Name { get; set; } = string.Empty;

        [BsonElement("rating")]
        public int Rating { get; set; }
    }
}
