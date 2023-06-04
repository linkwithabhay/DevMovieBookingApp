namespace MongoDBSetup.Configurations
{
    public interface IKafkaConfig
    {
        string BootstrapServers { get; set; }
    }
}
