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
            await _unitOfWork.Repository<Member>().AddAsync(member);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<Member?> GetByIdAsync(int id)
        {
            return await _unitOfWork.Repository<Member>().GetByIdAsync(id);
        }



        public async Task<PagedResponseDto<MemberResponseDto>> GetAllAsync(int pageNumber, int pageSize,bool active)
        {
            var result = await _unitOfWork.Repository<Member>().GetPagedAsync(pageNumber, pageSize,m=>m.Active==active);

            return new PagedResponseDto<MemberResponseDto>
            {
                Items = result.Items.Select(ToDto),
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = result.TotalCount
            };
        }


        public async Task SetActiveStatusAsync(int id,bool state)
        {
            var member = await _unitOfWork.Repository<Member>().GetByIdAsync(id);

            if (member == null)
                throw new KeyNotFoundException("Member not found");


            if (state == member.Active)
                throw new InvalidOperationException("The active status is already set to the requested value.");

            member.Active = state;
            _unitOfWork.Repository<Member>().Update(member);
            await _unitOfWork.SaveChangesAsync();
        }

      

        public async Task PermanentDeleteAsync(int id)
        {
            var member = await _unitOfWork.Repository<Member>().GetByIdAsync(id);
            if (member == null)
                throw new KeyNotFoundException("Member not found");
            _unitOfWork.Repository<Member>().Delete(member); 
            await _unitOfWork.SaveChangesAsync(); 
        }

        public async Task<PagedResponseDto<MemberResponseDto>> SearchAsync(MemberFilterDto request)
        {
            var specification = new MemberSpecification(request);

            var members = await _unitOfWork.Repository<Member>()
                .ListAsync(specification);

            var totalCount = await _unitOfWork.Repository<Member>()
                .CountAsync(specification);

            return new PagedResponseDto<MemberResponseDto>
            {
                Items = members.Select(ToDto).ToList(),
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = totalCount
            };
        }

        // Helper Methods
        private MemberResponseDto ToDto(Member entity)
        {
            if (entity == null)
                return null;

            MemberResponseDto ret = new();
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


    }
}
