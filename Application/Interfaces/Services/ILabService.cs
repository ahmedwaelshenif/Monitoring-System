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
        Task CreateAsync(LabRequestDto lab);
        Task<LabResponseDto?> GetByIdAsync(int id);
        Task<PagedResponseDto<LabResponseDto>> GetAllAsync(int pageNumber, int pageSize,bool active);
        Task<bool> UpdateAsync(int id, UpdateLabDto update);

        Task<int> CountAsync();

        Task SoftDeleteAsync(int id);
        Task RestoreAsync(int id);
        Task PermanentDeleteAsync(int id);

        Task<PagedResponseDto<LabResponseDto>> SearchAsync(LabFilterDto request);

    }
}
