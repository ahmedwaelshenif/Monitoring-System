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

        [HttpGet("")]

        public async Task<ActionResult<PagedResponse<MemberResponse>>> GetAllMembers(
           [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _memberService.GetAllAsync(pageNumber, pageSize);
            return Ok(result);
        }

        [HttpGet("search")]
        public async Task<ActionResult<PagedResponse<DashboardAccountResponse>>> Search([FromQuery] SearchMemberRequest request)
        {
            var result = await _memberService.SearchAsync(request);

            return Ok(result);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMember(int id)
        {
            await _memberService.DeleteAsync(id);
            return NoContent();
        }

    }
}
