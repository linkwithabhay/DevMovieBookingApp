using MongoDB.Driver;
using MongoDBSetup.Models;

namespace MongoDBSetup.Services
{
    public class StudentService : IStudentService
    {
        private readonly IMongoCollection<Student> _Students;

        public StudentService(IMongoDBSettings settings, IMongoClient mongoClient)
        {
            var database = mongoClient.GetDatabase(settings.DatabaseName);
            _Students = database.GetCollection<Student>(settings.CollectionName);
        }
        public Student Create(Student student)
        {
            _Students.InsertOne(student);
            return student;
        }

        public List<Student> Get()
        {
            return _Students.Find(student => true).ToList();
        }

        public Student Get(string id)
        {
            return _Students.Find(student => student.Id == id).FirstOrDefault();
        }

        public void Update(string id, Student student)
        {
            _Students.ReplaceOne(s => s.Id == id, student);
        }

        public void Delete(string id)
        {
            _Students.DeleteOne(student => student.Id == id);
        }
    }
}
