using RabbitMQ.Client;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TestSysplan.Core.Util;

namespace TestSysplan.Core.Infrastructure.Messenger
{
    internal sealed class SimplePublisher
    {
        private static readonly SimplePublisher instance;

        private readonly HashSet<long> queuesDeclared;

        public static SimplePublisher GetInstance()
        {
            return instance;
        }

        static SimplePublisher()
        {
            instance = new SimplePublisher();
        }

        private SimplePublisher() 
        {
            queuesDeclared = new HashSet<long>();
        }

        #region SendMessage
        public void SendMessage(byte[] message, string queueName, string exchange = "", string routingKey = null)
        {
            if (message is null)
            {
                throw new ArgumentNullException(nameof(message));
            }
            else if(queueName != null && queueName.Trim().Length == 0)
            {
                throw new ArgumentException("Queue name can not be empty or whitespace!");
            }
            
            using (var conn = AmqpConnectionWrapper.GetInstance())
            using (var channel = conn.CreateModel())
            {
                if (queueName != null && NonDeclaredYet(queueName))
                {
                    channel.QueueDeclare(
                        queue: queueName,
                        durable: false,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null);
                }

                channel.BasicPublish(
                    exchange: exchange,
                    routingKey: routingKey ?? queueName ?? string.Empty,
                    basicProperties: null,
                    body: message);
            }
        }

        public void SendMessage(string message, string queueName, string exchange = "", string routingKey = null)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentNullException(nameof(message));
            }

            SendMessage(Encoding.UTF8.GetBytes(message), queueName, exchange, routingKey);
        }

        public void SendMessage<TModel>(TModel message, string queueName = null, string exchange = "", string routingKey = null)
        {
            if (message is null)
            {
                throw new ArgumentException(nameof(message));
            }
            else if (string.IsNullOrEmpty(queueName) && string.IsNullOrEmpty(exchange))
            {
                queueName = message.GetType().Name;
            }
            
            var json = message.ToJsonBytes();
            SendMessage(json, queueName, exchange, routingKey);
        }
        #endregion

        #region Local
        private bool NonDeclaredYet(params object[] args)
        {
            lock (queuesDeclared)
            {
                long hashcode = Objects.GetHashCode(args);
                return !queuesDeclared.Contains(hashcode) &&
                    queuesDeclared.Add(hashcode);
            }
        }

        private string GetExchange(string type, string[] queues) =>
            queues is null || queues.Length == 0 ?
            throw new ArgumentOutOfRangeException(nameof(queues)) :
            queues.Aggregate(type, (acc, curr) => acc + '_' + curr);

        private IDictionary<string, object> GetRoutingKeysDictionary(object routing)
        {
            return routing?.GetType().GetProperties().ToDictionary(
                k => k.Name, 
                k => k.GetValue(routing));
        }
        #endregion

        #region Setup        
        private IEnumerable<string> GetRountingKeysByValue(object routingValue)
        {
            if (routingValue is string str)
            {
                if (string.IsNullOrWhiteSpace(str))
                {
                    yield return string.Empty;
                }
                else
                {
                    foreach(string aux in str.Split(',', ';'))
                    {
                        yield return aux.Trim();
                    }
                }                
            }
            else if (routingValue is IEnumerable e)
            {
                var en = e.GetEnumerator();
                while (en.MoveNext())
                {
                    if (en.Current is string curr)
                    {
                        yield return curr;
                    }
                }
            }
            else
            {
                yield return string.Empty;
            }
        }

        private void Setup(string type, string exchange, IDictionary<string, object> routing)
        {
            if (string.IsNullOrWhiteSpace(exchange))
            {
                throw new ArgumentNullException(nameof(exchange));
            }
            else if(routing is null)
            {
                throw new ArgumentNullException(nameof(routing));
            }

            if (NonDeclaredYet(type, exchange, routing.Keys, routing.Values))
            {
                using (var conn = AmqpConnectionWrapper.GetInstance())
                using (var channel = conn.CreateModel())
                {
                    channel.ExchangeDeclare(exchange, type);

                    foreach (var r in routing)
                    {
                        if (NonDeclaredYet(r.Key))
                        {
                            channel.QueueDeclare(
                            queue: r.Key,
                            durable: false,
                            exclusive: false,
                            autoDelete: false,
                            arguments: null);
                        }

                        foreach (var rountingKey in GetRountingKeysByValue(r.Value))
                        {
                            channel.QueueBind(r.Key, exchange, rountingKey);
                        }
                    }
                }
            }
        }

        #region Fanout
        public void SetupAsFanout(string exchange, string[] queue)
        {
            if(queue is null)
            {
                throw new ArgumentNullException(nameof(queue));
            }
            else if(queue.Length == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(queue));
            }

            Setup("fanout", exchange, queue
                .ToDictionary(
                    k => k, 
                    e => default(object)));
        }

        public string SetupAsFanout(params string[] queue)
        {
            var exchange = GetExchange("f", queue);
            SetupAsFanout(exchange, queue);
            return exchange;
        }
        #endregion

        #region Direct
        public void SetupAsDirect(string exchange, IDictionary<string, object> routing)
        {
            Setup("direct", exchange, routing);
        }

        public void SetupAsDirect(string exchange, object routing)
        {
            SetupAsDirect(exchange, GetRoutingKeysDictionary(routing));
        }

        public string SetupAsDirect(IDictionary<string, object> routing)
        {
            var exchange = GetExchange("d", routing.Keys.ToArray());
            SetupAsDirect(exchange, routing);
            return exchange;
        }

        public string SetupAsDirect(object routing)
        {
            return SetupAsDirect(GetRoutingKeysDictionary(routing));
        }
        #endregion

        #region Topic
        public void SetupAsTopic(string exchange, IDictionary<string, object> routing)
        {
            Setup("topic", exchange, routing);
        }

        public void SetupAsTopic(string exchange, object routing)
        {
            SetupAsTopic(exchange, GetRoutingKeysDictionary(routing));
        }

        public string SetupAsTopic(IDictionary<string, object> routing)
        {
            var exchange = GetExchange("t", routing.Keys.ToArray());
            SetupAsTopic(exchange, routing);
            return exchange;
        }

        public string SetupAsTopic(object routing)
        {
            return SetupAsTopic(GetRoutingKeysDictionary(routing));
        }
        #endregion

        #endregion
    }
}
