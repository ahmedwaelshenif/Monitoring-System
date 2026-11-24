using Application.DTOs.RequestDtos;
using Application.DTOs.ResponseDtos;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.Specifications;
using Domain.Entities;
using Domain.Enums;
using System;
using System.Collections.Generic;
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
                e => e.status == targetStatus &&
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
                throw new ArgumentException("End date cannot be earlier than start date.");

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

        //Helper Methods
        public EntryLog ToEntity(EntryLogRequestDto req)
        {
            return new EntryLog
            {
                EventTime = req.EventTime,
                LabId = req.LabId,
                MemberId = req.MemberId,
                status = req.status
            };
        }

        public EntryLogResponseDto ToDto(EntryLog entity)
        {
            EntryLogResponseDto res = new();
            res.Id = entity.Id;
            res.EventTime = entity.EventTime;
            res.LabId = entity.LabId;
            res.Lab = entity.Lab;
            res.MemberId = entity.MemberId;
            res.Member = entity.Member;
            res.status = entity.status;
            res.FaceImage = entity.Member?.FaceImage;
            return res;
        }    
    }
}
