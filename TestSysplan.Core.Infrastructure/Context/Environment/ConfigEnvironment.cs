using TestSysplan.Core.Infrastructure.Util;

namespace TestSysplan.Core.Infrastructure.Context.Environment
{
    public abstract class ConfigEnvironment
    {
        public static readonly LocalContextEnvironment LocalContext;

        public sealed class LocalContextEnvironment : ConfigEnvironment
        {
            internal LocalContextEnvironment() : base(
                "DATABASE_LOCAL_NAME",
                "DATABASE_LOCAL_HOST",
                "DATABASE_LOCAL_PORT",
                "DATABASE_LOCAL_USER",
                "DATABASE_LOCAL_PSWD",
                "DATABASE_LOCAL_TOUT")
            { }
        }

        public static readonly EnvironmentVariable DotnetRunningInContainer;

        public readonly EnvironmentVariable DatabaseName;
        public readonly EnvironmentVariable DatabaseHost;
        public readonly EnvironmentVariable DatabasePort;
        public readonly EnvironmentVariable DatabaseUser;
        public readonly EnvironmentVariable DatabasePswd;
        public readonly EnvironmentVariable DatabaseTout;

        static ConfigEnvironment()
        {
            DotnetRunningInContainer = "DOTNET_RUNNING_IN_CONTAINER";
            LocalContext = new LocalContextEnvironment();
        }

        protected ConfigEnvironment(string name, string host, string port, string user, string pswd, string tout)
        {
            DatabaseName = name;
            DatabaseHost = host;
            DatabasePort = port;
            DatabaseUser = user;
            DatabasePswd = pswd;
            DatabaseTout = tout;
        }
                        
    }
}
