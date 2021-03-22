using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using TestSysplan.Core.Infrastructure.Logger;
using TestSysplan.Core.Infrastructure.Services;
using TestSysplan.Core.Model;

namespace TestSysplan.API.Controllers
{
    public abstract class ControllerAsync<Model, Service> : ControllerBase<Model, Service>
        where Model : ModelBase
        where Service : IServiceBaseAsync<Model>
    {
        protected ControllerAsync(Service service, ILogger<ControllerBase<Model, Service>> logger) : base(service, logger) { }

        protected ControllerAsync(Service service) : base(service) { }

        #region [C]reate
        [HttpPost]
        public virtual async Task<IActionResult> CreateAsync([FromBody] Model result)
        {
            try
            {
                if (await service.ExistsAsync(result))
                {
                    throw new InvalidOperationException("Already exists a register with this UUID!");
                }

                return Ok(await service.InsertAsync(result));
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
        public virtual async Task<IActionResult> GetAsync(CancellationToken cancellationToken)
        {
            try
            {
                var result = await service.ListAsync(cancellationToken);

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
        public virtual async Task<IActionResult> GetAsync(long id)
        {
            try
            {
                if (id <= 0)
                {
                    throw new ArgumentException("Invalid id!");
                }

                var result = await service.GetAsync(id);

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
        public virtual async Task<IActionResult> GetAsync(Guid uuid)
        {
            try
            {
                if (uuid == default)
                {
                    throw new ArgumentException("Invalid uuid!");
                }

                var result = await service.GetAsync(uuid);
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
        public virtual async Task<IActionResult> PagingAsync(int page, int limit = -1, CancellationToken cancellationToken = default)
        {
            try
            {
                var result = await service.PagingAsync(page, limit, cancellationToken);
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
        public async Task<IActionResult> UpdateAsync([FromBody] Model result)
        {
            try
            {
                if (await service.ExistsAsync(result))
                {
                    await service.UpdateAsync(result);
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
        public async Task<IActionResult> DeleteAsync(Guid uuid)
        {
            try
            {
                if (uuid == default)
                {
                    throw new ArgumentException("Invalid uuid!");
                }
                else if (!await service.ExistsAsync(uuid))
                {
                    logger.LogD("Uuid {0} NotFound", args: uuid);
                    return NotFound();
                }
                else if (await service.DeleteAsync(uuid))
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
