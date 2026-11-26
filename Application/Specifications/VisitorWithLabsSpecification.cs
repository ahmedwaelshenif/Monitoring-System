using Domain.Entities;
using Domain.Specifications;

public class VisitorWithLabsSpecification : BaseSpecification<Visitor>
{
    public VisitorWithLabsSpecification(int visitorId)
        : base(visitor => visitor.Id == visitorId)
    {
        AddInclude(m => m.Labs);
    }
}
