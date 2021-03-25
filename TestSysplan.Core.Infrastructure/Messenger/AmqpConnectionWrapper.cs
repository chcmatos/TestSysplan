using RabbitMQ.Client;
using System;
using System.Threading;

namespace TestSysplan.Core.Infrastructure.Messenger
{
    internal sealed class AmqpConnectionWrapper : IDisposable
    {
        private static readonly AmqpConnectionWrapper instance;
        private static readonly object locker;
        private static long count;
        
        private readonly Lazy<ConnectionFactory> factoryLazy;
        private IConnection current;

        public static AmqpConnectionWrapper GetInstance() => instance;

        static AmqpConnectionWrapper()
        {
            locker = new object();
            instance = new AmqpConnectionWrapper();
        }

        private AmqpConnectionWrapper()
        {
            factoryLazy = new Lazy<ConnectionFactory>(AmqpConnectionFactory.GetConnectionFactory);
        }

        private IConnection Unwrap()
        {
            lock (locker)
            {
                if (Interlocked.Increment(ref count) == 1L)
                {
                    current = factoryLazy.Value.CreateConnection();
                }

                return current;
            }
        }

        public IModel CreateModel() => Unwrap().CreateModel();

        public void Dispose()
        {
            lock(locker)
            {
                if (Interlocked.Read(ref count) > 0L &&
                    Interlocked.Decrement(ref count) == 0L)
                {
                    current?.Close();
                    current?.Dispose();
                }
            }
        }
    }
}
