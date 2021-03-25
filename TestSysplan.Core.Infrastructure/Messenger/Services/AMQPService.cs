using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestSysplan.Core.Infrastructure.Logger;
using TestSysplan.Core.Model;

namespace TestSysplan.Core.Infrastructure.Messenger.Services
{
    internal sealed class AMQPService : IMessageService
    {
        private readonly ILogger logger;

        public AMQPService(ILogger logger)
        {
            this.logger = logger;
        }

        #region Publish
        private void PublishAny<TModel>(TModel model, string rountingKey)
        {
            try
            {
                SimplePublisher.GetInstance().SendMessage(model, rountingKey);
            }
            catch (Exception ex)
            {
                //TODO add to redis cache.
                logger.LogE(ex);
#if DEBUG
                throw;
#endif
            }
        }

        public void Publish<TModel>(TModel model, string rountingKey) where TModel : ModelBase
        {
            Task.Factory.StartNew(s =>
            {
                if (s is Tuple<TModel, string> t)
                {
                    PublishAny(t.Item1, t.Item2);
                }
            }, Tuple.Create(model, rountingKey));
        }

        public void Publish<TModel>(IEnumerable<TModel> model, string rountingKey) where TModel : ModelBase
        {
            model.AsParallel().ForAll(m => Publish(m, rountingKey));
        }
        #endregion

        #region Consume
        public bool RegisterConsume<TModel>(Action<TModel> onConsumedCallback, string rountingKey, bool requeue, ushort balance)
        {
            try
            {
                return SimpleConsumer.GetInstance().Register(
                    onConsumedCallback: onConsumedCallback,
                    queueName: rountingKey,
                    requeue: requeue,
                    balanceCount: balance);
            } 
            catch (Exception ex)
            {
                logger.LogE(ex);
#if DEBUG
                throw;
#else
                return false;
#endif
            }
        }

        public Task<bool> RegisterConsumeAsync<TModel>(Action<TModel> onConsumedCallback, string rountingKey, bool requeue, ushort balance)
        {
            try
            {
                return SimpleConsumer.GetInstance().RegisterAsync(
                    onConsumedCallback: onConsumedCallback,
                    queueName: rountingKey,
                    requeue: requeue,
                    balanceCount: balance);
            }
            catch (Exception ex)
            {
                logger.LogE(ex);
#if DEBUG
                throw;
#else
                return Task.FromResult(false);
#endif
            }
        }

        public bool UnregisterConsume<TModel>(string rountingKey)
        {
            return SimpleConsumer.GetInstance().Unregister<TModel>(rountingKey);
        }

        public void UnregisterAll()
        {
            SimpleConsumer.GetInstance().UnregisterAll();
        }
        #endregion
    }
}
