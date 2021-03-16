using TestSysplan.Core.Infrastructure.Context;
using TestSysplan.Core.Model;

namespace TestSysplan.Core.Infrastructure.Services
{
    internal sealed class ClientService : ServiceBase<Client, LocalContext>, IClientService
    {
        public ClientService(LocalContext context) : base(context, context?.Clients) { }
    }
}
