using Confluent.Kafka;

namespace MongoDBSetup.Kafka
{
    public class KafkaProducerService : IKafkaProducerService
    {
        private readonly ILogger<KafkaProducerService> _logger;
        private readonly IProducer<Null, string> _producer;

        public KafkaProducerService(ILogger<KafkaProducerService> logger) {
            _logger = logger;
            _producer = new ProducerBuilder<Null, string>(new ProducerConfig { BootstrapServers = "broker:29092" }).Build();
        }

        public async Task SendToTopicAsync(string topic, string message)
        {
            try
            {
                await _producer.ProduceAsync(topic, new Message<Null, string> { Value = message });
                var log = "KAFKA_PRODUCER - " + $"Topic = '{topic}', Message = '{message}'";
                _logger.LogInformation(log);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Someting went wrong at Kafka Producer");
            }
        }
    }
}
