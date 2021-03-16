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
using TestSysplan.Core.Infrastructure.Context;

namespace TestSysplan.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        private void AddSwaggerDoc(SwaggerGenOptions options)
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
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

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            new ServiceProviderHelper(services)
               #region Infra
               .UseConfiguration(Configuration)
               .UseLocalContextAsSqlServer()
               .UseServices()
               .UseConnectionKey("Local")
               .GetServiceCollection()
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
               .AddSwaggerGen(options => AddSwaggerDoc(options))
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
