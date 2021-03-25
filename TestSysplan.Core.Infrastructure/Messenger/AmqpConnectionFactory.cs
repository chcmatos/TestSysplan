using RabbitMQ.Client;
using System;
using System.Security.Authentication;

namespace TestSysplan.Core.Infrastructure.Messenger
{
    internal static class AmqpConnectionFactory
    {
        private static SslProtocols GetSslProtocols()
        {
            if (ConnectionEnvironment.AMQP_SSL.TryGetValue(out string value))
            {
                var aux = value.Split(',', ';', '|');
                SslProtocols ssl = SslProtocols.None;
                foreach (var prot in aux)
                {
                    if (Enum.TryParse(prot, out SslProtocols sslProt))
                    {
                        ssl |= sslProt;
                    }
                }
                return ssl;
            }

            return SslProtocols.None;
        }

        internal static ConnectionFactory GetConnectionFactory()
        {
            var sslProtocols = GetSslProtocols();
            return new ConnectionFactory
            {
                HostName = ConnectionEnvironment.AMQP_HOST,
                Port = ConnectionEnvironment.AMQP_PORT,
                UserName = ConnectionEnvironment.AMQP_USER,
                Password = ConnectionEnvironment.AMQP_PASS,
                VirtualHost = ConnectionEnvironment.AMQP_VHOST,
                Ssl = new SslOption
                {
                    ServerName = ConnectionEnvironment.AMQP_HOST,
                    Enabled = sslProtocols != SslProtocols.None,
                    Version = sslProtocols
                }
            };
        }

        public static IConnection CreateConnection()
        {
            return GetConnectionFactory().CreateConnection();
        }
    }
}
