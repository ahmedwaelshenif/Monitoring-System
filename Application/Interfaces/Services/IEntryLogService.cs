using Application.DTOs.RequestDtos;
using Application.DTOs.ResponseDtos;
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
        Task DeleteAsync(int id);
        Task<int> CountStatus(bool status,DateOnly date);

        Task<PagedResponseDto<EntryLogResponseDto>> GetByLabAsync(int LabId, int pageNumber, int pageSize);


        Task<PagedResponseDto<EntryLogResponseDto>> GetRejectedTodayAsync(int pageNumber, int pageSize);
        Task<PagedResponseDto<EntryLogResponseDto>> GetAcceptedTodayAsync(int pageNumber, int pageSize);


    }
}
