using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using TestSysplan.Core.Infrastructure.Logger;
using TestSysplan.Core.Infrastructure.Services;
using TestSysplan.Core.Model;

namespace TestSysplan.API.Controllers
{
    public abstract class Controller<Model, Service> : ControllerBase<Model, Service>
        where Model : ModelBase
        where Service : IServiceBase<Model>
    {
        protected Controller(Service service, ILogger<ControllerBase<Model, Service>> logger) : base(service, logger) { }

        protected Controller(Service service) : base(service) { }

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
                if(id <= 0)
                {
                    throw new ArgumentException("Invalid id!");
                }

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
                if(uuid == default)
                {
                    throw new ArgumentException("Invalid uuid!");
                }

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
                if(uuid == default)
                {
                    throw new ArgumentException("Invalid uuid!");
                }
                else if (!service.Exists(uuid))
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
