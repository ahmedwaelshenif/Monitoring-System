using Application.DTOs.RequestDtos;
using Application.DTOs.ResponseDtos;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.Services
{
    public interface ILabService
    {
        Task CreateAsync(LabRequest lab);
        Task<LabResponse?> GetByIdAsync(int id);
        Task<PagedResponse<LabResponse>> GetAllAsync(int pageNumber, int pageSize);

    }
}
