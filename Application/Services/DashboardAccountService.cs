using Application.DTOs;
using Application.DTOs.RequestDtos;
using Application.DTOs.ResponseDtos;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.Specifications;
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Principal;
using System.Text;


namespace Application.Services
{
    public class DashboardAccountService : IDashboardAccountService
    {
        private readonly IPasswordHasher<DashboardAccount> _passwordHasher;

        private readonly IUnitOfWork _unitOfWork;

        public DashboardAccountService(IUnitOfWork unitOfWork, IPasswordHasher<DashboardAccount> passwordHasher)
        {
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
        }


        public async Task CreateAsync(DashboardAccountRequestDto account)
        {
            DashboardAccount entity = await ToEntity(account);
            await _unitOfWork.Repository<DashboardAccount>().AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<DashboardAccountResponseDto?> GetByIdAsync(int id)
        {
            DashboardAccount? entity = await _unitOfWork.Repository<DashboardAccount>().GetByIdAsync(id);
            if (entity == null)
            {
                throw new KeyNotFoundException($"Account with id {id} is not found");
            }
            DashboardAccountResponseDto ret = ToDto(entity);
            return ret;
        }

        public async Task<PagedResponseDto<DashboardAccountResponseDto>> GetAllAsync(int pageNumber, int pageSize)
        {
            var result = await _unitOfWork.Repository<DashboardAccount>().GetPagedAsync(
                pageNumber,
                pageSize);

            return new PagedResponseDto<DashboardAccountResponseDto>
            {
            Items = result.Items.Select(ToDto),
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = result.TotalCount
            };
        }

        public async Task SetActiveStatusAsync(int id,bool state)
        {
            DashboardAccount? entity = await _unitOfWork.Repository<DashboardAccount>().GetByIdAsync(id);
            if (entity == null)
                throw new KeyNotFoundException($"Account with id {id} is not found");

            if(entity.Role==Role.Admin)
                throw new InvalidOperationException("Can't change Active Status for an admin");

            if (state == entity.IsActive)
                throw new InvalidOperationException("The active status is already set to the requested value.");

            entity.IsActive = state;
            await _unitOfWork.SaveChangesAsync();
        }

       
        public async Task UpdateAsync(int id,UpdateDashboardAccountDto update)
        {
            var acc = await _unitOfWork.Repository<DashboardAccount>().GetByIdAsync(id);
            if (acc == null)
            {
                throw new KeyNotFoundException($"Account with id {id} is not found");
            }
            if(update.FullName!=null) 
                acc.FullName = update.FullName;

            if (update.Email != null)
                acc.Email = update.Email;

            if (!string.IsNullOrWhiteSpace(update.Password))
            {
                acc.PasswordHash =
                    _passwordHasher.HashPassword(acc, update.Password);
            }
            if (update.Role.HasValue)
                acc.Role = update.Role.Value;
            await _unitOfWork.SaveChangesAsync();
           
        }

        public async Task DeleteAsync(int id)
        {
            var acc=await _unitOfWork.Repository<DashboardAccount>().GetByIdAsync(id);
            if (acc == null)
            {
                throw new KeyNotFoundException($"Account with id {id} is not found");
            }
            if (acc.Role == Role.Admin)
                throw new InvalidOperationException("Cannot delete an Admin");
            _unitOfWork.Repository<DashboardAccount>().Delete(acc);
            await _unitOfWork.SaveChangesAsync();

        }

        public async Task<PagedResponseDto<DashboardAccountResponseDto>> SearchAsync(AccountFilterDto request)
        {
            var specification = new AccountSpecification(request);


            var accounts = await _unitOfWork.Repository<DashboardAccount>()
                .ListAsync(specification);

            var totalCount = await _unitOfWork.Repository<DashboardAccount>()
                .CountAsync(specification);

            return new PagedResponseDto<DashboardAccountResponseDto>
            {
                Items = accounts.Select(ToDto).ToList(),
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = totalCount
            };
        }

        //Helper Methods
        private async Task<DashboardAccount> ToEntity(DashboardAccountRequestDto dto)
        {
            DashboardAccount acc = new();
            acc.Email = dto.Email;
            acc.FullName = dto.FullName;
            acc.Role = dto.Role;

            if (dto.LabIds == null || !dto.LabIds.Any())
                throw new InvalidOperationException("At least one lab must be assigned.");

            var labs = await _unitOfWork.Repository<Lab>().GetAllAsync(l => dto.LabIds.Contains(l.Id) &&l.Active);

            if (labs.Count() != dto.LabIds.Distinct().Count())
                throw new KeyNotFoundException("One or more selected labs do not exist or are inactive.");

            acc.Labs = labs.ToList();
            acc.PasswordHash = _passwordHasher.HashPassword(acc,dto.Password);
            return acc;
        }
        private DashboardAccountResponseDto ToDto(DashboardAccount entity)
        {
            if (entity == null)
                return null;
            return new DashboardAccountResponseDto {
                Id = entity.Id,
                FullName = entity.FullName,
                Email = entity.Email,
                Role = entity.Role,
                IsActive = entity.IsActive,
            };  
        }

      
    }
}
