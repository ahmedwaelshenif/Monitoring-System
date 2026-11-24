using Application.DTOs.RequestDtos;
using Application.DTOs.ResponseDtos;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.Specifications;
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
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
        private readonly IEmailService _emailService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<RegistrationRequestService> _logger;

        public RegistrationRequestService(
            IEmbeddingService embeddingService,
            IUnitOfWork unitOfWork,
            IEmailService emailService,
            ILogger<RegistrationRequestService> logger
            )
        {
            _embeddingService = embeddingService;
            _unitOfWork = unitOfWork;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task CreateAsync(RegistrationRequestDto request)
        {
          
            bool exist = await _unitOfWork.Repository<Member>().AnyAsync(m => m.MemberId == request.MemberId);
            if (exist)
                throw new InvalidOperationException("This member already has access to the permitted labs.");

            var requests = await _unitOfWork.Repository<Registration_Request>().GetAllAsync(r => r.MemberId == request.MemberId && r.status== RequestStatus.Pending);
            foreach(var item in requests){
                _unitOfWork.Repository<Registration_Request>().Delete(item);
            }

            Registration_Request req = await ToEnity(request);
            await _unitOfWork.Repository<Registration_Request>().AddAsync(req);
            await _unitOfWork.SaveChangesAsync();
        }


        public async Task AcceptAsync(int id, List<int> LabIds)
        {
            Registration_Request? request = await _unitOfWork.Repository<Registration_Request>().GetByIdAsync(id);

            if (request == null)
                throw new KeyNotFoundException($"Registration request with id {id} is not found.");

            if (request.status != RequestStatus.Pending)
                throw new InvalidOperationException("This registration request has already been processed.");



            var exists = await _unitOfWork.Repository<Member>().AnyAsync(m => m.MemberId == request.MemberId);
            if (exists)
                throw new InvalidOperationException($"A member with MemberId {request.MemberId} already exists.");

            Member member = await RequestToMember(request, LabIds);
            request.status = RequestStatus.Accepted;
            await _unitOfWork.Repository<Member>().AddAsync(member);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task RejectAsync(int id)
        {
            Registration_Request? request = await _unitOfWork.Repository<Registration_Request>().GetByIdAsync(id);

            if (request == null)
                throw new KeyNotFoundException($"Registration request with id {id} is not found.");

            if (request.status != RequestStatus.Pending)
                throw new InvalidOperationException("This registration request has already been processed.");


            
            request.status = RequestStatus.Rejected;
            await _unitOfWork.SaveChangesAsync();
            try
            {
                if (status)
                {
                    await _emailService.SendEmailAsync(
                        entity.Email,
                        "Registration Approved",
                        $"""
                        <html>
                            <body>
                                <h2>Congratulations {entity.FullName}!</h2>
                                <p>Your registration request has been approved.</p>
                            </body>
                        </html>
                        """
                    );
                }
                else
                {
                    await _emailService.SendEmailAsync(
                        entity.Email,
                        "Registration Rejected",
                        $"""
                        <html>
                            <body>
                                <h2>Hello {entity.FullName},</h2>
                                <p>Your registration request for Student ID {entity.MemberId} has been rejected. Please submit a new request with clear and valid Information.</p>
                            </body>
                        </html>
                        """
                    );
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send registration email to {Email}", entity.Email);
            }
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



        private async Task<Member> RequestToMember(Registration_Request req,List<int> labIds)
        {
            ArgumentNullException.ThrowIfNull(req);

            if (labIds == null || !labIds.Any())
                throw new InvalidOperationException(
                    "At least one lab must be assigned.");

            var distinctLabIds = labIds.Distinct().ToList();

            var labs = await _unitOfWork.Repository<Lab>()
                .GetAllAsync(l => distinctLabIds.Contains(l.Id) && l.Active);

            if (labs.Count() != distinctLabIds.Count)
                throw new InvalidOperationException(
                    "One or more selected labs do not exist or are inactive.");

            Member member = new()
            {
                FaceImage = req.FaceImage,
                IdImage = req.IdImage,
                MemberId = req.MemberId,
                FullName = req.FullName,
                AcademicYear = req.AcademicYear,
                Email = req.Email,
                Labs = labs.ToList()
            };

            member.EmbeddingData =
                await _embeddingService.ExtractEmbeddingAsync(req.FaceImage);

            return member;
        }


    }
}
