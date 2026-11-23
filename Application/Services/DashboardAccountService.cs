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
            DashboardAccount entity = ToEntity(account);
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

        public async Task Deactivate(int id)
        {
            DashboardAccount? entity = await _unitOfWork.Repository<DashboardAccount>().GetByIdAsync(id);
            if (entity == null)
            {
                throw new KeyNotFoundException($"Account with id {id} is not found");
            }

            entity.IsActive = false;
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task Activate(int id)
        {
            DashboardAccount? entity = await _unitOfWork.Repository<DashboardAccount>().GetByIdAsync(id);
            if (entity == null)
            {
                throw new KeyNotFoundException($"Account with id {id} is not found");
            }


            entity.IsActive = true;
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
            _unitOfWork.Repository<DashboardAccount>().Delete(acc);
            await _unitOfWork.SaveChangesAsync();

        }

        public async Task<PagedResponseDto<DashboardAccountResponseDto>> SearchAsync(SearchAccountDto request)
        {
            var specification = new SearchAccountSpecification(request);


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
        public DashboardAccount ToEntity(DashboardAccountRequestDto dto)
        {
            DashboardAccount acc = new();
            acc.Email = dto.Email;
            acc.FullName = dto.FullName;
            acc.Role = dto.Role;

            acc.PasswordHash = _passwordHasher.HashPassword(acc,dto.Password);
            return acc;
        }
        public DashboardAccountResponseDto ToDto(DashboardAccount entity)
        {
            if (entity == null)
                return null;
            DashboardAccountResponseDto ret = new();
            ret.Id = entity.Id;
            ret.FullName = entity.FullName;
            ret.Email = entity.Email;
            ret.Role = entity.Role;
            ret.IsActive = entity.IsActive;

            return ret;
        }

      
    }
}
