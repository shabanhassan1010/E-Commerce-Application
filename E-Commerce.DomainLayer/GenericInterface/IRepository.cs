
namespace E_Commerce.InfrastructureLayer.Data.DBContext.Repositories
{
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T?> GetByIdAsync(int id);
        void AddAsync(T Entity);
        void UpdateAsync(T entity);
        void DeleteAsync(T entity);
        Task<bool> SaveAsync();
    }
}
