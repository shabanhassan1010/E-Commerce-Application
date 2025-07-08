using E_Commerce.DomainLayer.Entities;
using E_Commerce.InfrastructureLayer;
using E_Commerce.InfrastructureLayer.Data.DBContext.Repositories;

namespace E_Commerce.DomainLayer.Interfaces
{
    public interface IProductRepository  : IGenericRepository<Product>
    {
        Task<IReadOnlyList<Product>> GetBrandsAsync(string brand);
        Task<IReadOnlyList<Product>> GetTypesAsync(string type);
        Task<IReadOnlyList<Product>> FilterProductsAsync(string? brand , string? type , string? sort);
        Task<PaginationResponse<Product>> GetProductsPagedAsync(int pageIndex, int pageSize);
        Task<IReadOnlyList<Product>> SearchProductsAsync(string? searchTerm);
        Task<PaginationResponse<Product>> FuzzySearchAsync(string query ,int page, int pageSize);

    }
}
