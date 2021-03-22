using Microsoft.Extensions.Configuration;
using System;

namespace TestSysplan.Core.Infrastructure.Context.Environment
{
    internal abstract class DatabaseConnectionStringParameters : IDisposable
    {
        protected enum DatabaseTypes
        {
            NotSet,
            Postgres,
            SqlServer
        }

        protected IConfiguration Configuration { get; set; }

        protected string ConnectionStringKey { get; set; }

        protected ConfigEnvironment ConfigEnvironment { get; set; }

        protected DatabaseTypes DatabaseType { get; set; }

        ~DatabaseConnectionStringParameters()
        {
            Dispose(false);
        }

        protected DatabaseConnectionStringParameters() { }

        protected DatabaseConnectionStringParameters(DatabaseConnectionStringParameters other) 
        {
            this.Configuration          = other.Configuration;
            this.ConnectionStringKey    = other.ConnectionStringKey;
            this.ConfigEnvironment      = other.ConfigEnvironment;
            this.DatabaseType           = other.DatabaseType;
        }

        protected virtual void OnDispose(bool isDisposing) { }

        private void Dispose(bool isDisposing)
        {
            OnDispose(isDisposing);
            this.Configuration          = null;
            this.ConnectionStringKey    = null;
            this.ConfigEnvironment      = null;
            GC.SuppressFinalize(this);
        }

        void IDisposable.Dispose() => Dispose(true);
    }
}
