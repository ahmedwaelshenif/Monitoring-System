using Application.DTOs.RequestDtos;
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
        public SearchAccountSpecification(SearchAccount dto)
        {
            AddCriteria(m =>

                (string.IsNullOrWhiteSpace(dto.Name) ||
                 m.FullName.Contains(dto.Name)) &&

                (string.IsNullOrWhiteSpace(dto.Email) ||
                 m.Email.Contains(dto.Email)) &&

                (!dto.role.HasValue ||
                 m.Role == dto.role.Value) &&

                 (!dto.IsActive.HasValue ||
                 m.IsActive == dto.IsActive.Value)
            );

            AddOrderBy(m => m.Id);

            ApplyPaging(dto.PageNumber, dto.PageSize);
        }
    }
}
