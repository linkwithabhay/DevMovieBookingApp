using Confluent.Kafka;

Console.WriteLine("KAFKA CONSUMER");

const string TOPIC1 = "topic_get_student_data";
const string TOPIC2 = "topic_post_student_data";

var config = new ConsumerConfig
{
    GroupId = "unique_group_id",
    BootstrapServers = "broker:29092",
    AutoOffsetReset = AutoOffsetReset.Earliest
};

using var consumer = new ConsumerBuilder<Null, string>(config).Build();
consumer.Subscribe(new[] { TOPIC1, TOPIC2 });
while (true)
{
    try
    {
        var result = consumer.Consume();
        Console.WriteLine(new
        {
            result.Topic,
            result.Message.Value,
            TimeStamp = result.Message.Timestamp.UtcDateTime,
        });
    }
    catch (Exception ex)
    {
        Console.WriteLine(new { ex });
    }
}