using AvitoKafkaProducer.Models;
using Confluent.Kafka;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace AvitoKafkaConsumer
{
    class Program
    {
        private static readonly ConsumerConfig _consumerConfig = new ConsumerConfig
            {
                GroupId = "gid-consumers",
                BootstrapServers = "kafka-1:9092,kafka-2:9093,kafka-3:9094"
        };

        private static readonly ProducerConfig _producerConfig = new ProducerConfig
        {
            BootstrapServers = "kafka-1:9092,kafka-2:9093,kafka-3:9094",
            TopicMetadataRefreshIntervalMs = -1
        };

        private static readonly string _topicName = "messages";
        private static readonly string _deadQueueLetterTopicName = "deadletterqueue";

    static async Task Main(string[] args)
        {
            Console.WriteLine($"Starting listening on topic {_topicName}...");

            using (var consumer = new ConsumerBuilder<Null, string>(_consumerConfig).Build())
            {
                consumer.Subscribe(_topicName);
                while (true)
                {
                    var messageJson = consumer.Consume();

                    var message = JsonConvert.DeserializeObject<Message>(messageJson.Message.Value);

                    if (message.Type == MessageTypes.Message)
                    {
                        Console.WriteLine("Done!");
                    }
                    else
                    {
                        using (var producer = new ProducerBuilder<Null, string>(_producerConfig).Build())
                        {
                            await producer.ProduceAsync(_deadQueueLetterTopicName, new Message<Null, string> { Value = messageJson.Message.Value });
                            producer.Flush(TimeSpan.FromSeconds(10));
                        }
                        Console.WriteLine("Sent to DLQ");
                    }
                }
            }
        }
    }
}
