using Domain.Entities;
using Domain.Specifications;

public class VisitorWithLabsSpecification : BaseSpecification<Visitor>
{
    public VisitorWithLabsSpecification(int memberId)
        : base(m => m.MemberId == memberId)
    {
        AddInclude(m => m.Labs);
    }
}