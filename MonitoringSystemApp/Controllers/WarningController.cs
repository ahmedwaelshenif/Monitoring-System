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
        public async Task<ActionResult<PagedResponseDto<WarningResponseDto>>> GetAllWarnings(
           [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _warningService.GetAllAsync(pageNumber, pageSize);
            return Ok(result);
        }


        [HttpPost("create")]
        public async Task<IActionResult> CreateWarning(WarningRequestDto req)
        {
            await _warningService.CreateAsync(req);
            return NoContent();
        }


        [HttpPost("search")]
        public async Task<IActionResult> Search(WarningFilterDto filter)
        {
            var res=await _warningService.SearchAsync(filter);
            return Ok(res);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWarning(int id)
        {
            try
            {
                await _warningService.DeleteAsync(id);
            }
            catch(KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            return NoContent();
        }

    

        [HttpGet("alert/{warning}")]

        public async Task<ActionResult<PagedResponseDto<WarningResponseDto>>> GetAllByWarning(
                         [FromRoute] Alert warning, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            if (!Enum.IsDefined(typeof(Alert), warning))
                return BadRequest("Invalid alert type.");
            var result = await _warningService.GetWarningsByAlertAsync(warning, pageNumber,pageSize);
            return Ok(result);
        }


        [HttpGet("lab/{labid}")]

        public async Task<ActionResult<PagedResponseDto<WarningResponseDto>>> GetAllByLab(
                           [FromRoute]  int labid, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _warningService.GetWarningsByLabIdAsync(labid, pageNumber,pageSize);
            return Ok(result);
        }

        [HttpGet("lab/name/{labName}")]
        public async Task<ActionResult<PagedResponseDto<WarningResponseDto>>> GetWarningsByLabName(
            [FromRoute] string labName,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            if (string.IsNullOrWhiteSpace(labName))
                return BadRequest("Lab name cannot be empty.");

            var result = await _warningService.GetWarningsByLabNameAsync(
                labName, pageNumber, pageSize);

            return Ok(result);
        }


    }
}
