using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Serilog;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using TestSysplan.Core.Infrastructure.Logger;

namespace TestSysplan.Core.Infrastructure.Messenger
{
    internal sealed class SimpleConsumer
    {
        private static readonly SimpleConsumer instance;

        private readonly ConcurrentDictionary<string, ManualResetEvent> resetEvents; 

        public static SimpleConsumer GetInstance()
        {
            return instance;
        }

        static SimpleConsumer()
        {
            instance = new SimpleConsumer();
        }

        private SimpleConsumer() 
        {
            resetEvents = new ConcurrentDictionary<string, ManualResetEvent>();
        }

        private string GetResetEventHash<TModel>(string queue)
        {
            return $"{typeof(TModel).FullName}_{queue}";
        }

        /// <summary>
        /// Registre um consumidor para obter mensagens da fila alvo.
        /// O processo ficará preso até solicitar o <see cref="Unregister{TModel}(string)"/> ou
        /// <see cref="UnregisterAll"/> que finalizará o consumer.
        /// </summary>
        /// <typeparam name="TModel">
        /// tipo do modelo serializado na mensagem na fila.
        /// </typeparam>
        /// <param name="onConsumedCallback">
        /// callback que será executado para receber o objeto json
        /// consumido da fila.
        /// </param>
        /// <param name="queueName">
        /// nome da fila
        /// </param>
        /// <param name="autoAck">
        /// true, define mensagem consumida automaticamente, false, 
        /// aguarda finalizacao do callback para confirmar consumo.
        /// </param>
        /// <param name="requeue">
        /// define se deve reenfileirar mensagem caso nao seja ocorra algum erro.
        /// </param>
        /// <param name="throwsException">
        /// define se deve lançar a exceção caso ocorra ao tentar consumir mensagem da fila.
        /// </param>
        /// <param name="balanceCount">
        /// caso maior que 0 (zero), o message broker vai entregar por requisição do consumer
        /// apenas o numero de mensagens definido.
        /// Caso menor ou igual a 0 (zero), será verificado se existe a variavel de ambiente
        /// AMQP_DBC, caso exista com um valor numérico maior que zero, será considerado
        /// com a regra descrita acima, caso constrário, não habilita Qos Prefetch.
        /// Viavel para balanceamento de carga.
        /// </param>
        /// <param name="logger">
        /// log que armazenará mensagens de erro gerada.
        /// </param>
        /// <returns>
        /// true, processo foi finalizado corretamente através de um
        /// Unregister. false, processo foi interrompido por algum erro.
        /// </returns>
        public bool Register<TModel>(
            Action<TModel> onConsumedCallback,
            string queueName = null, 
            bool autoAck = false,
            bool requeue = true,
            bool throwsException = true,
            ushort balanceCount = 0 /*not enabled*/,
            ILogger logger = null)
        {
            if (queueName is null)
            {
                queueName = nameof(TModel);
            }
            else if (queueName.Trim().Length == 0)
            {
                throw new ArgumentException("Invalid queue name!");
            }
            
            Exception error = null;
            var hash = GetResetEventHash<TModel>(queueName);
            if(resetEvents.TryGetValue(hash, out var resetEvent))
            {
                throw new InvalidOperationException("Operation already registered before to " +
                    "the same model type and target queue!");
            }
            else if(!resetEvents.TryAdd(hash, resetEvent = new ManualResetEvent(false)))
            {
                return false;//another process did it.
            }

            try
            {
                using (var conn = AmqpConnectionWrapper.GetInstance())
                using (var channel = conn.CreateModel())
                {
                    if (balanceCount > 0 ||
                        (balanceCount = ConnectionEnvironment.AMQP_DBC.ToUInt16()) > 0)
                    {
                        channel.BasicQos(0u, balanceCount, false /*per consumer*/);
                    }

                    channel.QueueDeclare(
                        queue: queueName,
                        durable: false,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null);

                    var consumer = new EventingBasicConsumer(channel);

                    void received(object ch, BasicDeliverEventArgs args)
                    {
                        try
                        {
                            var body = args.Body.Span;
                            TModel model = JsonSerializer.Deserialize<TModel>(body);
                            onConsumedCallback.Invoke(model);
                            if (!autoAck && channel.IsOpen) channel.BasicAck(args.DeliveryTag, false);
                        }
                        catch (Exception ex)
                        {
                            if (!autoAck && channel.IsOpen) channel.BasicNack(args.DeliveryTag, false, requeue);
                            logger?.LogE(ex);
                            if (throwsException) error = ex;
                            resetEvent.Set();
                        }
                    }

                    void shutdown(object ch, ShutdownEventArgs args)
                    {
                        consumer.Received -= received;
                        consumer.Shutdown -= shutdown;
                        try
                        {
                            resetEvent.Set();
                        }
                        catch (ObjectDisposedException) { /*disposed*/ }
                    };

                    consumer.Received += received;
                    consumer.Shutdown += shutdown;

                    channel.BasicConsume(
                        queue: queueName,
                        autoAck: autoAck,
                        consumer: consumer);

                    bool result = resetEvent.WaitOne();
                    if (result && throwsException && error != null)
                    {
                        throw new AggregateException(error);
                    }

                    return !result;
                }
            }
            catch (Exception)
            {
                Unregister<TModel>(queueName);
                throw;
            }
        }

