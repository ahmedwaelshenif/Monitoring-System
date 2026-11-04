
using Application.Common;
using Domain.Specifications;
using System.Linq.Expressions;

namespace Application.Interfaces.Repositories
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(int id);

        Task<IEnumerable<T>> GetAllAsync();

        Task<IEnumerable<T>> GetAllAsync(
            Expression<Func<T, bool>> predicate);

        Task<PagedResult<T>> GetPagedAsync(
            int pageNumber, int pageSize);

        Task AddAsync(T entity);

        void Update(T entity);

        void Delete(T entity);

        Task<int> SaveChangesAsync();

        Task<int> CountAsync();

        Task<int> CountAsync(
            Expression<Func<T, bool>> predicate);

        Task<List<T>> ListAsync(
            ISpecification<T> specification);

        Task<int> CountAsync(
            ISpecification<T> specification);
    }
}
