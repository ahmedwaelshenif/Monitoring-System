using Application.DTOs.RequestDtos;
using Application.DTOs.ResponseDtos;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.Specifications;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Application.Services
{
    public class MemberService : IMemberService
    {

        private readonly IUnitOfWork _unitOfWork;
        public MemberService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task CreateAsync(Member member)
        {
            await _unitOfWork.Members.AddAsync(member);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<Member?> GetByIdAsync(int id)
        {
            return await _unitOfWork.Members.GetByIdAsync(id);
        }

        public async Task<PagedResponse<MemberResponse>> GetAllAsync(int pageNumber, int pageSize)
        {
            var result = await _unitOfWork.Members.GetPagedAsync(pageNumber, pageSize);

            return new PagedResponse<MemberResponse>
            {
                Items = result.Items.Select(ToDto),
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = result.TotalCount
            };
        }


        public async Task DeleteAsync(int id)
        {
            var member = await _unitOfWork.Members.GetByIdAsync(id);
            if (member == null) return;
            _unitOfWork.Members.Delete(member);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<PagedResponse<MemberResponse>> SearchAsync(SearchMemberRequest request)
        {
            var specification = new SearchMemberSpecification(
                request.Id,
                request.FullName,
                request.Email,
                request.AcademicYear,
                request.PageNumber,
                request.PageSize
            );

            var members = await _unitOfWork.Members
                .ListAsync(specification);

            var totalCount = await _unitOfWork.Members
                .CountAsync(specification);

            return new PagedResponse<MemberResponse>
            {
                Items = members.Select(ToDto).ToList(),
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = totalCount
            };
        }

        // Helper Methods
        private MemberResponse ToDto(Member entity)
        {
            if (entity == null)
                return null;

            MemberResponse ret = new();
            ret.AcademicYear = entity.AcademicYear;
            ret.FullName = entity.FullName;
            ret.FaceImage = entity.FaceImage;
            ret.IdImage = entity.IdImage;
            ret.Id = entity.Id;
            ret.Email = entity.Email;
            return ret;
        }

     
    }
}
