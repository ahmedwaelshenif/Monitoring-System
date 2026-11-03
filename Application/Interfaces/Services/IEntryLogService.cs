using Application.DTOs.RequestDtos;
using Application.DTOs.ResponseDtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.Services
{
    public interface IEntryLogService
    {
        Task<EntryLogResponse?> GetByIdAsync(int id);
        Task<PagedResponse<EntryLogResponse>> GetAllAsync(int pageNumber, int pageSize);
        Task CreateAsync(EntryLogRequest request);
        Task DeleteAsync(int id);

    }
}
