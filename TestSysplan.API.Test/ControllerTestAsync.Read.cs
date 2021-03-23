using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace TestSysplan.API.Test
{
    public abstract partial class ControllerTestAsync<TModel, TService, TController>
    {
        #region [R]ead

        #region Get All
        [Fact]
        [Trait("RestApiVerb", "GET")]
        [Trait("Operation", "GetAll")]
        public async void GetAllValues()
        {
            #region  Arrange (Preparar)
            var list = this.MockValues().ToList();
            var mock = new Mock<TService>();
            mock.Setup(s => s.ListAsync(CancellationToken.None)).Returns(Task.FromResult(list));
            #endregion

            #region Act (Agir)
            var controller = GetController(mock);
            var result = await controller.GetAsync(CancellationToken.None);
            #endregion

            #region Assert (Verificar)
            var ok = Assert.IsType<OkObjectResult>(result);
            var value = Assert.IsAssignableFrom<IEnumerable<TModel>>(ok.Value);
            Assert.Equal(list, value);
            #endregion
        }

        [Fact]
        [Trait("RestApiVerb", "GET")]
        [Trait("Operation", "GetAll")]
        public async void GetAllValuesButReturningAsNoContent()
        {
            #region  Arrange (Preparar)
            var list = new List<TModel>();
            var mock = new Mock<TService>();
            mock.Setup(s => s.ListAsync(CancellationToken.None)).Returns(Task.FromResult(list));
            #endregion

            #region Act (Agir)
            var controller = GetController(mock);
            var result = await controller.GetAsync(CancellationToken.None);
            #endregion

            #region Assert (Verificar)
            Assert.IsType<NoContentResult>(result);
            #endregion
        }

        [Fact]
        [Trait("RestApiVerb", "GET")]
        [Trait("Operation", "GetAll")]
        [Trait("Operation", "Error")]
        public async void GetAllValuesButReturningAsBadRequest()
        {
            #region  Arrange (Preparar)
            var ex = new Exception("Simulated Error!");
            var mock = new Mock<TService>();
            mock.Setup(s => s.ListAsync(CancellationToken.None)).Returns(() => throw ex);
            #endregion

            #region Act (Agir)
            var controller = GetController(mock);
            var result = await controller.GetAsync(CancellationToken.None);
            #endregion

            #region Assert (Verificar)
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(ex.Message, badRequest.Value);
            #endregion
        }
        #endregion

        #region Get By Id
        [Fact]
        [Trait("RestApiVerb", "GET")]
        [Trait("Operation", "GetById")]
        public async void GetById()
        {
            #region  Arrange (Preparar)
            var item = this.MockValues().First();
            var mock = new Mock<TService>();
            mock.Setup(s => s.GetAsync(item.Id, CancellationToken.None)).Returns(Task.FromResult(item));
            #endregion

            #region Act (Agir)
            var controller = GetController(mock);
            var result = await controller.GetAsync(item.Id);
            #endregion

            #region Assert (Verificar)
            var ok = Assert.IsType<OkObjectResult>(result);
            var value = Assert.IsAssignableFrom<TModel>(ok.Value);
            Assert.Equal(item, value);
            #endregion
        }

        [Fact]
        [Trait("RestApiVerb", "GET")]
        [Trait("Operation", "GetById")]
        public async void GetByIdButReturningAsNotFound()
        {
            #region  Arrange (Preparar)            
            var mock = new Mock<TService>();
            mock.Setup(s => s.GetAsync(long.MaxValue, CancellationToken.None)).Returns(Task.FromResult(default(TModel)));
            #endregion

            #region Act (Agir)
            var controller = GetController(mock);
            var result = await controller.GetAsync(long.MaxValue);
            #endregion

            #region Assert (Verificar)
            Assert.IsType<NotFoundResult>(result);
            #endregion
        }

        [Theory]
        [Trait("RestApiVerb", "GET")]
        [Trait("Operation", "GetById")]
        [Trait("Operation", "Error")]
        [InlineData(long.MinValue)]
        [InlineData(-1L)]
        [InlineData(0L)]
        public async void GetByIdButReturningAsBadRequest(long id)
        {
            #region  Arrange (Preparar)            
            var mock = new Mock<TService>();
            mock.Setup(s => s.GetAsync(id, CancellationToken.None)).Returns(Task.FromResult(default(TModel)));
            #endregion

            #region Act (Agir)
            var controller = GetController(mock);
            var result = await controller.GetAsync(id);
            #endregion

            #region Assert (Verificar)
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            var error = Assert.IsType<string>(badRequest.Value);
            Assert.NotEmpty(error);
            #endregion
        }
        #endregion

        #region Get By Uuid
        [Fact]
        [Trait("RestApiVerb", "GET")]
        [Trait("Operation", "GetByUuid")]
        public async void GetByUuid()
        {
            #region  Arrange (Preparar)
            var item = this.MockValues().First();
            var mock = new Mock<TService>();
            mock.Setup(s => s.GetAsync(item.Uuid, CancellationToken.None)).Returns(Task.FromResult(item));
            #endregion

            #region Act (Agir)
            var controller = GetController(mock);
            var result = await controller.GetAsync(item.Uuid);
            #endregion

            #region Assert (Verificar)
            var ok = Assert.IsType<OkObjectResult>(result);
            var value = Assert.IsAssignableFrom<TModel>(ok.Value);
            Assert.Equal(item, value);
            #endregion
        }

        [Fact]
        [Trait("RestApiVerb", "GET")]
        [Trait("Operation", "GetByUuid")]
        public async void GetByUuidButReturningAsNotFound()
        {
            #region  Arrange (Preparar)  
            var uuid = Guid.NewGuid();
            var mock = new Mock<TService>();
            mock.Setup(s => s.GetAsync(uuid, CancellationToken.None)).Returns(Task.FromResult(default(TModel)));
            #endregion

            #region Act (Agir)
            var controller = GetController(mock);
            var result = await controller.GetAsync(uuid);
            #endregion

            #region Assert (Verificar)
            Assert.IsType<NotFoundResult>(result);
            #endregion
        }
        #endregion

        #region Pagging
        [Theory]
        [Trait("RestApiVerb", "GET")]
        [Trait("Operation", "Paging")]
        [InlineData(0, 3)]
        [InlineData(1, 3)]
        [InlineData(3, 3)]
        public async void GetValuesAsPaging(int page, int limit)
        {
            #region  Arrange (Preparar)
            var item = this.MockValues().Skip(page * limit).Take(limit).ToList();
            var mock = new Mock<TService>();
            mock.Setup(s => s.PagingAsync(page, limit, CancellationToken.None)).Returns(Task.FromResult(item));
            #endregion

            #region Act (Agir)
            var controller = GetController(mock);
            var result = await controller.PagingAsync(page, limit, CancellationToken.None);
            #endregion

            #region Assert (Verificar)
            var ok = Assert.IsType<OkObjectResult>(result);
            var value = Assert.IsAssignableFrom<IEnumerable<TModel>>(ok.Value);
            Assert.Equal(item, value);
            #endregion
        }
        #endregion

        #endregion
    }
}
