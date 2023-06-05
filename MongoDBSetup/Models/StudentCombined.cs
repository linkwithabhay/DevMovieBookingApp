using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MongoDBSetup.Models
{
    [BsonIgnoreExtraElements]
    public class StudentCombined
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("name")]
        public string Name { get; set; } = string.Empty;

        [BsonElement("gender")]
        public Gender Gender { get; set; } = new();

        [BsonElement("age")]
        public int Age { get; set; }

        [BsonElement("graduated")]
        public bool IsGraduated { get; set; }

        [BsonElement("courses")]
        public Course[] Courses { get; set; } = Array.Empty<Course>();
    }
}
