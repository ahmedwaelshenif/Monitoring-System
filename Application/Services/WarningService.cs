using Application.DTOs.RequestDtos;
using Application.DTOs.ResponseDtos;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
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
        public async Task CreateAsync(WarningRequest request)
        {
            await _unitOfWork.Warnings.AddAsync(ToEntity(request));
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            Warning? entity = await _unitOfWork.Warnings.GetByIdAsync(id);

            if (entity != null)
            {
                 _unitOfWork.Warnings.Delete(entity);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task<PagedResponse<WarningResponse>> GetAllAsync(int pageNumber, int pageSize)
        {
          var result =await _unitOfWork.Warnings.GetAllWithDetailsAsync(pageNumber,pageSize);

            return new PagedResponse<WarningResponse>
            {
                Items = result.Items.Select(ToDto),
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = result.TotalCount
            };

        }

        public async Task<PagedResponse<WarningResponse>> GetWarningsByAlertAsync(Alert warning,int pageNumber, int pageSize)
        {
            var result = await _unitOfWork.Warnings.GetWarningsByAlertAsync(
                    warning,
                    pageNumber,
                    pageSize);



            return new PagedResponse<WarningResponse>
            {
                Items = result.Items.Select(ToDto),
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = result.TotalCount
            };
        }

        public async Task<PagedResponse<WarningResponse>> GetWarningsByLabAsync(int LabID,int pageNumber, int pageSize)
        {

            var result = await _unitOfWork.Warnings.GetWarningsByLabAsync(
                        LabID,
                        pageNumber,
                        pageSize);

            return new PagedResponse<WarningResponse>
            {
                Items = result.Items.Select(ToDto),
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = result.TotalCount
            };
        }

        public async Task<WarningResponse?> GetByIdAsync(int id)
        {
            Warning? entity = await _unitOfWork.Warnings.GetByIdAsync(id);

            if (entity != null)
            {

                return ToDto(entity);
            }
            return null;
        }

        public WarningResponse ToDto (Warning entity)
        {
            WarningResponse res = new();

            res.Id = entity.Id;
            res.Lab = entity.Lab;
            res.LabId = entity.LabId;
            res.warning = entity.warning;
            return res;
        }
       public Warning ToEntity (WarningRequest request)
        {
            Warning res = new();
            res.LabId = request.LabId;
            res.warning = request.warning;
            return res;
        }
    }
}
