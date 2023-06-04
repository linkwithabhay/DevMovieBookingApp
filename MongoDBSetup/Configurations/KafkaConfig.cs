namespace MongoDBSetup.Configurations
{
    public class KafkaConfig : IKafkaConfig
    {
        public string BootstrapServers { get; set; } = string.Empty;
    }
}
