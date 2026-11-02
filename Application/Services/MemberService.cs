using Application.DTOs.ResponseDtos;
using Application.Interfaces.Services;
using Application.Interfaces.Repositories;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Services
{
    public class MemberService : IMemberService
    {
        private readonly IMemberRepository _memberRepository;
        public MemberService(IMemberRepository memberRepository)
        {
            _memberRepository = memberRepository;
        }
      

        public async Task CreateAsync(Member member)
        {
            await _memberRepository.AddAsync(member);
            await _memberRepository.SaveChangesAsync();
        }

        public async Task<Member?> GetByIdAsync(int id)
        {
            return await _memberRepository.GetByIdAsync(id);
        }


        public async Task<PagedResponse<MemberResponseDto>> GetAllAsync(int pageNumber,int pageSize)
        {
            var result = await _memberRepository.GetPagedAsync(pageNumber,pageSize);

            return new PagedResponse<MemberResponseDto>
            {
                Items = result.Items.Select(ToDto),
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = result.TotalCount
            };
        }




        //Helper Methods
        private MemberResponseDto ToDto(Member entity)
        {
            MemberResponseDto ret = new();
            ret.AcademicYear = entity.AcademicYear;
            ret.FullName = entity.FullName;
            ret.FaceImage = ConvertToFormFile(entity.FaceImage, "FaceImage.jpg");
            ret.IdImage = ConvertToFormFile(entity.IdImage, "IdImage.jpg");
            ret.Id = entity.Id;
            ret.Email = entity.Email;
            return ret;
        }


        private IFormFile ConvertToFormFile(byte[] bytes, string fileName)
        {
            var stream = new MemoryStream(bytes);

            return new FormFile(stream, 0, stream.Length, fileName, fileName)
            {
                Headers = new HeaderDictionary(),
                ContentType = "image/jpeg"
            };
        }
    }
}
