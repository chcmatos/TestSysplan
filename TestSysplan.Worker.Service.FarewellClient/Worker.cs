using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using TestSysplan.Core.Infrastructure.Logger;
using TestSysplan.Core.Infrastructure.Messenger;
using TestSysplan.Core.Infrastructure.Messenger.Services;
using TestSysplan.Core.Model;

namespace TestSysplan.Worker.Service.FarewellClient
{
    public class Worker : BackgroundService
    {
        private const int MAX_ATTEMPT_RESTART = 5;
        private const int MBROKER_START_DELAY_IN_MILLIS = 5000;
        private const int ATTEMPT_DELAY_IN_MILLIS = 1000;

        private readonly ILogger<Worker> _logger;
        private readonly IConsumeMessageService _messageService;
        private int attempt;

        public Worker(ILogger<Worker> logger, IConsumeMessageService messageService)
        {
            _logger = logger;
            _messageService = messageService;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogI("Service has been started!");
            return base.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _messageService.UnregisterAll();
            _logger.LogI("Service has been stopped!");
            return base.StopAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Delay(MBROKER_START_DELAY_IN_MILLIS);

            while (!stoppingToken.IsCancellationRequested)
            {
                bool done;
                try
                {
                    done = await _messageService.RegisterConsumeAsync<Client>(
                        OnConsumedCallback,
                        MessageRountingKeys.CLIENT_DELETED);
                }
                catch (Exception ex)
                {
                    done = false;
                    _logger.LogE(ex);
                }

                if (!done)
                {
                    if (++attempt > MAX_ATTEMPT_RESTART)
                    {
                        break;//muitas tentativas de reinicio sem sucesso.
                    }
                    else
                    {
                        await Task.Delay(ATTEMPT_DELAY_IN_MILLIS);
                    }
                }
            }
        }

        private void OnConsumedCallback(Client client)
        {
            _logger.LogInformation($"Ah, que pena que vc tem que ir {client.Name} :(");
        }
    }
}
