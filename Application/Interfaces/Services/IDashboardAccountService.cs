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
        
        Task CreateAsync(DashboardAccountRequest account);
        Task<DashboardAccountResponse?> GetByIdAsync(int id);

        Task<PagedResponse<DashboardAccountResponse>> GetAllAsync(int pageNumber, int pageSize);

        Task<bool> UpdateAsync(int id,UpdateDashboardAccount update);

        Task Deactivate(int id);

        Task Activate(int id);

        Task DeleteAsync(int id);
        Task<PagedResponse<DashboardAccountResponse>> SearchAsync(SearchAccount request);

    }
}
