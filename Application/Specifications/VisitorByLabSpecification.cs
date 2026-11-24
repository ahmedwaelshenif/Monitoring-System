using DocumentFormat.OpenXml.Wordprocessing;
using Domain.Entities;
using Domain.Specifications;

namespace Application.Specifications
{
    public class VisitorByLabSpecification : BaseSpecification<Visitor>
    {
        public VisitorByLabSpecification(int labId,int pageNumber, int pageSize)
        {
            AddCriteria(v => v.Labs.Any(l => l.Id == labId));

            AddInclude(v => v.Labs);
            ApplyPaging(pageNumber, pageSize);
        }
    }
}