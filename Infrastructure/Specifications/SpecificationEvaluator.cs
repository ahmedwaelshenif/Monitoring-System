
using Domain.Specifications;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Specifications;

public static class SpecificationEvaluator
{
    public static IQueryable<T> GetQuery<T>(IQueryable<T> inputQuery,ISpecification<T> specification,bool applyPaging = true)
        where T : class
    {
        var query = inputQuery;

        // WHERE
        if (specification.Criteria != null)
        {
            query = query.Where(specification.Criteria);
        }

        // INCLUDE
        query = specification.Includes.Aggregate(
            query,
            (current, include) => current.Include(include));

        // ORDER BY
        if (specification.OrderBy != null)
        {
            query = query.OrderBy(specification.OrderBy);
        }
        else if (specification.OrderByDescending != null)
        {
            query = query.OrderByDescending(
                specification.OrderByDescending);
        }

        // PAGINATION
        if (applyPaging)
        {
            if (specification.Skip.HasValue)
            {
                query = query.Skip(specification.Skip.Value);
            }

            if (specification.Take.HasValue)
            {
                query = query.Take(specification.Take.Value);
            }
        }

        return query;
    }
}
