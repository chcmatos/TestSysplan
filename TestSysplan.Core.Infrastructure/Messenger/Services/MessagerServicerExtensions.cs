using Microsoft.Extensions.DependencyInjection;
using TestSysplan.Core.Infrastructure.Services;

namespace TestSysplan.Core.Infrastructure.Messenger.Services
{
    public static class MessagerServicerExtensions
    {
        public static IServiceCollection AddAmqpService(this IServiceCollection services)
        {
            return services
                .AddSingleton<IMessageService, AMQPService>()
                .AddAmqpConsumerService()
                .AddAmqpPublisherService();
        }

        public static IServiceCollection AddAmqpConsumerService(this IServiceCollection services)
        {
            return services
                .AddSingleton<IConsumeMessageService, AMQPService>();
        }

        public static IServiceCollection AddAmqpPublisherService(this IServiceCollection services)
        {
            return services
                .AddSingleton<IPublishMessageService, AMQPService>();
        }
    }
}
