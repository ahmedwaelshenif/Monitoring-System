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

        public async Task<PagedResponse<MemberResponse>> GetAllAsync(int pageNumber, int pageSize,bool active)
        {

            var result = await _unitOfWork.Members.GetPagedAsync(pageNumber, pageSize,m=>m.Active==active);

            return new PagedResponse<MemberResponse>
            {
                Items = result.Items.Select(ToDto),
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = result.TotalCount
            };
        }


        public async Task SoftDeleteAsync(int id)
        {
            var member = await _unitOfWork.Members.GetByIdAsync(id);
            if (member == null)
                throw new KeyNotFoundException("Member not found");
            member.Active = false;
            _unitOfWork.Members.Update(member);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task RestoreAsync(int id)
        {
            var member = await _unitOfWork.Members.GetByIdAsync(id);
            if (member == null)
                throw new KeyNotFoundException("Member not found");
            member.Active = true;
            _unitOfWork.Members.Update(member);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task PermanentDeleteAsync(int id)
        {
            var member = await _unitOfWork.Members.GetByIdAsync(id);
            if (member == null)
                throw new KeyNotFoundException("Member not found");
            _unitOfWork.Members.Delete(member); 
            await _unitOfWork.SaveChangesAsync(); 

        }

        public async Task<PagedResponse<MemberResponse>> SearchAsync(SearchMemberRequest request)
        {
            var specification = new SearchMemberSpecification(request);

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
            ret.Active = entity.Active;
            return ret;
        }


    }
}
