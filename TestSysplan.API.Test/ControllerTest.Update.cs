using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Linq;
using TestSysplan.API.Controllers;
using TestSysplan.Core.Infrastructure.Services;
using TestSysplan.Core.Model;
using Xunit;

namespace TestSysplan.API.Test
{
    public abstract partial class ControllerTest<TModel, TService, TController>
        where TModel : ModelBase
        where TService : class, IServiceBase<TModel>
        where TController : Controller<TModel, TService>
    {
        #region [U]pdate
        [Fact]
        [Trait("RestApiVerb", "PUT")]
        [Trait("Operation", "Update")]
        public void UpdateExistentValue()
        {
            #region  Arrange (Preparar)
            var item = this.MockValues().Last();            
            var mock = new Mock<TService>();
            mock.Setup(s => s.Exists(item)).Returns(true);
            mock.Setup(s => s.Update(item)).Returns(item);
            #endregion

            #region Act (Agir)
            var controller = GetController(mock);
            var result = controller.Update(item);
            #endregion

            #region Assert (Verificar)
            var ok = Assert.IsType<OkResult>(result);
            Assert.NotEqual(default, item.Id);
            Assert.NotEqual(default, item.Uuid);
            #endregion
        }

        [Fact]
        [Trait("RestApiVerb", "PUT")]
        [Trait("Operation", "Update")]
        public void UpdateExistentValueButReturningAsNotFound()
        {
            #region  Arrange (Preparar)
            var item = this.MockValues().First();
            var mock = new Mock<TService>();
            mock.Setup(s => s.Exists(item)).Returns(false);
            #endregion

            #region Act (Agir)
            var controller = GetController(mock);
            var result = controller.Update(item);
            #endregion

            #region Assert (Verificar)
            Assert.IsType<NotFoundResult>(result);                        
            #endregion
        }
        
        [Fact]
        [Trait("RestApiVerb", "PUT")]
        [Trait("Operation", "Update")]
        [Trait("Operation", "Error")]
        public void UpdateExistentValueButReturningAsBadRequest()
        {
            #region  Arrange (Preparar)
            var ex      = new Exception("Simulated Error!");
            var mock    = new Mock<TService>();
            mock.Setup(s => s.Exists(null)).Returns<TModel>((t) => throw ex);
            #endregion

            #region Act (Agir)
            var controller = GetController(mock);
            var result = controller.Update(null);
            #endregion

            #region Assert (Verificar)
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(ex.Message, badRequest.Value);
            #endregion
        }
        #endregion
    }
}
