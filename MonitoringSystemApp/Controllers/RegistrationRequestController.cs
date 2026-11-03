using Application.DTOs.ResponseDtos;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace MonitoringSystemApp.Controllers
{
    [ApiController]
    [Route(" registrationRequest")]
    public class RegistrationRequestController : Controller
    {
        private readonly IRegistrationRequestService _RegistrationRequestService;

        public RegistrationRequestController(IRegistrationRequestService registrationRequestService)
        {
            _RegistrationRequestService = registrationRequestService;
        }

        [HttpGet("")]

        public async Task<ActionResult<PagedResponse<RegistrationResponseDto>>> GetAllRegistrationRequests(
          [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _RegistrationRequestService.GetAllAsync(pageNumber, pageSize);
            return Ok(result);
        }


        [HttpPatch("reject/{id}")]
        public async Task<IActionResult> RejectRegistrationRequest(int id)
        {
            await _RegistrationRequestService.MarkStatus(id, false);
            return NoContent();
        }

        [HttpPatch("approve/{id}")]
        public async Task<IActionResult> ApproveRegistrationRequest(int id)
        {
            await _RegistrationRequestService.MarkStatus(id, true);
            return NoContent();
        }

    }
}
