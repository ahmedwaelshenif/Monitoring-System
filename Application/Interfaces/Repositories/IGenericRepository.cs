
using Application.Common;
using Domain.Specifications;
using System.Linq.Expressions;

namespace Application.Interfaces.Repositories
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(int id);



        Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? criteria = null);

        Task<PagedResult<T>> GetPagedAsync(int pageNumber, int pageSize, Expression<Func<T, bool>>? criteria = null);

        Task AddAsync(T entity);

        void Update(T entity);

        void Delete(T entity);

        Task<int> SaveChangesAsync();


        Task<int> CountAsync(
            Expression<Func<T, bool>> ?criteria=null);

        Task<List<T>> ListAsync(
            ISpecification<T> specification);

        Task<int> CountAsync(
            ISpecification<T> specification);
    }
}
