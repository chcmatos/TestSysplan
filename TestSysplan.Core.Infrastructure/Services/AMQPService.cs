using Serilog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TestSysplan.Core.Infrastructure.Logger;
using TestSysplan.Core.Infrastructure.Messenger;
using TestSysplan.Core.Model;

namespace TestSysplan.Core.Infrastructure.Services
{
    internal sealed class AMQPService : IMessageService
    {
        private readonly ILogger logger;

        public AMQPService(ILogger logger)
        {
            this.logger = logger;
        }

        private void PublishAny<TModel>(TModel model, string rountingKey)
        {
            try
            {
                SimplePublisher.GetInstance().SendMessage(model, rountingKey);
            }
            catch (Exception ex)
            {
                logger.LogE(ex);
#if DEBUG
                throw;
#endif
            }
        }

        public void Publish<TModel>(TModel model, string rountingKey) where TModel : ModelBase
        {
            this.PublishAny(model, rountingKey);
        }

        public void Publish<TModel>(IEnumerable<TModel> model, string rountingKey) where TModel : ModelBase
        {
            this.PublishAny(model, rountingKey);
        }
    }
}
