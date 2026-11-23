using Application.DTOs.RequestDtos;
using Application.DTOs.ResponseDtos;
using Application.Interfaces.Services;
using Application.Services;
using Domain.Enums;
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

        [HttpPost("")]
        public async Task<IActionResult> CreateEntryLog(
             [FromBody] EntryLogRequestDto request)
        {
            try
            {
                await _entryLogService.CreateAsync(request);
                return Ok("Entry log created successfully.");
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while creating the entry log.");
            }
        }




        [HttpGet("")]

        public async Task<ActionResult<PagedResponseDto<EntryLogResponseDto>>> GetAllEntryLogs(
           [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _entryLogService.GetAllAsync(pageNumber, pageSize);
            return Ok(result);
        }


        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var result = await _entryLogService.GetByIdAsync(id);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteEntryLog(int id)
        {
            try
            {
                await _entryLogService.DeleteAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("count/accepted")]
        public async Task<IActionResult> GetAcceptedCount(DateOnly date)
        {
            return Ok(await _entryLogService.CountStatus(true,date));
        }

        [HttpGet("count/rejected")]
        public async Task<IActionResult> GetRejectedCount(DateOnly date)
        {
            return Ok(await _entryLogService.CountStatus(false,date));
        }

        [HttpGet("ByLab")]
        public async Task<IActionResult> GetLogsByLab([FromQuery]int LabId, [FromQuery] int PageNumber=1, [FromQuery] int PageSize=10 )
        {
            return Ok(await _entryLogService.GetByLabAsync(LabId,PageNumber,PageSize));
        }

        [HttpGet("date-range")]
        public async Task<IActionResult> GetByDateRange(
                [FromQuery] DateTime fromDate,
                [FromQuery] DateTime toDate,
                [FromQuery] int pageNumber = 1,
                [FromQuery] int pageSize = 10,
                [FromQuery] EntryStatus? status = null)
        {
            var result = await _entryLogService.GetByDateRangeAsync(
                fromDate,
                toDate,
                pageNumber,
                pageSize,
                status);

            return Ok(result);
        }
    }
}
