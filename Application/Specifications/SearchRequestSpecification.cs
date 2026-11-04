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
        public SearchRequestSpecification(int? id, string? fullName, string? email, int? academicYear,RequestStatus? status, int pageNumber, int pageSize)
        {
            AddCriteria(m =>
                (!id.HasValue || m.Id == id.Value) &&

                (string.IsNullOrWhiteSpace(fullName) ||
                 m.FullName.Contains(fullName)) &&

                (string.IsNullOrWhiteSpace(email) ||
                 m.Email.Contains(email)) &&

                (!academicYear.HasValue ||
                 m.AcademicYear == academicYear.Value) &&

                 (!status.HasValue ||
                 m.status==status.Value)
            );

            AddOrderBy(m => m.Id);

            ApplyPaging(pageNumber, pageSize);
        }
    }
}
