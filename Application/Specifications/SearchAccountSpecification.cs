using Domain.Entities;
using Domain.Enums;
using Domain.Specifications;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Specifications
{
    public class SearchAccountSpecification: BaseSpecification<DashboardAccount>
    {
        public SearchAccountSpecification(string?name, string email, Role? role, bool?status, int pageNumber, int pageSize)
        {
            AddCriteria(m =>

                (string.IsNullOrWhiteSpace(name) ||
                 m.FullName.Contains(name)) &&

                (string.IsNullOrWhiteSpace(email) ||
                 m.Email.Contains(email)) &&

                (!role.HasValue ||
                 m.Role == role.Value) &&

                 (!status.HasValue ||
                 m.IsActive == status.Value)
            );

            AddOrderBy(m => m.Id);

            ApplyPaging(pageNumber, pageSize);
        }
    }
}
