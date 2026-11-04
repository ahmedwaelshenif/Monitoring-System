using Application.DTOs.RequestDtos;
using Application.DTOs.ResponseDtos;
using Application.Interfaces.Services;
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


        [HttpGet("")]
        public async Task<ActionResult<PagedResponse<LabResponse>>> GetAllLabs(
           [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _labService.GetAllAsync(pageNumber, pageSize);
            return Ok(result);
        }



        [HttpPost("create")]
        public async Task<IActionResult> CreateLab(LabRequest lab)
        {
            await _labService.CreateAsync(lab);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLab(int id)
        {
            await _labService.DeleteAsync(id);
            return NoContent();
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateLab(int id, [FromBody] UpdateLab dto)
        {
            var result = await _labService.UpdateAsync(id, dto);

            if (!result)
                return NotFound("Lab not found.");

            return NoContent();
        }

        [HttpGet ("count")]
        public async Task<IActionResult> Count()
        {
            int cnt = await _labService.CountAsync();
            return Ok(cnt);
        }
    }
}
