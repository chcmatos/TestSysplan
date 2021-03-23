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
        #region [U]pdate
        [Fact]
        [Trait("RestApiVerb", "PUT")]
        [Trait("Operation", "Update")]
        public async void UpdateExistentValue()
        {
            #region  Arrange (Preparar)
            var item = this.MockValues().Last();
            var mock = new Mock<TService>();
            mock.Setup(s => s.ExistsAsync(item, CancellationToken.None)).Returns(Task.FromResult(true));
            mock.Setup(s => s.UpdateAsync(item, CancellationToken.None)).Returns(Task.FromResult(item));
            #endregion

            #region Act (Agir)
            var controller = GetController(mock);
            var result = await controller.UpdateAsync(item);
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
        public async void UpdateExistentValueButReturningAsNotFound()
        {
            #region  Arrange (Preparar)
            var item = this.MockValues().First();
            var mock = new Mock<TService>();
            mock.Setup(s => s.ExistsAsync(item, CancellationToken.None)).Returns(Task.FromResult(false));
            #endregion

            #region Act (Agir)
            var controller = GetController(mock);
            var result = await controller.UpdateAsync(item);
            #endregion

            #region Assert (Verificar)
            Assert.IsType<NotFoundResult>(result);
            #endregion
        }

        [Fact]
        [Trait("RestApiVerb", "PUT")]
        [Trait("Operation", "Update")]
        [Trait("Operation", "Error")]
        public async void UpdateExistentValueButReturningAsBadRequest()
        {
            #region  Arrange (Preparar)
            var ex = new Exception("Simulated Error!");
            var mock = new Mock<TService>();
            mock.Setup(s => s.ExistsAsync(null, CancellationToken.None)).Returns<TModel, CancellationToken>((t, ct) => throw ex);
            #endregion

            #region Act (Agir)
            var controller = GetController(mock);
            var result = await controller.UpdateAsync(null);
            #endregion

            #region Assert (Verificar)
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(ex.Message, badRequest.Value);
            #endregion
        }
        #endregion
    }
}
