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

        public StudentService(IDbContext dbContext, IMongoDBSettings dBSettings)
        {
            _dbContext = dbContext;
            _dbSettings = dBSettings;
        }
        public Student Create(Student student)
        {
            _dbContext.Students.InsertOne(student);
            return student;
        }

        public List<StudentViewModel> Get()
        {
            //return _dbContext.Students.Find(_ => true).ToList();
            var result = _dbContext.Students.Aggregate()
                .Lookup<Student, Gender, StudentViewModel>(_dbContext.Genders, x => x.Gender, x => x.Id, x => x.Genders)
                .ToList();
            return result;
        }

        public StudentViewModel Get(string id)
        {
            //return _dbContext.Students.Find(x => x.Id.Equals(id)).FirstOrDefault();
            var result = _dbContext.Students
                .Aggregate()
                .Match(x => x.Id.Equals(id))
                .Lookup<Student, Gender, StudentViewModel>(_dbContext.Genders, x => x.Gender, x => x.Id, x => x.Genders)
                .FirstOrDefault();
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
