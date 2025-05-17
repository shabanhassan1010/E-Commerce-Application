using Microsoft.EntityFrameworkCore;

namespace E_Commerce.InfrastructureLayer.Data.DBContext.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        #region DB Context
        private readonly ApplicationDBContext context;
        private readonly DbSet<T> _dbSet;
        public Repository(ApplicationDBContext context)
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
        void IRepository<T>.AddAsync(T Entity)
        {
            _dbSet.Add(Entity);
        }
        void IRepository<T>.UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
        }
        public void DeleteAsync(T entity)
        {
            _dbSet.Remove(entity);
        }
        public async Task<bool> SaveAsync()
        {
            return await context.SaveChangesAsync() > 0;      
        }
    }
}
