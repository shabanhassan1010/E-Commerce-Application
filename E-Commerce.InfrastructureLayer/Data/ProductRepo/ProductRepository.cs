using E_Commerce.DomainLayer.Entities;
using E_Commerce.DomainLayer.Interfaces;
using E_Commerce.InfrastructureLayer.Data.DBContext;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.InfrastructureLayer.Data
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDBContext context;
        public ProductRepository(ApplicationDBContext context)
        {
            this.context = context;
        }
        public void AddProductAsync(Product Entity)
        {
            context.products.Add(Entity);
        }

        public void DeleteProductAsync(Product product)
        {
            context.products.Remove(product);
        }

        public async Task<IEnumerable<Product>> GetAllProductAsync()
        {
            return await context.products.ToListAsync();
        }

        public async Task<Product?> GetProductAsync(int id)
        {
            return await context.products.FindAsync(id);
        }

        public bool ProductExist(int id)
        {
            return context.products.Any(x=>x.Id == id);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await context.SaveChangesAsync() > 0;
        }

        public void UpdateProductAsync(Product entity)
        {
            context.Entry(entity).State = EntityState.Modified;
        }
    }
}
