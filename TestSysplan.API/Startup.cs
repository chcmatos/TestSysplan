using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;
using System;
using System.Collections.Concurrent;
using TestSysplan.Core.Infrastructure.Context;
using TestSysplan.Core.Infrastructure.Logger;
using TestSysplan.Core.Infrastructure.Services;

namespace TestSysplan.API
{
    public class Startup
    {
        private readonly ConcurrentBag<string> versions;

        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
            this.versions = new ConcurrentBag<string>();
        }

        private void AddSwaggerDoc(SwaggerGenOptions options)
        {
            while (versions.TryTake(out string v))
            {
                options.SwaggerDoc(v, new OpenApiInfo
                {
                    Version = v,
                    Title = Configuration["SwaggerDoc:Title"],
                    Description = Configuration["SwaggerDoc:Description"],
                    Contact = new OpenApiContact
                    {
                        Name = Configuration["SwaggerDoc:Author:Name"],
                        Email = Configuration["SwaggerDoc:Author:Email"],
                        Url = new Uri(Configuration["SwaggerDoc:Author:Url"]),
                    }
                });
            }
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services
               #region Infra               
               .UseLogWithElasticsearch()
               .UseLocalContextAsSqlServer()
               .UseServices()
               #endregion
               
               #region ApiVersion
               .AddApiVersioning(p =>
               {
                   p.DefaultApiVersion = new ApiVersion(1, 0);
                   p.ReportApiVersions = true;
                   p.AssumeDefaultVersionWhenUnspecified = true;
               })
               .AddVersionedApiExplorer(p =>
               {
                   p.GroupNameFormat = "'v'VVV";
                   p.SubstituteApiVersionInUrl = true;
               })
               #endregion
               
               #region Swagger               
               .AddSwaggerGen(AddSwaggerDoc)
               #endregion
               
               .AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, 
            IWebHostEnvironment env, 
            IApiVersionDescriptionProvider provider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseApiVersioning()               
               .UseSwagger()
               .UseSwaggerUI(c =>
               {
                   foreach (var desc in provider.ApiVersionDescriptions)
                   {
                       c.SwaggerEndpoint(
                       $"/swagger/{desc.GroupName}/swagger.json",
                       desc.GroupName.ToUpperInvariant());
                       versions.Add(desc.GroupName);
                   }                   
                   c.DocExpansion(DocExpansion.List);
               })
               .UseHttpsRedirection()
               .UseAuthorization()
               .UseRouting()
               .UseEndpoints(endpoints => endpoints.MapControllers());
        }
    }
}
