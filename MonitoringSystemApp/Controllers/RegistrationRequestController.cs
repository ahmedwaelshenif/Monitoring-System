using Application.DTOs.RequestDtos;
using Application.DTOs.ResponseDtos;
using Application.Interfaces.Services;
using Application.Services;
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
        [HttpGet("{Id:int}")]

        public async Task<ActionResult<RegistrationResponseDto>> GetbyId(int id)
        {
            
            try
            {
              var result = await _RegistrationRequestService.GetByIdAsync(id);
              return Ok(result);

            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpGet("search")]
        public async Task<ActionResult<PagedResponse<RegistrationResponseDto>>> Search([FromQuery] SearchRequest request)
        {
            var result = await _RegistrationRequestService.SearchAsync(request);

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
            try
            {
                await _RegistrationRequestService.MarkStatus(id, false);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            return NoContent();
        }

        [HttpPatch("approve/{id}")]
        public async Task<IActionResult> ApproveRegistrationRequest(int id)
        {
            try
            {
                await _RegistrationRequestService.MarkStatus(id, true);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            return NoContent();
        }

    }
}
