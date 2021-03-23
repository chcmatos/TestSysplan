using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
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
        #region [R]ead
        
        #region Get All
        [Fact]
        [Trait("RestApiVerb", "GET")]
        [Trait("Operation", "GetAll")]
        public void GetAllValues()
        {
            #region  Arrange (Preparar)
            var list = this.MockValues().ToList();
            var mock = new Mock<TService>();
            mock.Setup(s => s.List()).Returns(list);
            #endregion

            #region Act (Agir)
            var controller = GetController(mock);
            var result = controller.Get();
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
        public void GetAllValuesButReturningAsNoContent()
        {
            #region  Arrange (Preparar)
            var list = new List<TModel>();
            var mock = new Mock<TService>();
            mock.Setup(s => s.List()).Returns(list);
            #endregion

            #region Act (Agir)
            var controller = GetController(mock);
            var result = controller.Get();
            #endregion

            #region Assert (Verificar)
            Assert.IsType<NoContentResult>(result);
            #endregion
        }

        [Fact]
        [Trait("RestApiVerb", "GET")]
        [Trait("Operation", "GetAll")]
        [Trait("Operation", "Error")]
        public void GetAllValuesButReturningAsBadRequest()
        {
            #region  Arrange (Preparar)
            var ex = new Exception("Simulated Error!");
            var mock = new Mock<TService>();
            mock.Setup(s => s.List()).Returns(() => throw ex);
            #endregion

            #region Act (Agir)
            var controller = GetController(mock);
            var result = controller.Get();
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
        public void GetById()
        {
            #region  Arrange (Preparar)
            var item = this.MockValues().First();
            var mock = new Mock<TService>();
            mock.Setup(s => s.Get(item.Id)).Returns(item);
            #endregion

            #region Act (Agir)
            var controller = GetController(mock);
            var result = controller.Get(item.Id);
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
        public void GetByIdButReturningAsNotFound()
        {
            #region  Arrange (Preparar)            
            var mock = new Mock<TService>();
            mock.Setup(s => s.Get(long.MaxValue)).Returns(default(TModel));
            #endregion

            #region Act (Agir)
            var controller = GetController(mock);
            var result = controller.Get(long.MaxValue);
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
        public void GetByIdButReturningAsBadRequest(long id)
        {
            #region  Arrange (Preparar)            
            var mock = new Mock<TService>();
            mock.Setup(s => s.Get(id)).Returns(default(TModel));
            #endregion

            #region Act (Agir)
            var controller = GetController(mock);
            var result = controller.Get(id);
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
        public void GetByUuid()
        {
            #region  Arrange (Preparar)
            var item = this.MockValues().First();
            var mock = new Mock<TService>();
            mock.Setup(s => s.Get(item.Uuid)).Returns(item);
            #endregion

            #region Act (Agir)
            var controller = GetController(mock);
            var result = controller.Get(item.Uuid);
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
        public void GetByUuidButReturningAsNotFound()
        {
            #region  Arrange (Preparar)  
            var uuid = Guid.NewGuid();
            var mock = new Mock<TService>();
            mock.Setup(s => s.Get(uuid)).Returns(default(TModel));
            #endregion

            #region Act (Agir)
            var controller = GetController(mock);
            var result = controller.Get(uuid);
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
        public void GetValuesAsPaging(int page, int limit)
        {
            #region  Arrange (Preparar)
            var item = this.MockValues().Skip(page * limit).Take(limit).ToList();
            var mock = new Mock<TService>();
            mock.Setup(s => s.Paging(page, limit)).Returns(item);
            #endregion

            #region Act (Agir)
            var controller = GetController(mock);
            var result = controller.Paging(page, limit);
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
