using Application.DTOs;
using Application.DTOs.RequestDtos;
using Application.DTOs.ResponseDtos;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Application.Interfaces.Services
{
    public interface IRegistrationRequestService
    {
        Task<RegistrationResponseDto?> GetByIdAsync(int id);
        Task<PagedResponseDto<RegistrationResponseDto>> GetAllAsync(int pageNumber, int pageSize);
        Task CreateAsync(RegistrationRequestDto request);
      
        Task DeleteAsync(int id);

        Task AcceptAsync(int id, List<int> LabIds);
        Task RejectAsync(int id);

        Task<PagedResponseDto<RegistrationResponseDto>> SearchAsync(RegistrationRequestFilterDto request);

    }
}

