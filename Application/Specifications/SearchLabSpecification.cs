using Domain.Entities;
using Domain.Enums;
using Domain.Specifications;
using System;
using System.Collections.Generic;
using System.Text;
using Application.DTOs.RequestDtos;

namespace Application.Specifications
{
    public class SearchLabSpecification : BaseSpecification<Lab>
    {
        public SearchLabSpecification(SearchLabRequest dto)
        {
            AddCriteria(l =>

                (string.IsNullOrWhiteSpace(dto.Name) ||
                 l.Name.Contains(dto.Name)) &&



                 (!dto.Active.HasValue ||
                 l.Active == dto.Active.Value) &&


                 (!dto.Id.HasValue ||
                 l.Id == dto.Id.Value)
            );

            AddOrderBy(m => m.Id);

            ApplyPaging(dto.PageNumber, dto.PageSize);
        }
    }
}
