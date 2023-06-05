using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using MongoDBSetup.Data;
using MongoDBSetup.Kafka;
using MongoDBSetup.Models;
using MongoDBSetup.ViewModels;

namespace MongoDBSetup.Services
{
    public class StudentService : IStudentService
    {
        private readonly IDbContext _dbContext;
        private readonly IMongoDBSettings _dbSettings;
        private readonly BsonDocument _ForeignKeyStringToObjectIdPipeline = new("$addFields", new BsonDocument
        {
            { "gender", new BsonDocument("$toObjectId", "$gender") },
            { "courses", new BsonDocument("$map",
                new BsonDocument
                {
                    { "input", "$courses" },
                    { "as", "course" },
                    { "in", new BsonDocument("$toObjectId", "$$course") }
                })
            }
        });
        private readonly BsonDocument _UnwindGendersPipeline = new("$unwind", new BsonDocument
        {
            { "path", "$gender" },
            { "preserveNullAndEmptyArrays", true }
        });
        private readonly BsonDocument _LookupGenderPipeline;
        private readonly BsonDocument _LookupCoursesPipeline;
        private BsonDocument _MatchPipeline(string key, BsonValue value) => new("$match", new BsonDocument(key, value));
        private BsonDocument _MatchByIdPipeline(string id) => _MatchPipeline("_id", new ObjectId(id));

        public StudentService(IDbContext dbContext, IMongoDBSettings dBSettings)
        {
            _dbContext = dbContext;
            _dbSettings = dBSettings;
            _LookupGenderPipeline = new BsonDocument("$lookup", new BsonDocument
            {
                { "from", _dbSettings.GenderCollection },
                { "localField", "gender" },
                { "foreignField", "_id" },
                { "as", "gender" }
            });
            _LookupCoursesPipeline = new BsonDocument("$lookup", new BsonDocument
            {
                { "from", _dbSettings.CourseCollection },
                { "localField", "courses" },
                { "foreignField", "_id" },
                { "as", "courses" }
            });
        }
        public Student Create(Student student)
        {
            _dbContext.Students.InsertOne(student);
            return student;
        }

        public List<StudentCombined> Get()
        {
            var pipelines = PipelineDefinition<Student, StudentCombined>.Create(new BsonDocument[]
            {
                _ForeignKeyStringToObjectIdPipeline,
                _LookupGenderPipeline,
                _LookupCoursesPipeline,
                _UnwindGendersPipeline
            });
            var result = _dbContext.Students.Aggregate(pipelines).ToList();
            return result;
        }

        public StudentCombined Get(string id)
        {
            var pipelines = PipelineDefinition<Student, StudentCombined>.Create(new BsonDocument[]
            {
                _MatchByIdPipeline(id),
                _ForeignKeyStringToObjectIdPipeline,
                _LookupGenderPipeline,
                _LookupCoursesPipeline,
                _UnwindGendersPipeline
            });
            var result = _dbContext.Students.Aggregate(pipelines).FirstOrDefault();
            return result;
        }

        public void Update(string id, Student student)
        {
            _dbContext.Students.ReplaceOne(s => s.Id.ToString() == id, student);
        }

        public void Delete(string id)
        {
            _dbContext.Students.DeleteOne(student => student.Id.ToString() == id);
        }
    }
}
