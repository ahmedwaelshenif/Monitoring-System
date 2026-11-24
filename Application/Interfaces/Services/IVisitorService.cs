using Application.DTOs.RequestDtos;
using Application.DTOs.ResponseDtos;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.Services
{
    public interface IVisitorService
    {
        Task CreateAsync(Visitor member);
        Task<PagedResponseDto<VisitorResponseDto>> GetAllAsync(int pageNumber, int pageSize,bool active);
        Task<PagedResponseDto<VisitorResponseDto>> SearchAsync(VisitorFilterDto request);
        Task SetActiveStatusAsync(int id, bool state);
        Task PermanentDeleteAsync(int id);

        Task AddLabAsync(int memberId, int LabId);
        Task RemoveLabAsync(int memberId, int LabId);
        Task<PagedResponseDto<VisitorResponseDto>> GetVisitorsbyLab(int id, PagedRequestDto pagination);

    }
}
