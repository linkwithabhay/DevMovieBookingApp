using MongoDB.Bson;
using MongoDB.Driver;
using MongoDBSetup.Models;

namespace MongoDBSetup.Data
{
    public class DbContext : IDbContext
    {
        public IMongoCollection<Student> Students { get; private set; }

        public IMongoCollection<Gender> Genders { get; private set; }

        public DbContext(IMongoClient mongoClient, IMongoDBSettings dBSettings)
        {
            var database = mongoClient.GetDatabase(dBSettings.DatabaseName);
            Students = database.GetCollection<Student>(dBSettings.StudentCollection);
            Genders = database.GetCollection<Gender>(dBSettings.GenderCollection);

            ConfigureModelsBuilder();
        }

        private void ConfigureModelsBuilder()
        {
            var studentOptions = new CreateIndexOptions<Student> { Unique = true };
            var studentIndexKeys = Builders<Student>.IndexKeys.Ascending(x => x.Name);
            var studentIndexModel = new CreateIndexModel<Student>(studentIndexKeys, studentOptions);
            Students.Indexes.CreateOne(studentIndexModel);

            var genderOptions = new CreateIndexOptions<Gender> {  Unique = true };
            var genderIndexKeys = Builders<Gender>.IndexKeys.Ascending(x => x.Name);
            var genderIndexModel = new CreateIndexModel<Gender>(genderIndexKeys, genderOptions);
            Genders.Indexes.CreateOne(genderIndexModel);
        }
    }
}
