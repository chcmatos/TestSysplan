using Microsoft.AspNetCore.Mvc;
using System;
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
        private readonly Service service;

        public Controller(Service service)
        {
            this.service = service ?? throw new ArgumentNullException(nameof(service));
        }

        #region [C]reate
        [HttpPost]
        public IActionResult Create([FromBody] Model result)
        {
            try
            {
                return new JsonResult(service.Insert(result));
            }
            catch (Exception ex)
            {
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
                    return NoContent();
                }

                return new JsonResult(result);
            }
            catch (Exception ex)
            {
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
                    return NotFound();
                }

                return new JsonResult(result);
            }
            catch (Exception ex)
            {
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
                    return NotFound();
                }
                return new JsonResult(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("page/{page}/{limit:int?}")]
        public virtual IActionResult Paging(int page, int limit = -1)
        {
            try
            {
                var result = service.Paging(page, limit);

                if (result.Count == 0)
                {
                    return NotFound();
                }

                return new JsonResult(result);
            }
            catch (Exception ex)
            {
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
                    return NotFound();
                }
                else if (service.Delete(uuid))
                {
                    return Ok();
                }

                return BadRequest("Was not possible to remove value!");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion
    }
}
