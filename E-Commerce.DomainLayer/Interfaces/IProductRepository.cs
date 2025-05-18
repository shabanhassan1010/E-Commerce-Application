using E_Commerce.DomainLayer.Entities;
using E_Commerce.InfrastructureLayer;
using E_Commerce.InfrastructureLayer.Data.DBContext.Repositories;

namespace E_Commerce.DomainLayer.Interfaces
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<IReadOnlyList<string>> GetBrandsAsync();
        Task<IReadOnlyList<string>> GetTypesAsync();
        Task<IReadOnlyList<Product>> FilterProductByBrand(string brand , string type);
        Task<PaginationResponse<Product>> GetProductsPagedAsync(int pageIndex, int pageSize);
    }
}
