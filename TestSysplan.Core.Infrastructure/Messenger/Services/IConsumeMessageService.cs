using System;
using System.Threading.Tasks;

namespace TestSysplan.Core.Infrastructure.Messenger.Services
{
    public interface IConsumeMessageService
    {
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
        bool RegisterConsume<TModel>(Action<TModel> onConsumedCallback, 
            string rountingKey = null, 
            bool requeue = true, 
            ushort balance = 0);

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
        Task<bool> RegisterConsumeAsync<TModel>(Action<TModel> onConsumedCallback,
            string rountingKey = null,
            bool requeue = true,
            ushort balance = 0);

        /// <summary>
        /// Finaliza e remove o consumidor em execução.
        /// </summary>
        /// <typeparam name="TModel">tipo do objeto alvo</typeparam>
        /// <param name="queueName">fila alvo</param>
        bool UnregisterConsume<TModel>(string rountingKey = null);

        /// <summary>
        /// Finaliza e remove todos os consumidores em execução.
        /// </summary>
        void UnregisterAll();
    }
}
