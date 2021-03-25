using TestSysplan.Core.Infrastructure.Util;

namespace TestSysplan.Core.Infrastructure.Messenger
{
    internal sealed class ConnectionEnvironment
    {
        public static readonly EnvironmentVariable AMQP_USER    = (nameof(AMQP_USER), "");
        public static readonly EnvironmentVariable AMQP_PASS    = (nameof(AMQP_PASS), "");
        public static readonly EnvironmentVariable AMQP_VHOST   = (nameof(AMQP_VHOST), "/");
        public static readonly EnvironmentVariable AMQP_HOST    = (nameof(AMQP_HOST), "localhost");
        public static readonly EnvironmentVariable AMQP_PORT    = (nameof(AMQP_PORT), 5672);
        public static readonly EnvironmentVariable AMQP_SSL     = (nameof(AMQP_SSL), "");
        public static readonly EnvironmentVariable AMQP_DBC     = (nameof(AMQP_DBC), 0);
    }
}
