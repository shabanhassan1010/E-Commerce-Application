#region
using AutoMapper;
using Azure;
using E_Commerce.ApplicationLayer.Dtos.Product.Read;
using E_Commerce.ApplicationLayer.Dtos.Product.Update;
using E_Commerce.ApplicationLayer.Dtos.Product.Write;
using E_Commerce.ApplicationLayer.IService;
using E_Commerce.DomainLayer.Entities;
using E_Commerce.DomainLayer.Interfaces;
using E_Commerce.InfrastructureLayer;
using Serilog;
#endregion

namespace E_Commerce.ApplicationLayer.Service
{
    public class ProductService :IProductService
    {
        #region
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper _mapper;
        public ProductService(IUnitOfWork unitOfWork , IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        #endregion

        public async Task<IEnumerable<GetProductDto>> GetAllProductAsync()
        {
            var products = await unitOfWork.productRepository.GetAllAsync();
            var mapper = _mapper.Map<IEnumerable<GetProductDto>>(products);
            Log.Information("GetAllProductAsync");
            return mapper;
        }
        public async Task<PaginationResponse<GetProductDto>> GetAllPaginatedAsync(int page = 1, int pageSize = 10)
        {
            var paginatedProducts = await unitOfWork.productRepository.GetProductsPagedAsync(page, pageSize);

            var mappedProducts = _mapper.Map<IEnumerable<GetProductDto>>(paginatedProducts.Data);

            return new PaginationResponse<GetProductDto>(
                paginatedProducts.PageIndex,
                paginatedProducts.PageSize,
                paginatedProducts.TotalItems,
                mappedProducts
            );
        }
        public async Task<GetProductDto> GetProductAsync(int Id)
        {
            var Product = await unitOfWork.productRepository.GetByIdAsync(Id);
            if (Product == null)
                return null;
            var mapping = _mapper.Map<GetProductDto>(Product);

            Log.Information($"GetByIdAsync{Id}");

            return mapping;
        }
        public async Task<IEnumerable<GetProductDto>> GetProductsByBrandAsync(string brand)
        {
            var brands = await unitOfWork.productRepository.GetBrandsAsync(brand);
            var mapping = _mapper.Map<IEnumerable<GetProductDto>>(brands);
            return mapping;
        }
        public async Task<IEnumerable<GetProductDto>> GetProductsByTypeAsync(string type)
        {
            var Types = await unitOfWork.productRepository.GetTypesAsync(type);
            var mapping = _mapper.Map<IEnumerable<GetProductDto>>(Types);
            return mapping;
        }
        public async Task<GetProductDto> DeleteProductAsync(int Id)
        {
            if (Id == 0)    return null;

            var product = await unitOfWork.productRepository.GetByIdAsync(Id);
            if (product == null)
                return null;

            await unitOfWork.productRepository.DeleteAsync(product);
            await unitOfWork.SaveAsync();

            var mapping = _mapper.Map<GetProductDto>(product);
            return mapping;
        }
        public async Task<GetProductDto> CreateProduct(CreateProductDto createProductDto)
        {
            var mapping = _mapper.Map<Product>(createProductDto);

            await unitOfWork.productRepository.AddAsync(mapping);

            var success = await unitOfWork.SaveAsync();
            if(!success) return null;

            return _mapper.Map<GetProductDto>(mapping);
        }
        public async Task<GetProductDto> UpdateProduct(UpdateProductDto updateProductDto, int Id)
        {
            var product = await unitOfWork.productRepository.GetByIdAsync(Id);
            if (product == null) return null;

            _mapper.Map(updateProductDto, product);

            await unitOfWork.productRepository.UpdateAsync(product);
            await unitOfWork.SaveAsync();

            return _mapper.Map<GetProductDto>(product);
        }
        public async Task<IEnumerable<GetProductDto>> FilterProductBasedAsync(string? brand, string? type, string? sort)
        {
            var product = await unitOfWork.productRepository.FilterProductsAsync(brand, type, sort);
            if (product == null || !product.Any())
                return null;
            var mapping = _mapper.Map<IEnumerable<GetProductDto>>(product);
            return mapping;
        }
        public async Task<IEnumerable<GetProductDto>> SearchForProductAsync(string? searchTerm = null)
        {
            var product = await unitOfWork.productRepository.SearchProductsAsync(searchTerm);
            if (product == null || !product.Any())
                return null;

            var mapping = _mapper.Map<IEnumerable<GetProductDto>>(product);
            return mapping;
        }
        public async Task<PaginationResponse<GetProductDto>> FuzzySearchProductsAsync(string query, int page, int pageSize)
        {
            var products = await unitOfWork.productRepository.FuzzySearchAsync(query, page, pageSize);
            var mappedData = _mapper.Map<IEnumerable<GetProductDto>>(products.Data);

            return new PaginationResponse<GetProductDto>(
                products.PageIndex,
                products.PageSize,
                products.TotalItems,
                mappedData
            );
        }
    }
}
