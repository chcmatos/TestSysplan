using System.Collections.Generic;
using TestSysplan.Core.Infrastructure.Context;
using TestSysplan.Core.Infrastructure.Messenger;
using TestSysplan.Core.Model;

namespace TestSysplan.Core.Infrastructure.Services
{
    internal sealed class ClientService : ServiceBase<Client, LocalContext>, IClientService
    {
        private readonly IMessageService messageService;

        public ClientService(LocalContext context, IMessageService messageService) : base(context, context?.Clients) 
        {
            this.messageService = messageService;
        }

        protected override void OnInsertedCallback(Client entity)
        {
            this.messageService.Publish(entity, MessageRountingKeys.CLIENT_INSERTED);
        }

        protected override void OnDeletedCallback(IEnumerable<Client> entities)
        {
            this.messageService.Publish(entities, MessageRountingKeys.CLIENT_DELETED);
        }
    }
}
