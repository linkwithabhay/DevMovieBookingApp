namespace MongoDBSetup.Kafka
{
    public interface IKafkaProducerService
    {
        Task SendToTopicAsync(string topic, string message);
    }
}
