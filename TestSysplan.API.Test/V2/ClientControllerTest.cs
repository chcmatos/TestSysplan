using System;
using System.Collections.Generic;
using TestSysplan.API.Controllers.V2;
using TestSysplan.Core.Infrastructure.Services;
using TestSysplan.Core.Model;
using Xunit;
using Xunit.Abstractions;

namespace TestSysplan.API.Test.V2
{
    [Trait("APIVersion", "V2")]
    [Trait("Controller", "Client")]
    public class ClientControllerTest : ControllerTestAsync<Client, IClientService, ClientController>
    {
        public ClientControllerTest(ITestOutputHelper output) : base(output) { }

        protected override IEnumerable<Client> MockValues()
        {
            yield return new Client { Id = 1, Uuid = Guid.NewGuid(), Name = "Client Test 1", Age = 20 };
            yield return new Client { Id = 2, Uuid = Guid.NewGuid(), Name = "Client Test 2", Age = 31 };
            yield return new Client { Id = 3, Uuid = Guid.NewGuid(), Name = "Client Test 3", Age = 42 };
            yield return new Client { Id = 4, Uuid = Guid.NewGuid(), Name = "Client Test 4", Age = 53 };
        }
    }
}
