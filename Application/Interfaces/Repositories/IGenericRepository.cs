
using Application.Common;
using Domain.Specifications;
using System.Linq.Expressions;

namespace Application.Interfaces.Repositories
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(int id);



        Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? criteria = null);
        Task<IEnumerable<T>> GetAllTrackedAsync(Expression<Func<T, bool>>? criteria = null);

        Task<PagedResult<T>> GetPagedAsync(int pageNumber, int pageSize, Expression<Func<T, bool>>? criteria = null);

        Task AddAsync(T entity);

        void Update(T entity);

        void Delete(T entity);

        void DeleteRange(IEnumerable<T> entities);


        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);

        Task<int> SaveChangesAsync();


        Task<int> CountAsync(
            Expression<Func<T, bool>> ?criteria=null);

        Task<List<T>> ListAsync(
            ISpecification<T> specification);

        Task<int> CountAsync(
            ISpecification<T> specification);

        public Task<T?> GetEntityWithSpecAsync(
            ISpecification<T> specification);

        Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);
    }
}
