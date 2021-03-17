using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using TestSysplan.Core.Infrastructure.Logger;

namespace TestSysplan.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                        .UseLogWithElasticsearch()
                        .UseStartup<Startup>()
                        .UseKestrel()
                        .UseIISIntegration();
                });
    }
}
