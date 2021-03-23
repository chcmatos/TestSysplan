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
        #region [C]reate
        [Fact]
        [Trait("RestApiVerb", "POST")]
        [Trait("Operation", "Insert")]
        public void InsertANewValue()
        {
            #region  Arrange (Preparar)
            var item  = this.MockValues().First();
            item.Id   = default;
            item.Uuid = default;
            var mock  = new Mock<TService>();
            mock.Setup(s => s.Exists(item)).Returns(false);
            mock.Setup(s => s.Insert(item)).Returns<TModel>((t) => 
            {
                t.Id   = new Random().Next();
                t.Uuid = Guid.NewGuid();
                return t;
            });            
            #endregion

            #region Act (Agir)
            var controller = GetController(mock);
            var result = controller.Create(item);
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
        public void InsertANewValueButReturningAsExistingWithBadRequest()
        {
            #region  Arrange (Preparar)
            var item = this.MockValues().First();            
            var mock = new Mock<TService>();
            mock.Setup(s => s.Exists(item)).Returns(true);            
            #endregion

            #region Act (Agir)
            var controller = GetController(mock);
            var result = controller.Create(item);
            #endregion

            #region Assert (Verificar)
            var badRequest  = Assert.IsType<BadRequestObjectResult>(result);
            string error    = Assert.IsType<string>(badRequest.Value);
            Assert.NotEmpty(error);
            Assert.NotEqual(default, item.Id);
            Assert.NotEqual(default, item.Uuid);
            #endregion
        }
        #endregion
    }
}
