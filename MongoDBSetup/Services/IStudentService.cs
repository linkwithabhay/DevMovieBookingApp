using MongoDB.Bson;
using MongoDBSetup.Models;
using MongoDBSetup.ViewModels;

namespace MongoDBSetup.Services
{
    public interface IStudentService
    {
        List<StudentViewModel> Get();
        StudentViewModel Get(string id);
        Student Create(Student student);
        void Update(string id, Student student);
        void Delete(string id);
    }
}
