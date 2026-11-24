using Application.DTOs.RequestDtos;
using Application.DTOs.ResponseDtos;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.Specifications;
using DocumentFormat.OpenXml.Office2010.Excel;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Application.Services
{
    public class VisitorService : IVisitorService
    {

        private readonly IUnitOfWork _unitOfWork;
        public VisitorService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task CreateAsync(Visitor visitor)
        {
            await _unitOfWork.Repository<Visitor>().AddAsync(visitor);
            await _unitOfWork.SaveChangesAsync();
        }

       




        public async Task<PagedResponseDto<VisitorResponseDto>> GetAllAsync(int pageNumber, int pageSize,bool active)
        {
            var result = await _unitOfWork.Repository<Visitor>().GetPagedAsync(pageNumber, pageSize,m=>m.Active==active);

            return new PagedResponseDto<VisitorResponseDto>
            {
                Items = result.Items.Select(ToDto),
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = result.TotalCount
            };
        }


        public async Task SetActiveStatusAsync(int id,bool state)
        {
            var visitor = await _unitOfWork.Repository<Visitor>().GetByIdAsync(id);

            if (visitor == null)
                throw new KeyNotFoundException("Member not found");


            if (state == visitor.Active)
                throw new InvalidOperationException("The active status is already set to the requested value.");

            visitor.Active = state;
            _unitOfWork.Repository<Visitor>().Update(visitor);
            await _unitOfWork.SaveChangesAsync();
        }

      

        public async Task PermanentDeleteAsync(int id)
        {
            var visitor = await _unitOfWork.Repository<Visitor>().GetByIdAsync(id);
            if (visitor == null)
                throw new KeyNotFoundException("Member not found");
            _unitOfWork.Repository<Visitor>().Delete(visitor); 
            await _unitOfWork.SaveChangesAsync(); 
        }

        public async Task<PagedResponseDto<VisitorResponseDto>> SearchAsync(VisitorFilterDto request)
        {
            var specification = new VisitorSpecification(request);

            var visitor = await _unitOfWork.Repository<Visitor>()
                .ListAsync(specification);

            var totalCount = await _unitOfWork.Repository<Visitor>()
                .CountAsync(specification);

            return new PagedResponseDto<VisitorResponseDto>
            {
                Items = visitor.Select(ToDto).ToList(),
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = totalCount
            };
        }

        public async Task<PagedResponseDto<VisitorResponseDto>> GetVisitorsbyLab(int id, PagedRequestDto pagination)
        {
            var visitors = await _unitOfWork.Repository<Visitor>().ListAsync(new VisitorByLabSpecification(id, pagination.PageNumber, pagination.PageSize));
            int totalCount = await _unitOfWork.Repository<Visitor>().CountAsync(new VisitorByLabSpecification(id, pagination.PageNumber, pagination.PageSize));
            return new PagedResponseDto<VisitorResponseDto>
            {
                Items = visitors.Select(ToDto).ToList(),
                TotalCount = totalCount,
                PageNumber = pagination.PageNumber,
                PageSize = pagination.PageSize
            };
        }

        // Helper Methods
        private VisitorResponseDto ToDto(Visitor entity)
        {
            if (entity == null)
                return null;

            VisitorResponseDto ret = new();
            ret.Id = entity.Id;
            ret.MemberId = entity.MemberId;
            ret.AcademicYear = entity.AcademicYear;
            ret.FullName = entity.FullName;
            ret.FaceImage = entity.FaceImage;
            ret.IdImage = entity.IdImage;
            ret.Email = entity.Email;
            ret.Active = entity.Active;
            return ret;
        }

        public async Task AddLabAsync(int visitorId, int labId)
        {

            var visitor = await _unitOfWork.Repository<Visitor>().GetEntityWithSpecAsync(new VisitorWithLabsSpecification(visitorId));
            if (visitor == null)
                throw new KeyNotFoundException("Member doesn't exist");

            if (!visitor.Active)
                throw new InvalidOperationException("Member is Inactive");


            var lab = await _unitOfWork.Repository<Lab>().GetByIdAsync(labId);
            if (lab == null)
                throw new KeyNotFoundException("Lab doesn't exist");

            if (!lab.Active)
                throw new InvalidOperationException("Lab is Inactive");

            if (visitor.Labs.Any(l => l.Id == lab.Id))
                throw new InvalidOperationException("Member already registered in lab");

            visitor.Labs.Add(lab);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task RemoveLabAsync(int visitorId, int labId)
        {

            var visitor = await _unitOfWork.Repository<Visitor>().GetEntityWithSpecAsync(new VisitorWithLabsSpecification(visitorId));
            if (visitor == null)
                throw new KeyNotFoundException("Member doesn't exist");


            var lab = await _unitOfWork.Repository<Lab>().GetByIdAsync(labId);
            if (lab == null)
                throw new KeyNotFoundException("Lab doesn't exist");
           

            if (!visitor.Labs.Any(l => l.Id == lab.Id))
                throw new InvalidOperationException("Member isn't registered in lab");

            visitor.Labs.Remove(lab);
            await _unitOfWork.SaveChangesAsync();
        }


    }
}
