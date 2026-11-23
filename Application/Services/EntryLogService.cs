using Application.DTOs.RequestDtos;
using Application.DTOs.ResponseDtos;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Domain.Entities;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
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
            await _unitOfWork.EntryLogs.AddAsync(ToEntity(request));
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            EntryLog? entity = await _unitOfWork.EntryLogs.GetByIdAsync(id);
            if (entity == null)
            {
                throw new KeyNotFoundException($"log with id {id} is not found");
            }
            _unitOfWork.EntryLogs.Delete(entity);
            await _unitOfWork.SaveChangesAsync();

        }

        public async Task<PagedResponseDto<EntryLogResponseDto>> GetAllAsync(int pageNumber, int pageSize)
        {
            var result = await _unitOfWork.EntryLogs.GetPagedWithDetailsAsync(
                pageNumber,
                pageSize);
            
            return new PagedResponseDto<EntryLogResponseDto>
            {
                Items = result.Items.Select(ToDto),
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = result.TotalCount
            };
        }

        public async Task<int> CountStatus(bool status, DateOnly date)
        {
            var startOfDay = date.ToDateTime(TimeOnly.MinValue);
            var endOfDay = startOfDay.AddDays(1);

            var targetStatus = status
                ? EntryStatus.Accepted
                : EntryStatus.Rejected;

            return await _unitOfWork.EntryLogs.CountAsync(
                e => e.status == targetStatus &&
                     e.EventTime >= startOfDay &&
                     e.EventTime < endOfDay
            );
        }



        public async Task<EntryLogResponseDto?> GetByIdAsync(int id)
        {
            var log= await _unitOfWork.EntryLogs.GetByIdAsync(id);
            if (log == null)
            {
                throw new KeyNotFoundException($"log with id {id} is not found");
            }
            return ToDto(log);
        }


        public async Task<PagedResponseDto<EntryLogResponseDto>> GetByLabAsync(int LabId,int pageNumber,int pageSize)
        {
            var result = await _unitOfWork.EntryLogs.GetPagedLogsByLabAsync(
                LabId,
                pageNumber,
                pageSize);

            return new PagedResponseDto<EntryLogResponseDto>
            {
                Items = result.Items.Select(ToDto),
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = result.TotalCount
            };
        }
        public async Task<PagedResponseDto<EntryLogResponseDto>> GetAcceptedTodayAsync(int pageNumber,int pageSize)
        {
            var result = await _unitOfWork.EntryLogs.GetPagedAcceptedTodayAsync(
                pageNumber,
                pageSize);

            return new PagedResponseDto<EntryLogResponseDto>
            {
                Items = result.Items.Select(ToDto),
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = result.TotalCount
            };
        }
        public async Task<PagedResponseDto<EntryLogResponseDto>> GetRejectedTodayAsync(int pageNumber,int pageSize)
        {
            var result = await _unitOfWork.EntryLogs.GetPagedRejectedTodayAsync(
                pageNumber,
                pageSize);

            return new PagedResponseDto<EntryLogResponseDto>
            {
                Items = result.Items.Select(ToDto),
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = result.TotalCount
            };
        }

        //Helper Methods
        public EntryLog ToEntity(EntryLogRequestDto req)
        {
            EntryLog res = new();
            res.EventTime = req.EventTime;
            res.LabId = req.LabId;
            if (req.Lab != null) res.Lab = req.Lab;
            res.MemberId = req.MemberId;
            res.Member = req.Member;
            res.status = req.status;
            return res;
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
