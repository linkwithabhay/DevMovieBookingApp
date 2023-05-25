using MongoDB.Driver;
using MongoDBSetup.Models;

namespace MongoDBSetup.Data
{
    public class DbContext : IDbContext
    {
        public IMongoCollection<Student> Students { get; private set; }

        public DbContext(IMongoClient mongoClient, IMongoDBSettings dBSettings)
        {
            var database = mongoClient.GetDatabase(dBSettings.DatabaseName);
            Students = database.GetCollection<Student>(dBSettings.CollectionName);
        }
    }
}
