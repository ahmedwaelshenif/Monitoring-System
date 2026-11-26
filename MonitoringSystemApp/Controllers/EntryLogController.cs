using Application.DTOs.RequestDtos;
using Application.DTOs.ResponseDtos;
using Application.Interfaces.Services;
using Application.Services;
using Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace MonitoringSystemApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
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
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }




        [HttpGet("")]

        public async Task<ActionResult<PagedResponseDto<EntryLogResponseDto>>> GetAllEntryLogs(
           [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _entryLogService.GetAllAsync(pageNumber, pageSize);
            return Ok(result);
        }

        [HttpGet("search")]
        public async Task<ActionResult<PagedResponseDto<EntryLogResponseDto>>> Search(
            [FromQuery] EntryLogFilterDto request)
        {
            var result = await _entryLogService.SearchAsync(request);
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
                return NotFound(new { message = ex.Message });
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
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpDelete("Range")]
        public async Task<IActionResult> DeleteEntryLogsWithinDateRange([FromQuery] EntryLogDeleteDto dto)
        {
            try
            {
                await _entryLogService.DeleteLogsByDateRange(dto);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("Oldest")]
        public async Task<IActionResult> DeleteOldestEntryLogs([FromQuery] int cnt)
        {
            try
            {
                await _entryLogService.DeleteOldestLogs(cnt);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
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

        [HttpGet("export/excel")]
        public async Task<IActionResult> ExportAttendanceExcel(
                                [FromQuery] DateTime fromDate,
                                [FromQuery] DateTime toDate,
                                [FromQuery] int? labId = null)
        {
            try
            {
                var fileBytes = await _entryLogService.ExportAttendanceToExcelAsync(fromDate, toDate, labId);
                string fileName = $"Attendance_{fromDate:yyyyMMdd_HHmm}_to_{toDate:yyyyMMdd_HHmm}.xlsx";
                string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                return File(fileBytes, contentType, fileName);
            }
            catch(ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("rotation-config")]
        public async Task<ActionResult<LogRotationConfigDto>> GetRotationConfig()
        {
            return Ok(await _entryLogService.GetRotationConfigAsync());
        }
        [HttpPatch("rotation-config")]
        public async Task<IActionResult> UpdateRotationConfig([FromBody] LogRotationConfigDto dto)
        {
            await _entryLogService.UpdateRotationConfigAsync(dto);
            return NoContent();
        }
    }
}
