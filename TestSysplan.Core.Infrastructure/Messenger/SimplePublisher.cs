using RabbitMQ.Client;
using System;
using System.Text.Json;
using System.Threading;

namespace TestSysplan.Core.Infrastructure.Messenger
{
    internal sealed class SimplePublisher
    {
        private static readonly SimplePublisher instance;

        public static SimplePublisher GetInstance()
        {
            return instance;
        }

        static SimplePublisher()
        {
            instance = new SimplePublisher();
        }

        private SimplePublisher() { }

        public void SendMessage<TModel>(TModel target, string queueName = null, string exchange = "")
        {
            if (target is null)
            {
                throw new ArgumentException(nameof(target));
            }
            else if(queueName is null)
            {
                queueName = nameof(TModel);
            }
            else if (queueName.Trim().Length == 0)
            {
                throw new ArgumentException("Invalid queue name!");
            }

            using (var conn = AmqpConnectionWrapper.GetInstance())
            using (var channel = conn.CreateModel())
            {
                channel.QueueDeclare(
                    queue: queueName,
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                byte[] json = JsonSerializer.SerializeToUtf8Bytes(target,
                    new JsonSerializerOptions
                    {
                        IgnoreReadOnlyFields = true,
                        IgnoreReadOnlyProperties = true,
                        IgnoreNullValues = true
                    });

                channel.BasicPublish(
                    exchange: exchange,
                    routingKey: queueName,
                    basicProperties: null,
                    body: json);
                Thread.Sleep(200);
            }
        }
    }
}
