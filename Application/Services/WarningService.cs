using Application.DTOs.RequestDtos;
using Application.DTOs.ResponseDtos;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.Specifications;
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Http.HttpResults;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Services
{
    public class WarningService : IWarningService
    {

        private readonly IUnitOfWork _unitOfWork;

        public WarningService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task CreateAsync(WarningRequestDto request)
        {
            await _unitOfWork.Repository<Warning>().AddAsync(ToEntity(request));
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            Warning? entity = await _unitOfWork.Repository<Warning>().GetByIdAsync(id);
            if(entity == null)
            {
                throw new KeyNotFoundException($"warning with {id} id is not found");
            }

            _unitOfWork.Repository<Warning>().Delete(entity);
            await _unitOfWork.SaveChangesAsync();
            
        }

        public async Task<PagedResponseDto<WarningResponseDto>> GetAllAsync(int pageNumber, int pageSize)
        {
          
            return await SearchAsync(new WarningFilterDto
            {
                PageNumber = pageNumber,
                PageSize = pageSize
            });

        }

        public async Task<PagedResponseDto<WarningResponseDto>> GetWarningsByAlertAsync(Alert warning,int pageNumber, int pageSize)
        {
            return await SearchAsync(new WarningFilterDto
            {
                Warning = warning,
                PageNumber = pageNumber,
                PageSize = pageSize
            });
        }

        public async Task<PagedResponseDto<WarningResponseDto>> GetWarningsByLabIdAsync(int LabID,int pageNumber, int pageSize)
        {

            return await SearchAsync(new WarningFilterDto
            {
                LabId = LabID,
                PageNumber = pageNumber,
                PageSize = pageSize
            });
        }

        public async Task<PagedResponseDto<WarningResponseDto>> GetWarningsByLabNameAsync(string labName, int pageNumber, int pageSize)
        {
            return await SearchAsync(new WarningFilterDto
            {
                LabName = labName,
                PageNumber = pageNumber,
                PageSize = pageSize
            });
        }

        public async Task<PagedResponseDto<WarningResponseDto>> SearchAsync(WarningFilterDto filter)
        {
            var specifications = new WarningSpecification(filter);
            var warnings = await _unitOfWork.Repository<Warning>().ListAsync(specifications);
            int totalCount = await _unitOfWork.Repository<Warning>().CountAsync(specifications);

            return new PagedResponseDto<WarningResponseDto>
            {
                Items = warnings.Select(ToDto).ToList(),
                TotalCount = totalCount,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize
            };

        }

        //Helper Methods
        public WarningResponseDto ToDto (Warning entity)
        {
            WarningResponseDto res = new();

            res.Id = entity.Id;
            res.Lab = entity.Lab;
            res.LabId = entity.LabId;
            res.warning = entity.warning;
            return res;
        }
       public Warning ToEntity (WarningRequestDto request)
        {
            Warning res = new();
            res.LabId = request.LabId;
            res.warning = request.warning;
            return res;
        }

       
    }
}
