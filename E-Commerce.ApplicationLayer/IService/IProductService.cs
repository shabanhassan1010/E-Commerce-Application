using E_Commerce.ApplicationLayer.Dtos.Product.Read;
using E_Commerce.ApplicationLayer.Dtos.Product.Update;
using E_Commerce.ApplicationLayer.Dtos.Product.Write;
using E_Commerce.InfrastructureLayer;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.ApplicationLayer.IService
{
    public interface IProductService 
    {
        Task<IEnumerable<GetProductDto>> GetAllProductAsync();
        Task<PaginationResponse<GetProductDto>> GetAllPaginatedAsync(int page = 1, int pageSize = 10);
        Task<GetProductDto> GetProductAsync(int Id);
        Task<GetProductDto> DeleteProductAsync(int Id);
        Task<IEnumerable<GetProductDto>> GetProductsByBrandAsync(string brand);
        Task<IEnumerable<GetProductDto>> GetProductsByTypeAsync(string type);
        Task<GetProductDto> CreateProduct(CreateProductDto createProductDto);
        Task<GetProductDto> UpdateProduct(UpdateProductDto updateProductDto , int Id);
        Task<IEnumerable<GetProductDto>> FilterProductBasedAsync(string? brand,string? type, string? sort);
        Task<IEnumerable<GetProductDto>> SearchForProductAsync(string? searchTerm = null);
        Task<PaginationResponse<GetProductDto>> FuzzySearchProductsAsync(string query , int page, int pageSize);

    }
}
