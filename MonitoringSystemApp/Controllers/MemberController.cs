using Application.DTOs.RequestDtos;
using Application.DTOs.ResponseDtos;
using Application.Interfaces.Services;
using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace MonitoringSystemApp.Controllers
{
    [ApiController]
    [Route("member")]
    public class MemberController : ControllerBase
    {
        private readonly IMemberService _memberService;
        public MemberController(IMemberService memberService) { 
            _memberService = memberService;        
        }


        [HttpGet("search")]
        public async Task<ActionResult<PagedResponseDto<MemberResponseDto>>> Search([FromQuery] MemberFilterDto request)
        {
            var result = await _memberService.SearchAsync(request);

            return Ok(result);
        }


        [HttpGet("Active")]
        public async Task<IActionResult> GetAllActive(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var result = await _memberService.GetAllAsync(
                pageNumber, pageSize, true);

            return Ok(result);
        }
        [HttpGet("inactive")]
        public async Task<IActionResult> GetAllInActive(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10
            )
        {
            var result = await _memberService.GetAllAsync(
                pageNumber, pageSize, false);

            return Ok(result);
        }

        
        [HttpDelete("{id}")]
        public async Task<IActionResult> SoftDelete(int id)
        {
            try {
                await _memberService.SetActiveStatusAsync(id,false);
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
                await _memberService.SetActiveStatusAsync(id, true);
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
                await _memberService.PermanentDeleteAsync(id);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            return NoContent();
        }




    }
}
