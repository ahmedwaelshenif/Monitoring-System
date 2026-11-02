using Application.Common;
using System.Linq.Expressions;

namespace Application.Interfaces.Repositories
{

    public interface IGenericRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(int id);

        Task<IEnumerable<T>> GetAllAsync();

        Task<PagedResult<T>> GetPagedAsync(int pageNumber, int pageSize);

        Task AddAsync(T entity);

        void Update(T entity);

        void Delete(T entity);

        Task<int> SaveChangesAsync();
    }
}