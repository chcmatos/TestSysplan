using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Linq;
using TestSysplan.Core.Infrastructure.Logger;
using TestSysplan.Core.Infrastructure.Services;

namespace TestSysplan.Core.Infrastructure.Context
{
    public sealed class ServiceProviderHelper
    {
        internal interface IConnectionKey
        {
            string Value { get; }
        }

        internal interface IServiceParams 
        { 
            string ConnectionString { get; }
        }

        internal class ServiceParams : IServiceParams
        {
            public string ConnectionString { get; private set; }

            public ServiceParams(IConfiguration configuration, IConnectionKey connectionKey)
            {
                this.ConnectionString = GetConnectionString(configuration, connectionKey);
            }

            private string GetConnectionString(IConfiguration configuration, IConnectionKey connectionKey)
            {
                string key = connectionKey.Value;

                string str;
                if(string.IsNullOrEmpty(key))
                {
                    str = configuration
                        ?.GetSection("ConnectionStrings")
                        ?.GetChildren()
                        ?.FirstOrDefault()
                        ?.Value;
                } 
                else
                {
                    key = key.StartsWith("ConnectionStrings:") ? key : "ConnectionStrings:" + key;
                    str = configuration[key];
                }

                return string.IsNullOrWhiteSpace(str) ?
                    throw new InvalidOperationException("ConnectionString not set!") :
                    str;
            }
        }

        internal class ConnectionKey : IConnectionKey
        {
            private readonly string value;

            public string Value => value;

            public ConnectionKey(string value)
            {
                this.value = value;
            }
        }

        private readonly IServiceCollection services;
        private IConnectionKey connectionKey;
        private IConfiguration configuration;

        public ServiceProviderHelper(IServiceCollection services)
        {
            this.services = services ?? throw new ArgumentNullException(nameof(services));
            this.connectionKey = new ConnectionKey(null);
        }

        public ServiceProviderHelper() : this(new ServiceCollection()) { }

        public ServiceProviderHelper UseAppSettings(IHostingEnvironment env = null)
        {
            var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: env != null, reloadOnChange: true);

            if(env != null)
            {
                builder.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);                
            }

            return UseConfiguration(builder.Build());
        }

        public ServiceProviderHelper UseConfiguration(IConfiguration configuration)
        {
            services.AddSingleton(this.configuration = configuration);
            return this;
        }

        public ServiceProviderHelper UseConnectionKey(string connectionKey)
        {
            this.connectionKey = new ConnectionKey(connectionKey);
            return this;
        }

        /// <summary>
        /// Enable to use all own local services.
        /// </summary>
        /// <returns></returns>
        public ServiceProviderHelper UseServices()
        {
            this.services.UseServices();
            return this;
        }

        /// <summary>
        /// To use log with elasticsearch inputing configuration 
        /// values, create into <i>appsettings.json</i> project 
        /// the follow field:
        /// <code>
        /// "Elasticsearch": {<br/>
        ///  "Url": "http://localhost:5151/elasticsearch",<br/>
        ///  "Appname":  "YourAppNameInElasticsearch"<br/>
        /// },
        /// </code>
        /// or create the follow simple field within only elasticsearch url,
        /// then the app name will be the current execunting assembly name.
        /// <code>
        /// "Elasticsearch": "http://localhost:5151/elasticsearch"
        /// </code>
        /// </summary>
        /// <param name="configuration">current configuration</param>
        /// <returns></returns>
        public ServiceProviderHelper UseLogWithElasticsearch(IConfiguration configuration = null)
        {
            this.services.UseLogWithElasticsearch(configuration ??
                this.configuration ?? throw new InvalidOperationException("Configuration not set before it!"));
            return this;
        }

        public ServiceProviderHelper AddScoped<TService, TImplementation>()
            where TService : class
            where TImplementation : class, TService
        {
            this.services.AddScoped<TService, TImplementation>();
            return this;
        }
    
        public ServiceProviderHelper AddSingleton<TService, TImplementation>()
            where TService : class
            where TImplementation : class, TService
        {
            this.services.AddSingleton<TService, TImplementation>();
            return this;
        }

        #region LocalContext
        public ServiceProviderHelper UseLocalContextAsSqlServer()
        {
            services.AddDbContext<LocalContext>((prov, options) => 
                options.UseSqlServer(prov.GetService<IServiceParams>()
                    .ConnectionString));
            return this;
        }

        public ServiceProviderHelper UseLocalContextAsSqlite()
        {
            services.AddDbContext<LocalContext>((prov, options) =>
                options.UseSqlite(prov.GetService<IServiceParams>()
                    .ConnectionString));
            return this;
        }

        public ServiceProviderHelper UseLocalContextAsPostgres()
        {
            services
                .AddEntityFrameworkNpgsql()
                .AddDbContext<LocalContext>((prov, options) =>
                options.UseNpgsql(prov.GetService<IServiceParams>()
                    .ConnectionString));
            return this;
        }
        #endregion

        public IServiceCollection GetServiceCollection()
        {
            return services
                .AddSingleton(connectionKey)
                .AddScoped<IServiceParams, ServiceParams>();
        }

        public ServiceProvider CreateServiceProvider()
        {
            try
            {
                return GetServiceCollection().BuildServiceProvider();
            } finally
            {
                this.configuration = null;
                this.connectionKey = null;
            }
        }

        public IServiceScope CreateScope()
        {
            return CreateServiceProvider().CreateScope();
        }
    }
}
