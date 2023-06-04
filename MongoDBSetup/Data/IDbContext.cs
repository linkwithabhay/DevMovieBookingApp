using MongoDB.Driver;
using MongoDBSetup.Models;

namespace MongoDBSetup.Data
{
    public interface IDbContext
    {
        IMongoCollection<Student> Students { get; }
        IMongoCollection<Gender> Genders { get; }
    }
}
