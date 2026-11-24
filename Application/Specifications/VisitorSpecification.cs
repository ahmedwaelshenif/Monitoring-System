using Domain.Entities;
using Domain.Specifications;
using System;
using System.Collections.Generic;
using System.Text;
using Application.DTOs.RequestDtos;

namespace Application.Specifications
{
    public class VisitorSpecification:BaseSpecification<Visitor>
    {
        public VisitorSpecification(VisitorFilterDto dto)
        {
            AddCriteria(m =>
                (!dto.Id.HasValue || m.Id == dto.Id.Value) &&

                (string.IsNullOrWhiteSpace(dto.FullName) ||
                 m.FullName.Contains(dto.FullName)) &&

                (string.IsNullOrWhiteSpace(dto.Email) ||
                 m.Email.Contains(dto.Email)) &&

                (!dto.AcademicYear.HasValue ||
                 m.AcademicYear == dto.AcademicYear.Value) &&


                (!dto.Active.HasValue ||
                 m.Active == dto.Active.Value)&&

                (!dto.MemberId.HasValue ||
                 m.MemberId == dto.MemberId.Value)
            );

            AddOrderBy(m => m.Id);

            ApplyPaging(dto.PageNumber, dto.PageSize);
        }
    }
}

