using Application.DTOs.RequestDtos;
using Application.DTOs.ResponseDtos;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.Services
{
    public interface IEntryLogService
    {
        Task<EntryLogResponseDto?> GetByIdAsync(int id);
        Task<PagedResponseDto<EntryLogResponseDto>> GetAllAsync(int pageNumber, int pageSize);
        Task CreateAsync(EntryLogRequestDto request);
        Task<PagedResponseDto<EntryLogResponseDto>> SearchAsync(EntryLogFilterDto request);
        Task DeleteAsync(int id);
        Task<int> CountStatus(bool status,DateOnly date);

        Task<PagedResponseDto<EntryLogResponseDto>> GetByLabAsync(int LabId, int pageNumber, int pageSize);
        Task<PagedResponseDto<EntryLogResponseDto>> GetByDateRangeAsync(DateTime fromDate,DateTime toDate,int pageNumber,int pageSize, EntryStatus? status = null);

        Task DeleteOldestLogs(int cnt);

        Task DeleteLogsByDateRange(EntryLogDeleteDto request);

    }
}
