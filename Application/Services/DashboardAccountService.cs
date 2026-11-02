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
        private readonly IDashboardAccountRepository _dashboardAccountRepository;
        private readonly PasswordHasher<DashboardAccount> _passwordHasher;

        public DashboardAccountService(IDashboardAccountRepository dashboardAccountRepository, PasswordHasher<DashboardAccount> passwordHasher)
        {
            _dashboardAccountRepository = dashboardAccountRepository;
            _passwordHasher = passwordHasher;
        }


        public async Task CreateAsync(DashboardAccountRequest account)
        {
            DashboardAccount entity = ToEntity(account);
            await _dashboardAccountRepository.AddAsync(entity);
            await _dashboardAccountRepository.SaveChangesAsync();
        }

        public async Task<DashboardAccountResponse?> GetByIdAsync(int id)
        {
            DashboardAccount? entity = await _dashboardAccountRepository.GetByIdAsync(id);
            DashboardAccountResponse ret = ToDto(entity);
            return ret;
        }

        public async Task<PagedResponse<DashboardAccountResponse>> GetAllAsync(int pageNumber, int pageSize)
        {
            var result = await _dashboardAccountRepository.GetPagedAsync(
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
            DashboardAccount entity = await _dashboardAccountRepository.GetByIdAsync(id);
            entity.IsActive = false;
            await _dashboardAccountRepository.SaveChangesAsync();
        }

        public async Task Activate(int id)
        {
            DashboardAccount entity = await _dashboardAccountRepository.GetByIdAsync(id);
            entity.IsActive = true;
            await _dashboardAccountRepository.SaveChangesAsync();
        }




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
