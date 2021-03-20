using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TestSysplan.Core.Infrastructure.Services;
using TestSysplan.Core.Model;

namespace TestSysplan.API.Controllers.V2
{
    [ApiVersion("2")]
    public class ClientController : ControllerAsync<Client, IClientService>
    {
        public ClientController(IClientService service, 
            ILogger<ClientController> logger) : base(service, logger) { }
    }
}
