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
        #region [D]elete
        [Fact]
        [Trait("RestApiVerb", "DELETE")]
        [Trait("Operation", "Delete")]
        public void DeleteExistentValue()
        {
            #region  Arrange (Preparar)
            var item = this.MockValues().Last();
            var mock = new Mock<TService>();
            mock.Setup(s => s.Exists(item.Uuid)).Returns(true);
            mock.Setup(s => s.Delete(item.Uuid)).Returns(true);
            #endregion

            #region Act (Agir)
            var controller = GetController(mock);
            var result = controller.Delete(item.Uuid);
            #endregion

            #region Assert (Verificar)
            Assert.IsType<OkResult>(result);
            #endregion
        }

        [Fact]
        [Trait("RestApiVerb", "DELETE")]
        [Trait("Operation", "Delete")]
        public void DeleteExistentValueButReturningAsNotFound()
        {
            #region  Arrange (Preparar)
            var item = this.MockValues().First();
            var mock = new Mock<TService>();
            mock.Setup(s => s.Exists(item.Uuid)).Returns(false);
            #endregion

            #region Act (Agir)
            var controller = GetController(mock);
            var result = controller.Delete(item.Uuid);
            #endregion

            #region Assert (Verificar)
            Assert.IsType<NotFoundResult>(result);
            #endregion
        }

        [Theory]
        [Trait("RestApiVerb", "DELETE")]
        [Trait("Operation", "Delete")]
        [Trait("Operation", "Error")]
        [InlineData("00000000-0000-0000-0000-000000000000")]//invalid Uuid
        [InlineData("4a6020df-90ea-4e9b-bd44-8e3854703455")]//valid, exists, but not deleted.
        public void DeleteExistentValueButReturningAsBadRequest(Guid uuid)
        {
            #region  Arrange (Preparar)            
            var mock = new Mock<TService>();
            mock.Setup(s => s.Exists(uuid)).Returns(true);
            mock.Setup(s => s.Delete(uuid)).Returns(false);
            #endregion

            #region Act (Agir)
            var controller = GetController(mock);
            var result = controller.Delete(uuid);
            #endregion

            #region Assert (Verificar)
            var badRequest  = Assert.IsType<BadRequestObjectResult>(result);
            string error    = Assert.IsType<string>(badRequest.Value);
            Assert.NotEmpty(error);
            #endregion
        }
        #endregion
    }
}
