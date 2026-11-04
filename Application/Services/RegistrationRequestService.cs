using Application.DTOs.RequestDtos;
using Application.DTOs.ResponseDtos;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.Specifications;
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Application.Services
{
    public class RegistrationRequestService : IRegistrationRequestService
    {
        private readonly IEmbeddingService _embeddingService;

        private readonly IUnitOfWork _unitOfWork;

        public RegistrationRequestService(
            IEmbeddingService embeddingService,
            IUnitOfWork unitOfWork
            )
        {
            _embeddingService = embeddingService;
            _unitOfWork =unitOfWork;
        }

        public async Task<bool> CreateAsync(RegistrationRequestDto request)
        {
            // check member entity
            int cnt2= await _unitOfWork.Members.CountAsync(r => r.Id == request.MemberId);
            int cnt = await _unitOfWork.RegistrationRequests.CountAsync(r => r.MemberId == request.MemberId && r.status == RequestStatus.Accepted);
            if (cnt > 0 || cnt2>0)
                return false;
            var requests = await _unitOfWork.RegistrationRequests.GetAllAsync(r => r.MemberId == request.MemberId && r.status== RequestStatus.Pending);
            foreach(var item in requests){
                _unitOfWork.RegistrationRequests.Delete(item);
            }

            Registration_Request req = await ToEnity(request);
            await _unitOfWork.RegistrationRequests.AddAsync(req);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task MarkStatus(int id, bool status)
        {
            Registration_Request? entity =
                await _unitOfWork.RegistrationRequests.GetByIdAsync(id);

            if (entity == null)
                throw new KeyNotFoundException("Registration request not found.");

            if (entity.status != RequestStatus.Pending)
                throw new InvalidOperationException(
                    "This request has already been processed.");

            if (status)
            {
                Member member = await RequestToMember(entity);

                var existingMember =
                    await _unitOfWork.Members.GetByIdAsync(member.Id);

                if (existingMember != null)
                    throw new InvalidOperationException(
                        "A member with this Id already exists.");

                await _unitOfWork.Members.AddAsync(member);
            }
            entity.status = status ? RequestStatus.Accepted : RequestStatus.Rejected;
            _unitOfWork.RegistrationRequests.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            Registration_Request? entity = await _unitOfWork.RegistrationRequests.GetByIdAsync(id);
            if (entity != null)
            {
                _unitOfWork.RegistrationRequests.Delete(entity);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task<PagedResponse<RegistrationResponseDto>> GetAllAsync(int pageNumber, int pageSize)
        {
            var result = await _unitOfWork.RegistrationRequests.GetPagedAsync(
                pageNumber,
                pageSize);

            return new PagedResponse<RegistrationResponseDto>
            {
                Items = result.Items.Select(ToDto),
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = result.TotalCount
            };
        }

        public async Task<RegistrationResponseDto?> GetByIdAsync(int id)
        {
            Registration_Request? entity = await _unitOfWork.RegistrationRequests.GetByIdAsync(id);
            return entity != null ? ToDto(entity) : null;
        }

        public async Task<PagedResponse<RegistrationResponseDto>> SearchAsync(SearchRequest request)
        {
            var specification = new SearchRequestSpecification(
                request.Id,
                request.FullName,
                request.Email,
                request.AcademicYear,
                request.Status,
                request.PageNumber,
                request.PageSize
            );

            var requests = await _unitOfWork.RegistrationRequests
                .ListAsync(specification);

            var totalCount = await _unitOfWork.RegistrationRequests
                .CountAsync(specification);

            return new PagedResponse<RegistrationResponseDto>
            {
                Items = requests.Select(ToDto).ToList(),
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = totalCount
            };
        }

        // Helper Methods
        private async Task<Registration_Request> ToEnity(RegistrationRequestDto dto)
        {
            Registration_Request ret = new();
            ret.AcademicYear = dto.AcademicYear;
            ret.Email = dto.Email;
            ret.FaceImage = await ConvertToBytesAsync(dto.FaceImage);
            ret.IdImage = await ConvertToBytesAsync(dto.IdImage);
            ret.FullName = dto.FullName;
            ret.MemberId = dto.MemberId;
            ret.SubmittedAt = DateTime.Now;

            return ret;
        }

        private RegistrationResponseDto ToDto(Registration_Request entity)
        {
            if (entity == null)
                return null;

            RegistrationResponseDto ret = new();

            ret.AcademicYear = entity.AcademicYear;
            ret.Email = entity.Email;
            ret.FaceImage = entity.FaceImage;
            ret.IdImage = entity.IdImage;
            ret.FullName = entity.FullName;
            ret.MemberId = entity.MemberId;
            ret.Id = entity.Id;
            ret.status = entity.status;
            ret.SubmittedAt = entity.SubmittedAt;

            return ret;
        }

        private async Task<byte[]> ConvertToBytesAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return Array.Empty<byte>();

            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            return memoryStream.ToArray();
        }

        private async Task<Member> RequestToMember(Registration_Request req)
        {
            if (req == null)
                return null;
            Member member = new();
            member.FaceImage = req.FaceImage;
            member.IdImage = req.IdImage;
            member.Id = req.MemberId;
            member.FullName = req.FullName;
            member.AcademicYear = req.AcademicYear;
            member.Email = req.Email;
            member.EmbeddingData = await _embeddingService.ExtractEmbeddingAsync(req.FaceImage);
            return member;
        }

      
    }
}
