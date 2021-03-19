using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using TestSysplan.Core.Infrastructure.Logger;
using TestSysplan.Core.Infrastructure.Services;
using TestSysplan.Core.Model;

namespace TestSysplan.API.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public abstract class Controller<Model, Service> : ControllerBase 
        where Model : ModelBase
        where Service : IServiceBase<Model>
    {
        private class EmptyLogger : ILogger<Controller<Model, Service>>, IDisposable
        {
            public IDisposable BeginScope<TState>(TState state)
            {
                return this;
            }

            public bool IsEnabled(LogLevel logLevel)
            {
                return false;
            }

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter) { }

            void IDisposable.Dispose() { }
        }

        protected readonly Service service;
        protected readonly ILogger logger;

        protected Controller(Service service, ILogger<Controller<Model, Service>> logger)
        {
            this.service    = service ?? throw new ArgumentNullException(nameof(service));
            this.logger     = logger;
        }

        protected Controller(Service service) : this(service, new EmptyLogger()) { }

        #region [C]reate
        [HttpPost]
        public IActionResult Create([FromBody] Model result)
        {
            try
            {
                if(service.Exists(result))
                {
                    throw new InvalidOperationException("Already exists a register with this UUID!");
                }

                return Ok(service.Insert(result));
            }
            catch (Exception ex)
            {
                logger.LogE(ex);
                return BadRequest(ex.Message);
            }
        }
        #endregion

        #region [R]ead
        [HttpGet]
        public virtual IActionResult Get()
        {
            try
            {
                var result = service.List();

                if (result.Count == 0)
                {
                    logger.LogD("No Content");
                    return NoContent();
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogE(ex);
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public virtual IActionResult Get(long id)
        {
            try
            {
                var result = service.Get(id);

                if (result == null)
                {
                    logger.LogD("Id {0} NotFound", args: id);
                    return NotFound();
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogE(ex);
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("uuid/{uuid}")]
        public virtual IActionResult Get(Guid uuid)
        {
            try
            {
                var result = service.Get(uuid);
                if (result == null)
                {
                    logger.LogD("Uuid {0} NotFound", args: uuid);
                    return NotFound();
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogE(ex);
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("page/{page}/{limit:int?}")]
        public virtual IActionResult Paging(int page, int limit = -1)
        {
            try
            {
                var result = service.Paging(page, limit);
                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogE(ex);
                return BadRequest(ex.Message);
            }
        }
        #endregion

        #region [U]pdate
        [HttpPut]
        public IActionResult Update([FromBody] Model result)
        {
            try
            {
                if (service.Exists(result))
                {
                    service.Update(result);
                    return Ok();
                }

                return NotFound();
            }
            catch (Exception ex)
            {
                logger.LogE(ex);
                return BadRequest(ex.Message);
            }
        }
        #endregion

        #region [D]elete
        [HttpDelete("{uuid}")]
        public IActionResult Delete(Guid uuid)
        {
            try
            {
                if (!service.Exists(uuid))
                {
                    logger.LogD("Uuid {0} NotFound", args: uuid);
                    return NotFound();
                }
                else if (service.Delete(uuid))
                {
                    return Ok();
                }

                throw new InvalidOperationException("Was not possible to remove value!");
            }
            catch (Exception ex)
            {
                logger.LogE(ex);
                return BadRequest(ex.Message);
            }
        }
        #endregion
    }
}
