using Application.DTOs.RequestDtos;
using Application.DTOs.ResponseDtos;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Domain.Entities;
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

        public async Task CreateAsync(EntryLogRequest request)
        {
            await _unitOfWork.EntryLogs.AddAsync(ToEntity(request));
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            EntryLog? entity = await _unitOfWork.EntryLogs.GetByIdAsync(id);
            if (entity != null)
            {
                _unitOfWork.EntryLogs.Delete(entity);
                await _unitOfWork.SaveChangesAsync();
                return;
            }
            throw new KeyNotFoundException($"EntryLog with ID {id} not found.");
        }

        public async Task<PagedResponse<EntryLogResponse>> GetAllAsync(int pageNumber, int pageSize)
        {
            var result = await _unitOfWork.EntryLogs.GetPagedWithDetailsAsync(
                pageNumber,
                pageSize);
            
            return new PagedResponse<EntryLogResponse>
            {
                Items = result.Items.Select(ToDto),
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = result.TotalCount
            };
        }

        public async Task<EntryLogResponse?> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public EntryLog ToEntity(EntryLogRequest req)
        {
            EntryLog res = new();
            res.EventTime = req.EventTime;
            res.Lab = req.Lab;
            res.Member = req.Member;
            res.status = req.status;
            return res;
        }

        public EntryLogResponse ToDto(EntryLog entity)
        {
            EntryLogResponse res = new();
            res.Id = entity.Id;
            res.EventTime = entity.EventTime;
            res.Lab = entity.Lab;
            res.Member = entity.Member;
            res.status = entity.status;
            res.FaceImage = entity.Member?.FaceImage;
            return res;
        }
    }
}
