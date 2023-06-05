using MongoDB.Bson;
using MongoDBSetup.Models;
using MongoDBSetup.ViewModels;

namespace MongoDBSetup.Services
{
    public interface IStudentService
    {
        List<StudentCombined> Get();
        StudentCombined Get(string id);
        Student Create(Student student);
        void Update(string id, Student student);
        void Delete(string id);
    }
}
