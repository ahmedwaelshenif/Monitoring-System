using Application.DTOs;
using Application.DTOs.RequestDtos;
using Application.DTOs.ResponseDtos;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.Services
{
    public interface IDashboardAccountService
    {
        
        Task CreateAsync(DashboardAccountRequestDto account);
        Task<DashboardAccountResponseDto?> GetByIdAsync(int id);

        Task<PagedResponseDto<DashboardAccountResponseDto>> GetAllAsync(int pageNumber, int pageSize);

        Task UpdateAsync(int id,UpdateDashboardAccountDto update);

        Task SetActiveStatusAsync(int id, bool state);

        Task DeleteAsync(int id);
        Task<PagedResponseDto<DashboardAccountResponseDto>> SearchAsync(AccountFilterDto request);

    }
}
