using MongoDB.Driver;
using MongoDBSetup.Data;
using MongoDBSetup.Kafka;
using MongoDBSetup.Models;

namespace MongoDBSetup.Services
{
    public class StudentService : IStudentService
    {
        private readonly IDbContext _dbContext;
        public StudentService(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public Student Create(Student student)
        {
            _dbContext.Students.InsertOne(student);
            return student;
        }

        public List<Student> Get()
        {
            return _dbContext.Students.Find(student => true).ToList();
        }

        public Student Get(string id)
        {
            return _dbContext.Students.Find(student => student.Id == id).FirstOrDefault();
        }

        public void Update(string id, Student student)
        {
            _dbContext.Students.ReplaceOne(s => s.Id == id, student);
        }

        public void Delete(string id)
        {
            _dbContext.Students.DeleteOne(student => student.Id == id);
        }
    }
}
