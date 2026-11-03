using Application.DTOs;
using Application.DTOs.RequestDtos;
using Application.DTOs.ResponseDtos;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Domain.Entities;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Principal;
using System.Text;
using Microsoft.AspNetCore.Identity;


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


        public async Task CreateAsync(DashboardAccountRequest account)
        {
            DashboardAccount entity = ToEntity(account);
            await _unitOfWork.DashboardAccounts.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<DashboardAccountResponse?> GetByIdAsync(int id)
        {
            DashboardAccount? entity = await _unitOfWork.DashboardAccounts.GetByIdAsync(id);
            DashboardAccountResponse ret = ToDto(entity);
            return ret;
        }

        public async Task<PagedResponse<DashboardAccountResponse>> GetAllAsync(int pageNumber, int pageSize)
        {
            var result = await _unitOfWork.DashboardAccounts.GetPagedAsync(
                pageNumber,
                pageSize);

            return new PagedResponse<DashboardAccountResponse>
            {
            Items = result.Items.Select(ToDto),
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = result.TotalCount
            };
        }

        public async Task Deactivate(int id)
        {
            DashboardAccount entity = await _unitOfWork.DashboardAccounts.GetByIdAsync(id);
            entity.IsActive = false;
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task Activate(int id)
        {
            DashboardAccount entity = await _unitOfWork.DashboardAccounts.GetByIdAsync(id);
            entity.IsActive = true;
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var acc=await _unitOfWork.DashboardAccounts.GetByIdAsync(id);
            if (acc == null) return;
            _unitOfWork.DashboardAccounts.Delete(acc);
            await _unitOfWork.SaveChangesAsync();

        }

        //Helper Methods
        public DashboardAccount ToEntity(DashboardAccountRequest dto)
        {
            DashboardAccount acc = new();
            acc.Email = dto.Email;
            acc.FullName = dto.FullName;
            acc.Role = dto.Role;

            acc.PasswordHash = _passwordHasher.HashPassword(acc,dto.Password);
            return acc;
        }
        public DashboardAccountResponse ToDto(DashboardAccount entity)
        {
            if (entity == null)
                return null;
            DashboardAccountResponse ret = new();
            ret.Id = entity.Id;
            ret.FullName = entity.FullName;
            ret.Email = entity.Email;
            ret.Role = entity.Role;
            ret.IsActive = entity.IsActive;

            return ret;
        }

     
    }
}
