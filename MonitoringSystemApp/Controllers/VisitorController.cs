using Application.DTOs.RequestDtos;
using Application.DTOs.ResponseDtos;
using Application.Interfaces.Services;
using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace MonitoringSystemApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class VisitorController : ControllerBase
    {
        private readonly IVisitorService _visitorService;
        public VisitorController(IVisitorService visitorService) {
            _visitorService = visitorService;        
        }


        [HttpGet("search")]
        public async Task<ActionResult<PagedResponseDto<VisitorResponseDto>>> Search([FromQuery] VisitorFilterDto request)
        {
            var result = await _visitorService.SearchAsync(request);

            return Ok(result);
        }


        [HttpGet("Active")]
        public async Task<IActionResult> GetAllActive(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var result = await _visitorService.GetAllAsync(
                pageNumber, pageSize, true);

            return Ok(result);
        }
        [HttpGet("inactive")]
        public async Task<IActionResult> GetAllInActive(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10
            )
        {
            var result = await _visitorService.GetAllAsync(
                pageNumber, pageSize, false);

            return Ok(result);
        }

        
        [HttpDelete("{id}")]
        public async Task<IActionResult> SoftDelete(int id)
        {
            try {
                await _visitorService.SetActiveStatusAsync(id,false);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            return NoContent();
        }

        [HttpPut("{id}/restore")]
        public async Task<IActionResult> Restore(int id)
        {
            try
            {
                await _visitorService.SetActiveStatusAsync(id, true);
            }
            catch(KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            return NoContent();
        }

        // Permanent Delete
        [HttpDelete("{id}/permanent")]
        public async Task<IActionResult> PermanentDelete(int id)
        {
            try
            {
                await _visitorService.PermanentDeleteAsync(id);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            return NoContent();
        }

        [HttpPost("{visitorId}/labs/{labId}")]
        public async Task<IActionResult> AddLab(int visitorId, int labId)
        {
            try
            {
                await _visitorService.AddLabAsync(visitorId, labId);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{visitorId}/labs/{labId}")]
        public async Task<IActionResult> RemoveLab(int visitorId, int labId)
        {
            try
            {
                await _visitorService.RemoveLabAsync(visitorId, labId);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{labId}")]

        public async Task<IActionResult> GetVisitorsbyLab(int labId, [FromQuery] PagedRequestDto pagination )
        {
            var result = await _visitorService.GetVisitorsbyLab(labId, pagination);
            return Ok(result);

        }


    }
}
