using Application.DTOs.RequestDtos;
using Application.DTOs.ResponseDtos;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Domain.Enums;

namespace MonitoringSystemApp.Controllers
{
    [ApiController]
    [Route("warning")]
    public class WarningController : ControllerBase
    {
        private readonly IWarningService _warningService;
        public WarningController(IWarningService warningService)
        {
            _warningService = warningService;
           
        }


        [HttpGet("")]
        public async Task<ActionResult<PagedResponse<WarningResponse>>> GetAllWarnings(
           [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _warningService.GetAllAsync(pageNumber, pageSize);
            return Ok(result);
        }


        [HttpPost("create")]
        public async Task<IActionResult> CreateWarning(WarningRequest req)
        {
            await _warningService.CreateAsync(req);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWarning(int id)
        {
            await _warningService.DeleteAsync(id);
            return NoContent();
        }

        [HttpGet("{id}")]

        public async Task<ActionResult<WarningResponse>> GetByID(
                             int id)
        {
            var result = await _warningService.GetByIdAsync(id);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        [HttpGet("alert/{warning}")]

        public async Task<ActionResult<PagedResponse<WarningResponse>>> GetAllByWarning(
                             Alert warning, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _warningService.GetWarningsByAlertAsync(warning, pageNumber,pageSize);
            return Ok(result);
        }

        [HttpGet("lab/{labid}")]

        public async Task<ActionResult<PagedResponse<WarningResponse>>> GetAllByLab(
                             int labid, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _warningService.GetWarningsByLabAsync(labid, pageNumber,pageSize);
            return Ok(result);
        }


    }
}
