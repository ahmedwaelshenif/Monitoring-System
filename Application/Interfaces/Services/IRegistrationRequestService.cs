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
        Task<PagedResponse<RegistrationResponseDto>> GetAllAsync(int pageNumber, int pageSize);
        Task<bool> CreateAsync(RegistrationRequestDto request);
        //Task UpdateAsync(RegistrationRequestDto request);
        Task DeleteAsync(int id);

        Task MarkStatus(int id, Boolean status);

    }
}

