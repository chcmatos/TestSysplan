using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using TestSysplan.API.Controllers;
using TestSysplan.Core.Infrastructure.Services;
using TestSysplan.Core.Model;
using Xunit;
using Xunit.Abstractions;

namespace TestSysplan.API.Test
{
    [Trait("APIVersion", "V1")]
    [Trait("Category", "Controller")]
    public abstract partial class ControllerTest<TModel, TService, TController>
        where TModel : ModelBase
        where TService : class, IServiceBase<TModel>
        where TController : Controller<TModel, TService>
    {
        private readonly XUnitLogger<TController> logger;

        public ControllerTest(ITestOutputHelper output)
        {
            this.logger = new XUnitLogger<TController>(output);
        }

        protected TController GetController(Mock<TService> mock)
        {
            return GetController(mock.Object, logger);
        }

        protected virtual TController GetController(TService service, ILogger<TController> logger)
        {
            return (TController)Activator.CreateInstance(typeof(TController), service, logger);
        }

        protected abstract IEnumerable<TModel> MockValues();

    }
}
