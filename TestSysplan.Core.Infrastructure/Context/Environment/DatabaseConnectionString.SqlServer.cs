namespace TestSysplan.Core.Infrastructure.Context.Environment
{
    internal sealed class DatabaseConnectionStringSqlServer : DatabaseConnectionString
    {
        public DatabaseConnectionStringSqlServer(Builder builder) : base(builder) { }

        protected override string GetConnectionString(ConfigEnvironment env)
        {
            return 
                $"Server={env.DatabaseHost.GetValueOrDefault(".")},{env.DatabasePort.GetValueOrDefault(1433)};" +
                $"Database={env.DatabaseName};" +
                $"User Id={env.DatabaseUser.GetValueOrDefault("sa")};" +
                $"Password={env.DatabasePswd};" +
                $"Integrated Security={!ConfigEnvironment.DotnetRunningInContainer};" +
                $"MultipleActiveResultSets=True;" +
                $"Connection Timeout={env.DatabaseTout.GetValueOrDefault(30)}";
        }
    }
}
