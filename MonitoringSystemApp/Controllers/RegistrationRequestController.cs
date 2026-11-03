using Application.DTOs.RequestDtos;
using Application.DTOs.ResponseDtos;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace MonitoringSystemApp.Controllers
{
    [ApiController]
    [Route("registrationRequest")]
    public class RegistrationRequestController : ControllerBase
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


        [HttpPost("")]
        public async Task<ActionResult> Add([FromForm] RegistrationRequestDto request)
        {
            bool result = await _RegistrationRequestService.CreateAsync(request);
            if (!result)
                return BadRequest("You already have an accepted registration request.");

            return StatusCode(StatusCodes.Status201Created);
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
