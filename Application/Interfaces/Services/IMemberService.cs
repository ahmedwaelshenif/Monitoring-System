using Application.DTOs.RequestDtos;
using Application.DTOs.ResponseDtos;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.Services
{
    public interface IMemberService
    {
        Task CreateAsync(Member member);
        Task<Member?> GetByIdAsync(int id);
        Task<PagedResponseDto<MemberResponseDto>> GetAllAsync(int pageNumber, int pageSize,bool active);
        Task<PagedResponseDto<MemberResponseDto>> SearchAsync(SearchMemberRequestDto request);

        Task SoftDeleteAsync(int id);
        Task RestoreAsync(int id);
        Task PermanentDeleteAsync(int id);

    }
}
