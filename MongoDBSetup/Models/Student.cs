using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MongoDBSetup.Models
{
    [BsonIgnoreExtraElements]
    public class Student
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("name")]
        public string Name { get; set; } = string.Empty;

        [BsonElement("gender")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Gender { get; set; } = string.Empty;

        [BsonElement("age")]
        public int Age { get; set; }
        
        [BsonElement("graduated")]
        public bool IsGraduated { get; set; }

        [BsonElement("courses")]
        public string[] Courses { get; set; } = Array.Empty<string>();
    }
}
