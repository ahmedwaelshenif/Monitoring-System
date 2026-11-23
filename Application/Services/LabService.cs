using Application.DTOs.RequestDtos;
using Application.DTOs.ResponseDtos;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.Specifications;
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

        public async Task CreateAsync(LabRequestDto lab)
        {
            await _unitOfWork.Labs.AddAsync(ToEntity(lab));
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<PagedResponseDto<LabResponseDto>> GetAllAsync(int pageNumber, int pageSize,bool active)
        {
            var result = await _unitOfWork.Labs.GetPagedAsync(pageNumber, pageSize, l=>l.Active==active);

            return new PagedResponseDto<LabResponseDto>
            {
                Items = result.Items.Select(ToDto),
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = result.TotalCount
            };
        }

        public async Task<LabResponseDto?> GetByIdAsync(int id)
        {
            Lab? lab = await _unitOfWork.Labs.GetByIdAsync(id);
            if (lab == null)
                throw new KeyNotFoundException($"Lab with {id} not found");

            return ToDto(lab);
        }

        public async Task<PagedResponseDto<LabResponseDto>> SearchAsync(SearchLabRequestDto request)
        {
            var specification = new SearchLabSpecification(request);

            var labs = await _unitOfWork.Labs
                .ListAsync(specification);

            var totalCount = await _unitOfWork.Labs
                .CountAsync(specification);

            return new PagedResponseDto<LabResponseDto>
            {
                Items = labs.Select(ToDto).ToList(),
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = totalCount
            };
        }

        public async Task<bool> UpdateAsync(int id, UpdateLabDto update)
        {
            var lab= await _unitOfWork.Labs.GetByIdAsync(id);

            if(lab==null)
                throw new KeyNotFoundException($"Lab with {id} not found");

            if (!string.IsNullOrWhiteSpace(update.Name))
                lab.Name = update.Name;

            if (update.Capacity.HasValue)
                lab.Capacity = update.Capacity.Value;


            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task SoftDeleteAsync(int id)
        {
            var lab = await _unitOfWork.Labs.GetByIdAsync(id);

            if (lab == null)
                throw new KeyNotFoundException($"Lab with {id} not found");
            lab.Active = false;
            await _unitOfWork.SaveChangesAsync();
        }

        public async  Task RestoreAsync(int id)
        {
            var lab = await _unitOfWork.Labs.GetByIdAsync(id);

            if (lab == null)
                throw new KeyNotFoundException($"Lab with {id} not found");

            lab.Active = true;
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task PermanentDeleteAsync(int id)
        {
            var lab = await _unitOfWork.Labs.GetByIdAsync(id);

            if (lab == null)
                throw new KeyNotFoundException($"Lab with {id} not found");

            _unitOfWork.Labs.Delete(lab);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<int> CountAsync()
        {
            return await _unitOfWork.Labs.CountAsync();
        }



        //Helper Methods

        private Lab ToEntity(LabRequestDto dto)
        {
            Lab lab = new();
            lab.Name = dto.Name;
            lab.Capacity = dto.Capacity;
            return lab;
        }

        private LabResponseDto ToDto(Lab lab)
        {
            LabResponseDto labResponse = new();
            labResponse.Name = lab.Name;
            labResponse.Capacity = lab.Capacity;
            labResponse.Id = lab.Id;
            labResponse.active = lab.Active;
            return labResponse;
        }


    }
}
