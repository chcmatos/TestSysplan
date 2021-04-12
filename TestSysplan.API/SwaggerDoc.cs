using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

namespace TestSysplan.API
{
    internal static class SwaggerDocConfigurationExtensions
    {
        public static string ResolveSwaggerDoc(this IConfiguration configuration, 
            Func<Assembly, string> assemblyCallback,
            params string[] candidateKeys)
        {
            string[] prefixes = new[] { "SwaggerDoc", "Swagger" };
            
            foreach(string suffix in candidateKeys)
            {
                foreach(string prefix in prefixes)
                {
                    string key = string.Join(':', prefix, suffix);
                    string value = configuration[key];

                    if(!string.IsNullOrWhiteSpace(value))
                    {
                        return value;
                    }
                }
            }

            return assemblyCallback?.Invoke(Assembly.GetEntryAssembly());
        }

        public static void SwaggerDoc(this SwaggerGenOptions options, SwaggerDoc doc)
        {
            options.SwaggerDoc(doc.Version, doc);
        }
    }

    /// <summary>
    /// <para>
    /// Swagger doc informations.
    /// </para>
    /// <para>
    /// To create an instance use <see cref="Factory.Create(IConfiguration, string)"/>.
    /// </para>
    /// <para>
    /// Bellow an example how to implement swaggerdoc information in appsetting.json.<br/>
    /// Whether not defined it, the <see cref="Factory"/> will try to create it from assembly definition.
    /// </para>
    /// <code>
    /// "SwaggerDoc": {<br/>
    /// "Title": "Your API Name",<br/>
    /// "Description": "Your API description",<br/>
    /// "Author": {<br/>
    /// "Name": "Author name",<br/>
    /// "Email": "author@mail.com",<br/>
    /// "Url": "https://yourprojectaddress"<br/>
    /// }}<br/>
    /// </code>
    /// </summary>
    internal sealed class SwaggerDoc
    {
        public static implicit operator OpenApiInfo(SwaggerDoc doc)
        {
            return new OpenApiInfo
            {
                Version = doc.Version,
                Title = doc.Title,
                Description = doc.Description,
                Contact = doc.Author
            };
        }

        /// <summary>
        /// <para>
        /// Factory to generate a swaggerdoc from appsettings.json or from assembly info.
        /// </para>
        /// <para>
        /// Bellow an example how to implement swaggerdoc information in appsetting.json.<br/>
        /// Whether not defined it, the Factory will try to create it from assembly definition.
        /// </para>
        /// <code>
        /// "SwaggerDoc": {<br/>
        /// "Title": "Your API Name",<br/>
        /// "Description": "Your API description",<br/>
        /// "Author": {<br/>
        /// "Name": "Author name",<br/>
        /// "Email": "author@mail.com",<br/>
        /// "Url": "https://yourprojectaddress"<br/>
        /// }}<br/>
        /// </code>
        /// </summary>
        public static class Factory
        {
            /// <summary>
            /// Create an instance of <see cref="SwaggerDoc"/> from
            /// appsettings.json definition or from current entry assembly definition.
            /// </summary>
            /// <param name="configuration">appsetings.json configuration values</param>
            /// <param name="version">swagger api version, when null will try to load from configuration or assembly</param>
            /// <returns></returns>
            public static SwaggerDoc Create([NotNull] IConfiguration configuration, string version = null)
            {
                return new SwaggerDoc
                {
                    Version = version ?? configuration.ResolveSwaggerDoc(
                        a => a.GetName().Version.ToString(), 
                        "Version"),

                    Title = configuration.ResolveSwaggerDoc(
                        a => a.GetName().Name, 
                        "Title"),                    


                    Description = configuration.ResolveSwaggerDoc(
                        a => a.GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false)
                              .OfType<AssemblyDescriptionAttribute>()
                              .FirstOrDefault()?
                              .Description,  
                        "Description", "Desc"),

                    Author = new SwaggerAuthor
                    {
                        Name = configuration.ResolveSwaggerDoc(
                            a => FileVersionInfo.GetVersionInfo(a.Location).CompanyName, 
                            "Author:Name", "AuthorName", "Author"),

                        Email = configuration.ResolveSwaggerDoc(null, 
                            "Author:Email", "AuthorEmail", "Email"),

                        Url = Uri.TryCreate(
                                configuration.ResolveSwaggerDoc(null, 
                                    "Author:Url", "AuthorUrl", "Url"), 
                                UriKind.Absolute, 
                                out Uri uri) ? uri : default
                    }                                        
                };
            }
        }

        public sealed class SwaggerAuthor
        {
            public string Name { get; set; }
            
            public string Email { get; set; }

            public Uri Url { get; set; }

            public static implicit operator OpenApiContact(SwaggerAuthor author)
            {
                return new OpenApiContact
                {
                    Name = author.Name,
                    Email = author.Email,
                    Url = author.Url,
                };
            }
        }

        public string Title { get; private set; }

        public string Description { get; private set; }

        public string Version { get; private set; }

        public SwaggerAuthor Author { get; private set; }

        private SwaggerDoc() { }        

    }
}