using RabbitMQ.Client;
using System;
using System.Linq;
using System.Text;
using System.Text.Json;

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

        #region SendMessage
        public void SendMessage(byte[] message, string queueName)
        {
            if (message is null)
            {
                throw new ArgumentNullException(nameof(message));
            }
            else if (string.IsNullOrWhiteSpace(queueName))
            {
                throw new ArgumentNullException(nameof(queueName));
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

                channel.BasicPublish(
                    exchange: string.Empty,
                    routingKey: queueName,
                    basicProperties: null,
                    body: message);
            }
        }

        public void SendMessage(string message, string queueName, Encoding encoding = null)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentNullException(nameof(message));
            }

            encoding ??= Encoding.Default;
            SendMessage(encoding.GetBytes(message), queueName);
        }

        public void SendMessage<TModel>(TModel target, string queueName = null)
        {
            if (target is null)
            {
                throw new ArgumentException(nameof(target));
            }
            else if (queueName is null)
            {
                queueName = nameof(TModel);
            }
            else if (queueName.Trim().Length == 0)
            {
                throw new ArgumentException("Invalid queue name!");
            }

            var json = JsonSerializer.SerializeToUtf8Bytes(target, new JsonSerializerOptions
            {
                IgnoreReadOnlyFields = true,
                IgnoreReadOnlyProperties = true,
                IgnoreNullValues = true
            });

            SendMessage(json, queueName);
        }
        #endregion

        public void SendMessageExchangeFanout(byte[] message, string exchange, params string[] queues)
        {
            if (message is null)
            {
                throw new ArgumentNullException(nameof(message));
            }
            else if (queues.Length == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(queues));
            }
            else if (string.IsNullOrWhiteSpace(exchange))
            {
                throw new ArgumentNullException(nameof(exchange));
            }

            using (var conn = AmqpConnectionWrapper.GetInstance())
            using (var channel = conn.CreateModel())
            {
                channel.ExchangeDeclare(exchange, "fanout");

                foreach(var queue in queues)
                {
                    channel.QueueDeclare(
                    queue: queue,
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                    channel.QueueBind(queue, exchange, string.Empty);
                }

                channel.BasicPublish(
                    exchange: exchange,
                    routingKey: string.Empty,
                    basicProperties: null,
                    body: message);
            }
        }

        public void SendMessageExchangeFanout(byte[] message, params string[] queues)
        {
            this.SendMessageExchangeFanout(message, string.Join('_', queues), queues);
        }
    }
}
