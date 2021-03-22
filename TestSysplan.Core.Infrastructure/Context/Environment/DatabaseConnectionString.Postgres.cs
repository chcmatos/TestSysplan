using System.Reflection;

namespace TestSysplan.Core.Infrastructure.Context.Environment
{
    internal sealed class DatabaseConnectionStringPostgres : DatabaseConnectionString
    {
        public DatabaseConnectionStringPostgres(Builder builder) : base(builder) { }

        protected override string GetConnectionString(ConfigEnvironment env)
        {
            return 
                $"Server={env.DatabaseHost.GetValueOrDefault("localhost")};" +
                $"Port={env.DatabasePort.GetValueOrDefault(5432)};" +
                $"Database={env.DatabaseName};" +
                $"User Id={env.DatabaseUser};" +
                $"Password={env.DatabasePswd};" +
                $"Timeout={env.DatabaseTout.GetValueOrDefault(30)};" +
                $"Command Timeout={env.DatabaseTout.GetValueOrDefault(120)};" +
                $"ApplicationName={Assembly.GetEntryAssembly().GetName().Name}";
        }
    }
}