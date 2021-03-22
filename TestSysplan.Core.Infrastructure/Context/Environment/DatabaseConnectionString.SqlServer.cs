using System.Text;

namespace TestSysplan.Core.Infrastructure.Context.Environment
{
    internal sealed class DatabaseConnectionStringSqlServer : DatabaseConnectionString
    {
        public DatabaseConnectionStringSqlServer(Builder builder) : base(builder) { }

        protected override string GetConnectionString(ConfigEnvironment env)
        {
            return new StringBuilder()
                .Append("Data Source=").Append(env.DatabaseHost.GetValueOrDefault("."))
                .Append(",").Append(env.DatabasePort.GetValueOrDefault(1433)).Append(";")
                .Append("Initial Catalog=").Append(env.DatabaseName.Value).Append(";")
                .AppendIf(env.DatabaseUser.HasValue, "User Id=", env.DatabaseUser.Value, ';')
                .AppendIf(env.DatabasePswd.HasValue, "Password=", env.DatabasePswd.Value, ';')
                .AppendIf((!env.DatabaseUser.HasValue && !env.DatabasePswd.HasValue) || !ConfigEnvironment.DotnetRunningInContainer, "Integrated Security=True;")
                .AppendIf((!env.DatabaseUser.HasValue && !env.DatabasePswd.HasValue), "Trusted_Connection=True;")
                .Append("MultipleActiveResultSets=True;")
                .Append("Connection Timeout=").Append(env.DatabaseTout.GetValueOrDefault(30)).Append(";")
                .ToString();
        }
    }
}
