using Domain.Entities;
using Domain.Enums;
using Domain.Specifications;

namespace Application.Specifications
{
    public class AttendanceExportSpecification : BaseSpecification<EntryLog>
    {
        public AttendanceExportSpecification(DateTime fromDate, DateTime toDate, int? labId)
        {
            AddCriteria(l =>
                l.EventTime >= fromDate &&
                l.EventTime <= toDate &&
                (!labId.HasValue || l.LabId == labId.Value) &&
                l.Status == EntryStatus.Accepted
            );

            // Eagerly load Member and Lab in SQL:
            AddInclude(l => l.Visitor);
            AddInclude(l => l.Lab);

            AddOrderBy(l => l.EventTime);
        }
    }
}