using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TestSysplan.Core.Infrastructure.Services;
using TestSysplan.Core.Model;

namespace TestSysplan.API.Controllers.V1
{
    [ApiVersion("1")]
    public class ClientController : Controller<Client, IClientService>
    {
        public ClientController(IClientService service, 
            ILogger<ClientController> logger) : base(service, logger) { }
    }
}
