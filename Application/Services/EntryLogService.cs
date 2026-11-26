using Application.DTOs.RequestDtos;
using Application.DTOs.ResponseDtos;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.Specifications;
using ClosedXML.Excel;
using Domain.Entities;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class EntryLogService : IEntryLogService
    {
        private readonly IUnitOfWork _unitOfWork;

        public EntryLogService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        public async Task CreateAsync(EntryLogRequestDto request)
        {
            var lab = await _unitOfWork.Repository<Lab>().GetByIdAsync(request.LabId);
            if (lab == null)
                throw new KeyNotFoundException("Lab not found");
            if(!lab.Active)
                throw new InvalidOperationException("Lab isn't active");

            await _unitOfWork.Repository<EntryLog>().AddAsync(ToEntity(request));
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            EntryLog? entity = await _unitOfWork.Repository<EntryLog>().GetByIdAsync(id);
            if (entity == null)
            {
                throw new KeyNotFoundException($"log with id {id} is not found");
            }
            _unitOfWork.Repository<EntryLog>().Delete(entity);
            await _unitOfWork.SaveChangesAsync();

        }

        public async Task<PagedResponseDto<EntryLogResponseDto>> GetAllAsync(int pageNumber, int pageSize)
        {
            return await SearchAsync(new EntryLogFilterDto { PageNumber = pageNumber, PageSize = pageSize });

        }

        public async Task<int> CountStatus(bool status, DateOnly date)
        {
            var startOfDay = date.ToDateTime(TimeOnly.MinValue);
            var endOfDay = startOfDay.AddDays(1);

            var targetStatus = status
                ? EntryStatus.Accepted
                : EntryStatus.Rejected;

            return await _unitOfWork.Repository<EntryLog>().CountAsync(
                e => e.Status == targetStatus &&
                     e.EventTime >= startOfDay &&
                     e.EventTime < endOfDay
            );
        }



        public async Task<EntryLogResponseDto?> GetByIdAsync(int id)
        {
            var log= await _unitOfWork.Repository<EntryLog>().GetByIdAsync(id);
            if (log == null)
            {
                throw new KeyNotFoundException($"log with id {id} is not found");
            }
            return ToDto(log);
        }


        public async Task<PagedResponseDto<EntryLogResponseDto>> GetByLabAsync(int LabId,int pageNumber,int pageSize)
        {
            return await SearchAsync(new EntryLogFilterDto { PageNumber = pageNumber, PageSize = pageSize, LabId=LabId});
        
        }
     

        public async Task<PagedResponseDto<EntryLogResponseDto>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate, int pageNumber, int pageSize,EntryStatus? status= null)
        {
            if (toDate < fromDate)
                throw new ArgumentException("End date can't be earlier than start date.");

            return await SearchAsync(new EntryLogFilterDto {
                            StartDate = fromDate,
                            EndDate = toDate ,
                            PageNumber = pageNumber,
                            PageSize = pageSize, Status = status 
                        });
        }

        public async Task<PagedResponseDto<EntryLogResponseDto>> SearchAsync(EntryLogFilterDto request)
        {
            var specification = new EntryLogSpecification(request);
            var logs=await _unitOfWork.Repository<EntryLog>().ListAsync(specification);
            int totalCount=await _unitOfWork.Repository<EntryLog>().CountAsync(specification);
            return new PagedResponseDto<EntryLogResponseDto>
            {
                Items = logs.Select(ToDto).ToList(),
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }

        public async Task DeleteLogsByDateRange(EntryLogDeleteDto request)
        {
            if (!request.StartDate.HasValue || !request.EndDate.HasValue)
                throw new ArgumentException("Both StartDate and EndDate are required.");
            if (request.EndDate < request.StartDate)
                throw new ArgumentException("EndDate cannot be earlier than StartDate.");

            var specification = new DeleteLogsSpecification(request);
            
            var logs = await _unitOfWork.Repository<EntryLog>().ListAsync(specification);

            _unitOfWork.Repository<EntryLog>().DeleteRange(logs);
            await _unitOfWork.SaveChangesAsync();
        }
        public async Task DeleteOldestLogs(int cnt)
        {
  
            if (cnt <= 0)
                throw new ArgumentException("Number of logs must be greater than zero.");


            var specification = new DeleteLogsSpecification(new EntryLogDeleteDto { Cnt=cnt});
            
            var logs = await _unitOfWork.Repository<EntryLog>().ListAsync(specification);

            _unitOfWork.Repository<EntryLog>().DeleteRange(logs);
            await _unitOfWork.SaveChangesAsync();
        }







        public async Task<byte[]> ExportAttendanceToExcelAsync(DateTime fromDate, DateTime toDate, int? labId = null)
        {
            if (toDate < fromDate)
                throw new ArgumentException ("toDate can't be earlier than fromDate.");

            var spec = new AttendanceExportSpecification(fromDate, toDate, labId);
            var logs = await _unitOfWork.Repository<EntryLog>().ListAsync(spec);

            var orderedLogs = logs.OrderBy(l => l.EventTime).ToList();

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Attendance Report");
            // Title & Metadata
            worksheet.Cell(1, 1).Value = "Lab Attendance Report";
            worksheet.Cell(1, 1).Style.Font.Bold = true;
            worksheet.Cell(1, 1).Style.Font.FontSize = 16;
            worksheet.Cell(2, 1).Value = $"Range: {fromDate:yyyy-MM-dd HH:mm} to {toDate:yyyy-MM-dd HH:mm}";
            // Headers (Row 4)
            string[] headers = ["#", "Date & Time", "Student ID", "Full Name", "Academic Year", "Lab", "Status"];
            for (int i = 0; i < headers.Length; i++)
            {
                var cell = worksheet.Cell(4, i + 1);
                cell.Value = headers[i];
                cell.Style.Font.Bold = true;
                cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#1E3A8A"); // Navy blue
                cell.Style.Font.FontColor = XLColor.White;
                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            }
            // Populate Data (Starting Row 5)
            int row = 5;
            int index = 1;
            foreach (var log in orderedLogs)
            {
                worksheet.Cell(row, 1).Value = index++;
                worksheet.Cell(row, 2).Value = log.EventTime.ToString("yyyy-MM-dd HH:mm:ss");
                worksheet.Cell(row, 3).Value = log.Visitor?.MemberId.ToString() ?? "N/A";
                worksheet.Cell(row, 4).Value = log.Visitor?.FullName ?? "Unknown";
                worksheet.Cell(row, 5).Value = log.Visitor?.AcademicYear.ToString() ?? "-";
                worksheet.Cell(row, 6).Value = log.Lab?.Name ?? "N/A";
                worksheet.Cell(row, 7).Value = log.Status.ToString();
                // Alternating row styling
                if (row % 2 == 0)
                    worksheet.Row(row).Style.Fill.BackgroundColor = XLColor.FromHtml("#F8FAFC");
                row++;
            }
            // Auto-fit column widths
            worksheet.Columns().AdjustToContents();
            // 3. Save to byte array
            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }



        public async Task<LogRotationConfigDto> GetRotationConfigAsync()
        {
            var config = (await _unitOfWork.Repository<LogRotationConfig>().GetAllAsync()).FirstOrDefault()
                         ?? new LogRotationConfig();

            return new LogRotationConfigDto
            {
                IsEnabled = config.IsEnabled,
                RetentionDays = config.RetentionDays,
                RunAtHourUtc = config.RunAtHourUtc,
                LastRunAt = config.LastRunAt
            };
        }

        public async Task UpdateRotationConfigAsync(LogRotationConfigDto dto)
        {
            var config = (await _unitOfWork.Repository<LogRotationConfig>().GetAllAsync()).FirstOrDefault();
            if (config == null)
            {
                config = new LogRotationConfig
                {
                    IsEnabled = dto.IsEnabled,
                    RetentionDays = dto.RetentionDays > 0 ? dto.RetentionDays : 30,
                    RunAtHourUtc = Math.Clamp(dto.RunAtHourUtc, 0, 23)
                };
                await _unitOfWork.Repository<LogRotationConfig>().AddAsync(config);
            }
            else
            {
                config.IsEnabled = dto.IsEnabled;
                config.RetentionDays = dto.RetentionDays > 0 ? dto.RetentionDays : 30; /////////
                config.RunAtHourUtc = Math.Clamp(dto.RunAtHourUtc, 0, 23);
                _unitOfWork.Repository<LogRotationConfig>().Update(config);
            }

            await _unitOfWork.SaveChangesAsync();
        }






        //Helper Methods
        public EntryLog ToEntity(EntryLogRequestDto req)
        {
            return new EntryLog
            {
                EventTime = req.EventTime,
                LabId = req.LabId,
                VisitorId = req.VisitorId,
                Status = req.status
            };
        }

        public EntryLogResponseDto ToDto(EntryLog entity)
        {
            return new EntryLogResponseDto
            {
                Id = entity.Id,
                EventTime = entity.EventTime,
                LabId = entity.LabId,
                LabName = entity.Lab?.Name,
                VisitorId = entity.VisitorId,
                MemberId = entity.Visitor?.MemberId,
                VisitorName = entity.Visitor?.FullName,
                Status = entity.Status,
                FaceImage = entity.Visitor?.FaceImage
            };
        }
    }
}
