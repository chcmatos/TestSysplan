using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TestSysplan.Core.Infrastructure.Logger;
using TestSysplan.Core.Infrastructure.Messenger.Services;

namespace TestSysplan.Worker.Service.FarewellClient
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseLogServiceWithElasticsearch()
                .ConfigureServices((hostContext, services) =>
                {
                    services
                        .AddAmqpConsumerService()
                        .AddHostedService<Worker>();
                });
    }
}
