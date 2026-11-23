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
            int cnt = await _unitOfWork.Repository<Member>().CountAsync(m => m.MemberId == request.MemberId);
            if (cnt > 0)
                return false;
            var requests = await _unitOfWork.Repository<Registration_Request>().GetAllAsync(r => r.MemberId == request.MemberId && r.status== RequestStatus.Pending);
            foreach(var item in requests){
                _unitOfWork.Repository<Registration_Request>().Delete(item);
            }

            Registration_Request req = await ToEnity(request);
            await _unitOfWork.Repository<Registration_Request>().AddAsync(req);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task MarkStatus(int id, bool status)
        {
            Registration_Request? entity =
                await _unitOfWork.Repository<Registration_Request>().GetByIdAsync(id);

            if (entity == null)
                throw new KeyNotFoundException($"Registration request with id {id} is not found.");

            if (entity.status != RequestStatus.Pending)
                throw new InvalidOperationException(
                    "This request has already been processed.");

            if (status)
            {

                var exists = await _unitOfWork.Repository<Member>().CountAsync(m => m.MemberId == entity.MemberId);
                if (exists > 0)
                    throw new InvalidOperationException($"A member with MemberId {entity.MemberId} already exists.");

                Member member = await RequestToMember(entity);
                await _unitOfWork.Repository<Member>().AddAsync(member);
            }
            entity.status = status ? RequestStatus.Accepted : RequestStatus.Rejected;
            _unitOfWork.Repository<Registration_Request>().Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            Registration_Request? entity = await _unitOfWork.Repository<Registration_Request>().GetByIdAsync(id);
            if (entity == null)
            {
                throw new KeyNotFoundException($"Registration request with id {id} is not found.");
            }
            _unitOfWork.Repository<Registration_Request>().Delete(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<PagedResponseDto<RegistrationResponseDto>> GetAllAsync(int pageNumber, int pageSize)
        {
            var result = await _unitOfWork.Repository<Registration_Request>().GetPagedAsync(
                pageNumber,
                pageSize);

            return new PagedResponseDto<RegistrationResponseDto>
            {
                Items = result.Items.Select(ToDto),
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = result.TotalCount
            };
        }

        public async Task<RegistrationResponseDto?> GetByIdAsync(int id)
        {
            Registration_Request? entity = await _unitOfWork.Repository<Registration_Request>().GetByIdAsync(id);
            if (entity == null)
            {
                throw new KeyNotFoundException($"Registration request with id {id} is not found.");

            }
            return entity != null ? ToDto(entity) : null;
        }

        public async Task<PagedResponseDto<RegistrationResponseDto>> SearchAsync(RegistrationRequestFilterDto request)
        {
            var specification = new RequestSpecification(request);


            var requests = await _unitOfWork.Repository<Registration_Request>()
                .ListAsync(specification);

            var totalCount = await _unitOfWork.Repository<Registration_Request>()
                .CountAsync(specification);

            return new PagedResponseDto<RegistrationResponseDto>
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
            member.MemberId = req.MemberId;
            member.FullName = req.FullName;
            member.AcademicYear = req.AcademicYear;
            member.Email = req.Email;
            member.EmbeddingData = await _embeddingService.ExtractEmbeddingAsync(req.FaceImage);
            return member;
        }

      
    }
}
