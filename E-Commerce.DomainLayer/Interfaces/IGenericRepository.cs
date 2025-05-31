
using System.Linq.Expressions;

namespace E_Commerce.InfrastructureLayer.Data.DBContext.Repositories
{
    public interface IGenericRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        IReadOnlySet<T> FindAsync(Expression<Func<T, bool>> query);
        Task<T?> GetByIdAsync(int id);
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);
        
    }
}
