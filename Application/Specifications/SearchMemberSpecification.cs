using Domain.Entities;
using Domain.Specifications;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Specifications
{
    public class SearchMemberSpecification:BaseSpecification<Member>
    {
        public SearchMemberSpecification(int? id,string? fullName,string? email,int? academicYear,int pageNumber,int pageSize)
        {
            AddCriteria(m =>
                (!id.HasValue || m.Id == id.Value) &&

                (string.IsNullOrWhiteSpace(fullName) ||
                 m.FullName.Contains(fullName)) &&

                (string.IsNullOrWhiteSpace(email) ||
                 m.Email.Contains(email)) &&

                (!academicYear.HasValue ||
                 m.AcademicYear == academicYear.Value)
            );

            AddOrderBy(m => m.Id);

            ApplyPaging(pageNumber, pageSize);
        }
    }
}

