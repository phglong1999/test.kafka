using Confluent.Kafka;
using Newtonsoft.Json;
using System;

namespace DemoKafkaClient
{
    class Program
    {
        static void Main(string[] args)
        {
            var server = "192.168.10.123:9092,192.168.10.101:9092";
            var topic = "Kafka.Test";
            var groupId = "Kafka.Test.Id";
            int i = 0;
            Console.WriteLine("MENU");
            Console.WriteLine("1-Send message");
            Console.WriteLine("2-Receive message");
            Console.WriteLine("3-Continue");
            Console.WriteLine("4/any-Exit");
            Console.WriteLine("Please enter number:");
            //var str = Console.ReadLine();
            //int.TryParse(str, out i);


            do
            {
                //if (i == 1)
                {
                    #region Send message
                    var kafkaConfig = new ProducerConfig { BootstrapServers = server };
                    var producer = new ProducerBuilder<Null, string>(kafkaConfig).Build();

                    var data = new Student
                    {
                        Id = 619,
                        Name = "Doan to chau"
                    };
                    producer.ProduceAsync(topic, new Message<Null, string>
                    {
                        Value = JsonConvert.SerializeObject(data)
                    }).ContinueWith(c => { });

                    producer.Flush(TimeSpan.FromSeconds(5));
                    Console.WriteLine($"-->Send message:{JsonConvert.SerializeObject(data)}");
                    #endregion
                }
                //else if (i == 2)
                {
                    #region Recive message
                    var config = new ConsumerConfig
                    {
                        BootstrapServers = server,
                        GroupId = groupId,
                        EnableAutoCommit = true,
                        StatisticsIntervalMs = 5000,
                        SessionTimeoutMs = 6000,
                        AutoOffsetReset = AutoOffsetReset.Earliest,
                        EnablePartitionEof = true
                    };

                    const int commitPeriod = 5;
                    using (var consumer = new ConsumerBuilder<Ignore, string>(config)
                        .SetErrorHandler((_, e) => Console.WriteLine($"Error: {e.Reason}"))
                        .SetStatisticsHandler((_, json) => Console.Write($"."))
                        .SetPartitionsAssignedHandler((c, partitions) =>
                        {
                            Console.Write($">");
                        })
                        .SetPartitionsRevokedHandler((c, partitions) =>
                        {
                            Console.Write($"<");
                        })
                        .Build())
                    {
                        consumer.Subscribe(topic);

                        try
                        {

                            var consumeResult = consumer.Consume(TimeSpan.FromSeconds(100));
                            if (consumeResult == null)
                                Console.WriteLine("consumeResult is null");

                            if (consumeResult.IsPartitionEOF)
                            {
                                Console.WriteLine("_/_");

                            }
                            Console.WriteLine($"-->Receive message:{consumeResult.Message.Value}");
                            if (consumeResult.Offset % commitPeriod == 0)
                            {
                                try
                                {
                                    consumer.Commit(consumeResult);
                                }
                                catch (KafkaException e)
                                {
                                    Console.WriteLine($"Commit error: {e.Error.Reason}");
                                }
                            }
                        }
                        catch (OperationCanceledException)
                        {
                            Console.WriteLine("Closing consumer.");
                            consumer.Close();
                        }
                    }
                    #endregion
                }
                ////
                Console.WriteLine("1-Send message");
                Console.WriteLine("2-Receive message");
                Console.WriteLine("3-Continue");
                Console.WriteLine("4/any-Exit");
                //str = Console.ReadLine();
                //int.TryParse(str, out i);

            }
            while (i < 10);
            //while (i >= 1 && i <= 3) ;

            Console.WriteLine("Hello World!");
        }
    }
    public class Student
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
