using Application.DTOs.RequestDtos;
using Domain.Entities;
using Domain.Specifications;

namespace Application.Specifications
{
    public class EntryLogSpecification : BaseSpecification<EntryLog>
    {
        public EntryLogSpecification(EntryLogFilterDto filter)
        {
            AddCriteria(e =>
                (!filter.LabId.HasValue ||
                    e.LabId == filter.LabId.Value) &&

                (!filter.Status.HasValue ||
                    e.Status == filter.Status.Value) &&


                (!filter.MemberId.HasValue ||
                    e.Visitor.MemberId == filter.MemberId.Value) &&

                (!filter.StartDate.HasValue ||
                    e.EventTime >= filter.StartDate.Value) &&

                (!filter.EndDate.HasValue ||
                    e.EventTime <= filter.EndDate.Value) &&

                (string.IsNullOrEmpty(filter.FullName) ||
                    e.Visitor.FullName.Contains(filter.FullName)) &&

                (string.IsNullOrEmpty(filter.Email) ||
                    e.Visitor.Email.Contains(filter.Email)) &&

                (!filter.AcademicYear.HasValue ||
                    e.Visitor.AcademicYear == filter.AcademicYear.Value)
            );

            AddInclude(e => e.Lab);
            AddInclude(e => e.Visitor);

            AddOrderByDescending(e => e.EventTime);

            ApplyPaging(filter.PageNumber, filter.PageSize);
        }
    }
}