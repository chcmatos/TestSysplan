using TestSysplan.API.Controllers;
using TestSysplan.Core.Infrastructure.Services;
using TestSysplan.Core.Model;
using Xunit.Abstractions;

namespace TestSysplan.API.Test
{
    public abstract partial class ControllerTestAsync<TModel, TService, TController> : ControllerTestBase<TModel, TService, TController>
        where TModel : ModelBase
        where TService : class, IServiceBaseAsync<TModel>
        where TController : ControllerAsync<TModel, TService>
    {
        public ControllerTestAsync(ITestOutputHelper output) : base(output) { }
    }
}
