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
        #region [C]reate
        [Fact]
        [Trait("RestApiVerb", "POST")]
        [Trait("Operation", "Insert")]
        public async void InsertANewValue()
        {
            #region  Arrange (Preparar)
            var item = this.MockValues().First();
            item.Id = default;
            item.Uuid = default;
            var mock = new Mock<TService>();            
            mock.Setup(s => s.ExistsAsync(item, default)).Returns(Task.FromResult(false));
            mock.Setup(s => s.InsertAsync(item, default)).Returns<TModel, CancellationToken>((t, ct) =>
            {
                t.Id = new Random().Next();
                t.Uuid = Guid.NewGuid();
                return Task.FromResult(t);
            });
            #endregion

            #region Act (Agir)
            var controller = GetController(mock);
            var result = await controller.CreateAsync(item);
            #endregion

            #region Assert (Verificar)
            var ok = Assert.IsType<OkObjectResult>(result);
            var value = Assert.IsAssignableFrom<TModel>(ok.Value);
            Assert.Equal(item, value);
            Assert.NotEqual(default, item.Id);
            Assert.NotEqual(default, item.Uuid);
            #endregion
        }

        [Fact]
        [Trait("RestApiVerb", "POST")]
        [Trait("Operation", "Insert")]
        [Trait("Operation", "Error")]
        public async void InsertANewValueButReturningAsExistingWithBadRequest()
        {
            #region  Arrange (Preparar)            
            var item = this.MockValues().First();
            var mock = new Mock<TService>();
            mock.Setup(s => s.ExistsAsync(item, default)).Returns(Task.FromResult(true));
            #endregion

            #region Act (Agir)
            var controller = GetController(mock);
            var result = await controller.CreateAsync(item);
            #endregion

            #region Assert (Verificar)
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            string error = Assert.IsType<string>(badRequest.Value);
            Assert.NotEmpty(error);
            Assert.NotEqual(default, item.Id);
            Assert.NotEqual(default, item.Uuid);
            #endregion
        }
        #endregion
    }
}
