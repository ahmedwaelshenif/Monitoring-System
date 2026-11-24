using Domain.Entities;
using Domain.Specifications;

public class LabWithVisitorsSpecification : BaseSpecification<Lab>
{
    public LabWithVisitorsSpecification(int labId)
        : base(l => l.Id == labId)
    {
        AddInclude(l => l.Visitors);
    }
}