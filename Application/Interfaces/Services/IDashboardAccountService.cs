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

        Task Deactivate(int id);

        Task Activate(int id);
    }
}
