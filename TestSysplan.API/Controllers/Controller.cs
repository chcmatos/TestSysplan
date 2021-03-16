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
        public IActionResult Create([FromBody] Model client)
        {
            try
            {
                return new JsonResult(service.Insert(client));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion

        #region [R]ead
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                var clients = service.List();

                if (clients.Count == 0)
                {
                    return NoContent();
                }

                return new JsonResult(clients);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public IActionResult Get(long id)
        {
            try
            {
                var client = service.Get(id);

                if (client == null)
                {
                    return NotFound();
                }

                return new JsonResult(client);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("uuid/{uuid}")]
        public IActionResult Get(Guid uuid)
        {
            try
            {
                var client = service.Get(uuid);
                if (client == null)
                {
                    return NotFound();
                }
                return new JsonResult(client);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("page/{page}/{limit:int?}")]
        public IActionResult Paging(int page, int limit = -1)
        {
            try
            {
                var client = service.Paging(page, limit);

                if (client.Count == 0)
                {
                    return NotFound();
                }

                return new JsonResult(client);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion

        #region [U]pdate
        [HttpPut]
        public IActionResult Update([FromBody] Model client)
        {
            try
            {
                if (service.Exists(client))
                {
                    service.Update(client);
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
