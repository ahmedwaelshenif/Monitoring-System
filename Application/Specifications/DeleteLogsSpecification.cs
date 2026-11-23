using Application.DTOs.RequestDtos;
using Domain.Entities;
using Domain.Specifications;

namespace Application.Specifications
{
    public class DeleteLogsSpecification : BaseSpecification<EntryLog>
    {
        public DeleteLogsSpecification(EntryLogDeleteDto filter)
        {
            AddCriteria(e =>

                (!filter.StartDate.HasValue || e.EventTime >= filter.StartDate.Value) &&

                (!filter.EndDate.HasValue || e.EventTime <= filter.EndDate.Value)
            );

            AddOrderBy(e => e.EventTime);

            if (filter.Cnt.HasValue)
            {
                ApplyPaging(1, (int)filter.Cnt);
            }
        }
    }
}