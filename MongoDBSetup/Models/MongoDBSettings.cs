namespace MongoDBSetup.Models
{
    public class MongoDBSettings : IMongoDBSettings
    {
        public string StudentCollection { get; set; } = String.Empty;
        public string GenderCollection { get; set; } = String.Empty;
        public string DatabaseName { get; set; } = String.Empty;
        public string ConnectionString { get; set; } = String.Empty;
    }
}
