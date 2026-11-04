using Application.DTOs.RequestDtos;
using Application.DTOs.ResponseDtos;
using Application.Interfaces.Services;
using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace MonitoringSystemApp.Controllers
{
    [ApiController]
    [Route("dashboardAccount")]
    public class DashboardAccountController : ControllerBase
    {
        private readonly IDashboardAccountService _DashboardAccountService;
        public DashboardAccountController(IDashboardAccountService dashboardAccountService)
        {
            _DashboardAccountService = dashboardAccountService;
        }


        [HttpGet("")]

        public async Task<ActionResult<PagedResponse<DashboardAccountResponse>>> GetAllDashboardAccounts(
           [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _DashboardAccountService.GetAllAsync(pageNumber, pageSize);
            return Ok(result);
        }

        [HttpGet ("search")]
        public async Task<ActionResult<PagedResponse<DashboardAccountResponse>>> Search([FromQuery] SearchAccount request)
        {
            var result = await _DashboardAccountService.SearchAsync(request);

            return Ok(result);
        }


        [HttpPost("create")]
        public async Task<IActionResult> CreateAccount(DashboardAccountRequest acc)
        {
            await _DashboardAccountService.CreateAsync(acc);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAccount(int id)
        {
            await _DashboardAccountService.DeleteAsync(id);
            return NoContent();
        }

        [HttpPatch("deactivate/{id}")]
        public async Task<IActionResult> DeactivateAccount(int id)
        {
            await _DashboardAccountService.Deactivate(id);
            return NoContent();
        }

        [HttpPatch("activate/{id}")]
        public async Task<IActionResult> ActivateAccount(int id)
        {
            await _DashboardAccountService.Activate(id);
            return NoContent();
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateAccount(int id, [FromBody] UpdateDashboardAccount dto)
        {
            var result = await _DashboardAccountService.UpdateAsync(id, dto);

            if (!result)
                return NotFound("Lab not found.");

            return NoContent();
        }

        


    }
}
