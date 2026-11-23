using Application.DTOs.RequestDtos;
using Domain.Entities;
using Domain.Specifications;

namespace Application.Specifications
{
    public class WarningSpecification : BaseSpecification<Warning>
    {
        public WarningSpecification(WarningFilterDto filter)
        {
            AddCriteria(w =>
           (!filter.LabId.HasValue || w.LabId == filter.LabId.Value) &&
           (!filter.Warning.HasValue || w.warning == filter.Warning.Value) &&
           (string.IsNullOrWhiteSpace(filter.LabName) ||
            (w.Lab != null && w.Lab.Name.Contains(filter.LabName)))
        );

            AddInclude(w => w.Lab);

            ApplyPaging(filter.PageNumber, filter.PageSize);
        }
    }
}