namespace MongoDBSetup.Models
{
    public interface IMongoDBSettings
    {
        string CollectionName { get; set; }
        string DatabaseName { get; set; }
        string ConnectionString { get; set; }
    }
}
