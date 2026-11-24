using Application.DTOs.RequestDtos;
using Application.DTOs.ResponseDtos;
using Application.Interfaces.Services;
using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace MonitoringSystemApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RegistrationRequestController : ControllerBase
    {
        private readonly IRegistrationRequestService _RegistrationRequestService;

        public RegistrationRequestController(IRegistrationRequestService registrationRequestService)
        {
            _RegistrationRequestService = registrationRequestService;
        }

        [HttpGet("")]

        public async Task<ActionResult<PagedResponseDto<RegistrationResponseDto>>> GetAllRegistrationRequests(
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
        public async Task<ActionResult<PagedResponseDto<RegistrationResponseDto>>> Search([FromQuery] RegistrationRequestFilterDto request)
        {
            var result = await _RegistrationRequestService.SearchAsync(request);

            return Ok(result);
        }


        [HttpPost("")]
        public async Task<ActionResult> Add([FromForm] RegistrationRequestDto request)
        {
            try
            {
                await _RegistrationRequestService.CreateAsync(request);
                return Ok();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }

            
        }


        [HttpPatch("reject/{id}")]
        public async Task<IActionResult> RejectRegistrationRequest(int id)
        {
            try
            {
                await _RegistrationRequestService.RejectAsync(id);
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
        public async Task<IActionResult> ApproveRegistrationRequest(int id, [FromBody] List<int>LabIds)
        {
            try
            {
                await _RegistrationRequestService.AcceptAsync(id, LabIds);
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
