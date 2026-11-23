using Application.DTOs.RequestDtos;
using Application.DTOs.ResponseDtos;
using Application.Interfaces.Services;
using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace MonitoringSystemApp.Controllers
{
    [ApiController]
    [Route("lab")]
    public class LabController : ControllerBase
    {
        private readonly ILabService _labService;
        public LabController(ILabService labService) { 
            _labService = labService;
        }


        [HttpPost("create")]
        public async Task<IActionResult> CreateLab(LabRequestDto lab)
        {
            await _labService.CreateAsync(lab);
            return NoContent();
        }


        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateLab(int id, [FromBody] UpdateLabDto dto)
        {

            try
            {
                var result = await _labService.UpdateAsync(id, dto);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            return NoContent();
        }

        [HttpGet ("count")]
        public async Task<IActionResult> Count()
        {
            int cnt = await _labService.CountAsync();
            return Ok(cnt);
        }


        [HttpGet("search")]
        public async Task<ActionResult<PagedResponseDto<LabResponseDto>>> Search([FromQuery] SearchLabRequestDto request)
        {
            var result = await _labService.SearchAsync(request);

            return Ok(result);
        }



        [HttpGet("Active")]
        public async Task<IActionResult> GetAllActive(
                        [FromQuery] int pageNumber = 1,
                        [FromQuery] int pageSize = 10)
        {
            var result = await _labService.GetAllAsync(
                               pageNumber, pageSize, true);

            return Ok(result);
        }
        [HttpGet("inactive")]
        public async Task<IActionResult> GetAllInActive(
                                [FromQuery] int pageNumber = 1,
                                [FromQuery] int pageSize = 10)
        {
            var result = await _labService.GetAllAsync(
                pageNumber, pageSize, false);

            return Ok(result);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> SoftDelete(int id)
        {
            try
            {
                await _labService.SoftDeleteAsync(id);
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
                await _labService.RestoreAsync(id);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            return NoContent();
        }

        [HttpDelete("{id}/permanent")]
        public async Task<IActionResult> PermanentDelete(int id)
        {
            try
            {
                await _labService.PermanentDeleteAsync(id);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            return NoContent();
        }
    }
}
