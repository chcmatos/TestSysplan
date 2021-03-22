using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace TestSysplan.Core.Infrastructure.Context.Environment
{
    internal abstract partial class DatabaseConnectionString
    {
        internal sealed class Builder : DatabaseConnectionStringParameters
        {
            public Builder() { }

            public Builder UseConfiguration(IConfiguration configuration)
            {
                this.Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
                return this;
            }

            public Builder UseConfiguration(IServiceProvider provider)
            {
                return UseConfiguration((provider?.GetService<IConfiguration>() ??
                     throw new ArgumentNullException(nameof(provider))) ??
                     throw new ArgumentException("Service provider is do not attaching IConfiguration!"));
            }

            public Builder UseConnectionStringKey(string connectionStringKey)
            {
                this.ConnectionStringKey = connectionStringKey;
                return this;
            }

            public Builder UseEnvironmentVariables(ConfigEnvironment environment)
            {
                this.ConfigEnvironment = environment ?? throw new ArgumentNullException(nameof(environment));
                return this;
            }

            public Builder UseMsSqlServer()
            {
                this.DatabaseType = DatabaseTypes.SqlServer;
                return this;
            }

            public Builder UsePostgres()
            {
                this.DatabaseType = DatabaseTypes.Postgres;
                return this;
            }

            public DatabaseConnectionString Build()
            {
                using (this)
                {
                    switch (DatabaseType)
                    {
                        case DatabaseTypes.Postgres:
                            return new DatabaseConnectionStringPostgres(this);
                        case DatabaseTypes.SqlServer:
                            return new DatabaseConnectionStringSqlServer(this);
                        case DatabaseTypes.NotSet:
                        default:
                            throw new InvalidOperationException("Database type not set!");
                    }
                }
            }
        }
    }
}
