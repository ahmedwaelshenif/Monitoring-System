using Application.DTOs.RequestDtos;
using Application.DTOs.ResponseDtos;
using Application.Interfaces.Services;
using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace MonitoringSystemApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DashboardAccountController : ControllerBase
    {
        private readonly IDashboardAccountService _DashboardAccountService;
        public DashboardAccountController(IDashboardAccountService dashboardAccountService)
        {
            _DashboardAccountService = dashboardAccountService;
        }


        [HttpGet("")]

        public async Task<ActionResult<PagedResponseDto<DashboardAccountResponseDto>>> GetAllDashboardAccounts(
           [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _DashboardAccountService.GetAllAsync(pageNumber, pageSize);
            return Ok(result);
        }

        [HttpGet ("search")]
        public async Task<ActionResult<PagedResponseDto<DashboardAccountResponseDto>>> Search([FromQuery] AccountFilterDto request)
        {
            var result = await _DashboardAccountService.SearchAsync(request);

            return Ok(result);
        }


        [HttpPost("create")]
        public async Task<IActionResult> CreateAccount(DashboardAccountRequestDto acc)
        {
            try
            {
                await _DashboardAccountService.CreateAsync(acc);
            }
            catch (Exception ex) { 
                return BadRequest(new { message = ex.Message });
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAccount(int id)
        {
            try
            {
                await _DashboardAccountService.DeleteAsync(id);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            return NoContent();
        }

        [HttpPatch("deactivate/{id}")]
        public async Task<IActionResult> DeactivateAccount(int id)
        {

            try
            {
                await _DashboardAccountService.SetActiveStatusAsync(id,false);
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

        [HttpPatch("activate/{id}")]
        public async Task<IActionResult> ActivateAccount(int id)
        {

            try
            {
                await _DashboardAccountService.SetActiveStatusAsync(id, true);
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

        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateAccount(int id, [FromBody] UpdateDashboardAccountDto dto)
        {

            try
            {
                await _DashboardAccountService.UpdateAsync(id, dto);
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
