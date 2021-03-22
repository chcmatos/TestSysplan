using System;
using System.Linq;

namespace TestSysplan.Core.Infrastructure.Context.Environment
{
    internal abstract partial class DatabaseConnectionString : DatabaseConnectionStringParameters
    {
        private readonly string connectionString;

        internal protected DatabaseConnectionString(Builder builder) : base(builder) 
        {
            this.connectionString = GetConnectionString();
        }

        protected abstract string GetConnectionString(ConfigEnvironment env);

        private string GetConnectionStringFromConfigurationProperties()
        {
            string key = this.ConnectionStringKey;

            if (string.IsNullOrWhiteSpace(key))
            {
                if (ConfigEnvironment.DotnetRunningInContainer)
                {
                    return null;//Key not set and app is running in container, preferences to environment config.
                }

                return Configuration
                    ?.GetSection("ConnectionStrings")
                    ?.GetChildren()
                    ?.FirstOrDefault()
                    ?.Value;
            }
            else
            {
                key = key.StartsWith("ConnectionStrings:") ? key : "ConnectionStrings:" + key;
                return this.Configuration is null ? null : this.Configuration[key] ?? 
                    throw new InvalidOperationException($"ConnectionString key ({this.ConnectionStringKey}) not found in appsettings.json!");
            }
        }

        private string GetConnectionStringFromConfigEnvironment()
        {
            return this.ConfigEnvironment is null ? null : 
                GetConnectionString(ConfigEnvironment) ?? throw new InvalidOperationException("Is not possible generate " +
                "ConnectionString from ConfigEnvironment for " +
                $"database type {DatabaseType}!");
        }

        private string GetConnectionString()
        {
            return this.GetConnectionStringFromConfigurationProperties() ??
                this.GetConnectionStringFromConfigEnvironment() ??
                throw new InvalidOperationException("Is not possible recovery or generate ConnectionString without set " +
                $"{nameof(Builder.UseConfiguration)} or {nameof(Builder.UseEnvironmentVariables)}!");
        }

        public override string ToString()
        {
            return connectionString;
        }

        public static implicit operator string(DatabaseConnectionString db)
        {
            return db.ToString();
        }
    }
}
