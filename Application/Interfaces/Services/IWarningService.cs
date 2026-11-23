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
        
        Task<PagedResponseDto<WarningResponseDto>> GetAllAsync(int pageNumber, int pageSize);
        Task<PagedResponseDto<WarningResponseDto>> SearchAsync(WarningFilterDto filter);
        Task CreateAsync(WarningRequestDto request);
        Task DeleteAsync(int id);

        Task<PagedResponseDto<WarningResponseDto>> GetWarningsByLabIdAsync(int labId,int pageNumber, int pageSize);
        Task<PagedResponseDto<WarningResponseDto>> GetWarningsByLabNameAsync(string labName, int pageNumber, int pageSize);
        
        Task<PagedResponseDto<WarningResponseDto>> GetWarningsByAlertAsync(Alert warning,int pageNumber, int pageSize);
    }
}
