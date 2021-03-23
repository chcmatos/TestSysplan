using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using TestSysplan.API.Controllers;
using TestSysplan.Core.Model;
using Xunit;
using Xunit.Abstractions;

namespace TestSysplan.API.Test
{
    [Trait("Category", "Controller")]
    public abstract class ControllerTestBase<TModel, TService, TController>
        where TModel : ModelBase
        where TService : class
        where TController : ControllerBase<TModel, TService>
    {
        private readonly XUnitLogger<TController> logger;

        public ControllerTestBase(ITestOutputHelper output)
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
