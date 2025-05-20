using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace E_Commerce.InfrastructureLayer.Data.DBContext.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        #region DB Context
        private readonly ApplicationDBContext context;
        private readonly DbSet<T> _dbSet;
        public GenericRepository(ApplicationDBContext context)
        {
            this.context = context;
            _dbSet = context.Set<T>();
        }
        #endregion

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }
        public async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }
        async Task IGenericRepository<T>.AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }
        Task IGenericRepository<T>.UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            return Task.CompletedTask;
        }
        public Task DeleteAsync(T entity)
        {
           _dbSet.Remove(entity);
            return Task.CompletedTask;
        }
        public async Task<bool> SaveAsync()
        {
            return await context.SaveChangesAsync() > 0;   // return true -> if at least one row was modified on Database    
        }
    }
}