        /// <summary>
        /// Registre um consumidor para obter mensagens da fila alvo.
        /// O processo ficará em execução até solicitar o <see cref="Unregister{TModel}(string)"/> ou
        /// <see cref="UnregisterAll"/> que finalizará o consumer.
        /// </summary>
        /// <typeparam name="TModel">
        /// tipo do modelo serializado na mensagem na fila.
        /// </typeparam>
        /// <param name="onConsumedCallback">
        /// callback que será executado para receber o objeto json
        /// consumido da fila.
        /// </param>
        /// <param name="queueName">
        /// nome da fila
        /// </param>
        /// <param name="autoAck">
        /// true, define mensagem consumida automaticamente, false, 
        /// aguarda finalizacao do callback para confirmar consumo.
        /// </param>
        /// <param name="requeue">
        /// define se deve reenfileirar mensagem caso nao seja ocorra algum erro.
        /// </param>
        /// <param name="throwsException">
        /// define se deve lançar a exceção caso ocorra ao tentar consumir mensagem da fila.
        /// </param>
        /// <param name="balanceCount">
        /// caso maior que 0 (zero), o message broker vai entregar por requisição do consumer
        /// apenas o numero de mensagens definido.
        /// Caso menor ou igual a 0 (zero), será verificado se existe a variavel de ambiente
        /// AMQP_DBC, caso exista com um valor numérico maior que zero, será considerado
        /// com a regra descrita acima, caso constrário, não habilita Qos Prefetch.
        /// Viavel para balanceamento de carga.
        /// </param>
        /// <param name="logger">
        /// log que armazenará mensagens de erro gerada.
        /// </param>
        /// <returns>
        /// true, processo foi finalizado corretamente através de um
        /// Unregister. false, processo foi interrompido por algum erro.
        /// </returns>
        public Task<bool> RegisterAsync<TModel>(Action<TModel> onConsumedCallback,
            string queueName = null,
            bool autoAck = false,
            bool requeue = true,
            bool throwsException = true,
            ushort balanceCount = 0 /*not enabled*/,
            ILogger logger = null)
        {
            return Task.Factory
                .StartNew(
                state =>
                    state is Tuple<Action<TModel>, string, bool, bool, bool, ushort, ILogger> t &&
                    Register(t.Item1, t.Item2, t.Item3, t.Item4, t.Item5, t.Item6, t.Item7),
                Tuple.Create(
                    onConsumedCallback,
                    queueName,
                    autoAck,
                    requeue,
                    throwsException,
                    balanceCount,
                    logger));
        }

        /// <summary>
        /// Finaliza e remove o consumidor em execução.
        /// </summary>
        /// <typeparam name="TModel">tipo do objeto alvo</typeparam>
        /// <param name="queueName">fila alvo</param>
        public bool Unregister<TModel>(string queueName = null)
        {
            var hash = GetResetEventHash<TModel>(queueName);
            bool result = resetEvents.TryRemove(hash, out var resetEvent);
            if(result) resetEvent.Dispose();
            return result;
        }

        /// <summary>
        /// Finaliza e remove todos os consumidores em execução.
        /// </summary>
        public void UnregisterAll()
        {
            resetEvents.Keys
                .ToList()
                .AsParallel()
                .ForAll(hash =>
                {
                    if (resetEvents.TryRemove(hash, out var re))
                    {
                        re.Dispose();
                    }
                });
        }
    }
}
