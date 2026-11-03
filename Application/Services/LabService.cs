using Application.DTOs.RequestDtos;
using Application.DTOs.ResponseDtos;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Services
{
    public class LabService : ILabService
    {
        private readonly IUnitOfWork _unitOfWork;
        
        public LabService(IUnitOfWork unitOfWork)
        {
            _unitOfWork=unitOfWork;
        }

        public async Task CreateAsync(LabRequest lab)
        {
            await _unitOfWork.Labs.AddAsync(ToEntity(lab));
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<PagedResponse<LabResponse>> GetAllAsync(int pageNumber, int pageSize)
        {
            var result = await _unitOfWork.Labs.GetPagedAsync(pageNumber, pageSize);

            return new PagedResponse<LabResponse>
            {
                Items = result.Items.Select(ToDto),
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = result.TotalCount
            };
        }

        public async Task<LabResponse?> GetByIdAsync(int id)
        {
            Lab? lab = await _unitOfWork.Labs.GetByIdAsync(id);
            if (lab == null)
                return null;
            return ToDto(lab);
        }



        //Helper Methods

        private Lab ToEntity(LabRequest dto)
        {
            Lab lab = new();
            lab.Name = dto.Name;
            lab.Capacity = dto.Capacity;
            return lab;
        }

        private LabResponse ToDto(Lab lab)
        {
            LabResponse labResponse = new();
            labResponse.Name = lab.Name;
            labResponse.Capacity = lab.Capacity;
            labResponse.Id = lab.Id;
            return labResponse;
        }
    }
}
