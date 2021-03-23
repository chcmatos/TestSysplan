using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace TestSysplan.API.Test
{
    public abstract partial class ControllerTestAsync<TModel, TService, TController>
    {
        #region [D]elete
        [Fact]
        [Trait("RestApiVerb", "DELETE")]
        [Trait("Operation", "Delete")]
        public async void DeleteExistentValue()
        {
            #region  Arrange (Preparar)
            var item = this.MockValues().Last();
            var mock = new Mock<TService>();
            mock.Setup(s => s.ExistsAsync(item.Uuid, CancellationToken.None)).Returns(Task.FromResult(true));
            mock.Setup(s => s.DeleteAsync(item.Uuid, CancellationToken.None)).Returns(Task.FromResult(true));
            #endregion

            #region Act (Agir)
            var controller = GetController(mock);
            var result = await controller.DeleteAsync(item.Uuid);
            #endregion

            #region Assert (Verificar)
            Assert.IsType<OkResult>(result);
            #endregion
        }

        [Fact]
        [Trait("RestApiVerb", "DELETE")]
        [Trait("Operation", "Delete")]
        public async void DeleteExistentValueButReturningAsNotFound()
        {
            #region  Arrange (Preparar)
            var item = this.MockValues().First();
            var mock = new Mock<TService>();
            mock.Setup(s => s.ExistsAsync(item.Uuid, CancellationToken.None)).Returns(Task.FromResult(false));
            #endregion

            #region Act (Agir)
            var controller = GetController(mock);
            var result = await controller.DeleteAsync(item.Uuid);
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
        public async void DeleteExistentValueButReturningAsBadRequest(Guid uuid)
        {
            #region  Arrange (Preparar)            
            var mock = new Mock<TService>();
            mock.Setup(s => s.ExistsAsync(uuid, CancellationToken.None)).Returns(Task.FromResult(true));
            mock.Setup(s => s.DeleteAsync(uuid, CancellationToken.None)).Returns(Task.FromResult(false));
            #endregion

            #region Act (Agir)
            var controller = GetController(mock);
            var result = await controller.DeleteAsync(uuid);
            #endregion

            #region Assert (Verificar)
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            string error = Assert.IsType<string>(badRequest.Value);
            Assert.NotEmpty(error);
            #endregion
        }
        #endregion
    }
}
