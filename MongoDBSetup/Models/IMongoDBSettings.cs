namespace MongoDBSetup.Models
{
    public interface IMongoDBSettings
    {
        string StudentCollection { get; set; }
        string GenderCollection { get; set; }
        string CourseCollection { get; set; }
        string DatabaseName { get; set; }
        string ConnectionString { get; set; }
    }
}
