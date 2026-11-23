using Application.DTOs.RequestDtos;
using Application.DTOs.ResponseDtos;
using Domain.Entities;
using Domain.Enums;
using Domain.Specifications;

using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Specifications
{
    public class SearchRequestSpecification : BaseSpecification<Registration_Request>
    {
        public SearchRequestSpecification(SearchRequestDto dto)
        {
            AddCriteria(m =>
                (!dto.Id.HasValue || m.Id == dto.Id.Value) &&

                (string.IsNullOrWhiteSpace(dto.FullName) ||
                 m.FullName.Contains(dto.FullName)) &&

                (string.IsNullOrWhiteSpace(dto.Email) ||
                 m.Email.Contains(dto.Email)) &&

                (!dto.AcademicYear.HasValue ||
                 m.AcademicYear == dto.AcademicYear.Value) &&

                 (!dto.Status.HasValue ||
                 m.status==dto.Status.Value)&&

                 (!dto.MemberId.HasValue ||
                 m.MemberId == dto.MemberId.Value)
            );

            AddOrderBy(m => m.Id);

            ApplyPaging(dto.PageNumber, dto.PageSize);
        }
    }
}
