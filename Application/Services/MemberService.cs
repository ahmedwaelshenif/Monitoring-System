using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;
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





        //Helper Methods
        public Member ToEnity(MemberDto dto)
        {
            Member ret = new();
            ret.AcademicYear = dto.AcademicYear;
            ret.EmbeddingData = dto.EmbeddingData;
            ret.FullName = dto.FullName;
            ret.FaceImage = dto.FaceImage;
            ret.Id = dto.Id;
            ret.Email = dto.Email;
            ret.IdImage = dto.IdImage;

            return ret;
        }


        public MemberDto ToDto(Member entity)
        {
            MemberDto ret = new();
            ret.AcademicYear = entity.AcademicYear;
            ret.EmbeddingData = entity.EmbeddingData;
            ret.FullName = entity.FullName;
            ret.FaceImage = entity.FaceImage;
            ret.Id = entity.Id;
            ret.Email = entity.Email;
            ret.IdImage = entity.IdImage;

            return ret;
        }

        public Task CreateAsync(MemberDto request)
        {
            throw new NotImplementedException();
        }
    }
}
