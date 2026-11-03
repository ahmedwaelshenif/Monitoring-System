using Application.DTOs.RequestDtos;
using Application.DTOs.ResponseDtos;
using Application.Interfaces.Services;
using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace MonitoringSystemApp.Controllers
{
    [ApiController]
    [Route("entryLog")]
    public class EntryLogController : ControllerBase
    {
        private readonly IEntryLogService _entryLogService;

        public EntryLogController(IEntryLogService entryLogService) { 
            _entryLogService = entryLogService;
        }

        [HttpGet("")]

        public async Task<ActionResult<PagedResponse<EntryLogResponse>>> GetAllDashboardAccounts(
           [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _entryLogService.GetAllAsync(pageNumber, pageSize);
            return Ok(result);
        }


        [HttpGet("count/accepted/today")]
        public async Task<IActionResult> GetAcceptedCount(DateOnly date)
        {
            return Ok(await _entryLogService.CountStatus(true,date));
        }

        [HttpGet("count/rejected/today")]
        public async Task<IActionResult> GetRejectedCount(DateOnly date)
        {
            return Ok(await _entryLogService.CountStatus(false,date));
        }

        [HttpGet("ByLab")]
        public async Task<IActionResult> GetLogsByLab([FromQuery]int LabId, [FromQuery] int PageNumber, [FromQuery] int PageSize )
        {
            return Ok(await _entryLogService.GetByLabAsync(LabId,PageNumber,PageSize));
        }
    }
}
