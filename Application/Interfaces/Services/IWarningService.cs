using Application.DTOs.RequestDtos;
using Application.DTOs.ResponseDtos;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.Services
{
    public interface IWarningService
    {
        Task<WarningResponse?> GetByIdAsync(int id);
        Task<PagedResponse<WarningResponse>> GetAllAsync(int pageNumber, int pageSize);
        Task CreateAsync(WarningRequest request);
        Task DeleteAsync(int id);

        Task<PagedResponse<WarningResponse>> GetWarningsByLabAsync(int labId,int pageNumber, int pageSize);
        Task<PagedResponse<WarningResponse>> GetWarningsByAlertAsync(Alert warning,int pageNumber, int pageSize);
    }
}
