using Application.DTOs;
using Application.DTOs.ResponseDtos;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
namespace Application.Services
{

    public class RegistrationRequestService : IRegistrationRequestService
    {

        private readonly IRegistrationRequestRepository _registrationRequestRepository;
        private readonly IEmbeddingService _embeddingService;
        private readonly IMemberService _memberService;

        public RegistrationRequestService(IRegistrationRequestRepository registrationRequestRepository, IEmbeddingService embeddingService, IMemberService memberService)
        {
            _registrationRequestRepository = registrationRequestRepository;
            _embeddingService = embeddingService;
            _memberService = memberService;
        }

        public async Task CreateAsync(RegistrationRequestDto request)
        {
            Registration_Request req =await ToEnity(request);
            await _registrationRequestRepository.AddAsync(req);
            await _registrationRequestRepository.SaveChangesAsync();
           
        }
        public async Task MarkStatus(int id,Boolean status)
        {
            Registration_Request ?entity =await _registrationRequestRepository.GetByIdAsync(id);
            if (entity != null)
            {
                entity.status = status ? RequestStatus.Accepted : RequestStatus.Rejected;
                _registrationRequestRepository.Update(entity);
                await _registrationRequestRepository.SaveChangesAsync();

                if (status == true)
                {
                    Member member = await RequestToMember(entity);
                    if (await _memberService.GetByIdAsync(member.Id) == null)    //avoid dublication
                        await _memberService.CreateAsync(member);
                }
            } 
        }

        public async Task DeleteAsync(int id)
        {
            Registration_Request ?entity =await _registrationRequestRepository.GetByIdAsync(id);
            if (entity != null)
            {
                _registrationRequestRepository.Delete(entity);
                await _registrationRequestRepository.SaveChangesAsync();
            }
        }

   
        public async Task<PagedResponse<RegistrationRequestDto>> GetAllAsync(int pageNumber, int pageSize)
        {
            var result = await _registrationRequestRepository.GetPagedAsync(
                pageNumber,
                pageSize);

            return new PagedResponse<RegistrationRequestDto>
            {
                Items = result.Items.Select(ToDto),
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = result.TotalCount
            };
        }


        public async Task<RegistrationRequestDto?> GetByIdAsync(int id)
        {
            Registration_Request? entity = await _registrationRequestRepository.GetByIdAsync(id);
            return entity != null ? ToDto(entity) : null;
        }

        //public async Task UpdateAsync(RegistrationRequestDto request)
        //{
        //    Registration_Request req = await ToEnity(request);

        //    _Repo.Update(req);
        //    await _Repo.SaveChangesAsync();
        //}







        //Helper Methods
        private async Task<Registration_Request> ToEnity(RegistrationRequestDto dto)
        {
            Registration_Request ret = new();
            ret.AcademicYear = dto.AcademicYear;
            ret.Email = dto.Email;
            ret.FaceImage =  await ConvertToBytesAsync(dto.FaceImage);
            ret.IdImage = await ConvertToBytesAsync(dto.IdImage);
            ret.FullName = dto.FullName;
            ret.MemberId = dto.MemberId;
            ret.SubmittedAt = DateTime.Now;

            return ret;
        }


        private RegistrationRequestDto ToDto(Registration_Request entity)
        {
            RegistrationRequestDto ret = new();

            ret.AcademicYear = entity.AcademicYear;
            ret.Email = entity.Email;
            ret.FaceImage = ConvertToFormFile(entity.FaceImage, "FaceImage.jpg");
            ret.IdImage = ConvertToFormFile(entity.IdImage, "IdImage.jpg");
            ret.FullName = entity.FullName;
            ret.MemberId = entity.MemberId;

            return ret;
        }

       
        private async Task<byte[]> ConvertToBytesAsync(IFormFile file)
        {
            using var memoryStream = new MemoryStream();

            await file.CopyToAsync(memoryStream);

            return memoryStream.ToArray();
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

        private async Task<Member> RequestToMember(Registration_Request req) {
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
