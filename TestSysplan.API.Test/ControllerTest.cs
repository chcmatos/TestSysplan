using TestSysplan.API.Controllers;
using TestSysplan.Core.Infrastructure.Services;
using TestSysplan.Core.Model;
using Xunit.Abstractions;

namespace TestSysplan.API.Test
{
    public abstract partial class ControllerTest<TModel, TService, TController> : ControllerTestBase<TModel, TService, TController>
        where TModel : ModelBase
        where TService : class, IServiceBase<TModel>
        where TController : Controller<TModel, TService>
    {
        public ControllerTest(ITestOutputHelper output) : base(output) { }
    }
}
