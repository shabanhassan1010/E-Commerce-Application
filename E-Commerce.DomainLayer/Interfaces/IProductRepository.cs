using E_Commerce.DomainLayer.Entities;

namespace E_Commerce.DomainLayer.Interfaces
{
    public interface IProductRepository 
    {
        Task<IEnumerable<Product>> GetAllProductAsync();
        Task<Product?> GetProductAsync(int id);
        void UpdateProductAsync(Product entity);
        void AddProductAsync(Product Entity);
        void DeleteProductAsync(Product product);
        bool ProductExist(int id);
        Task<bool> SaveChangesAsync();
    }
}
