#region MyRegion
using E_Commerce.DomainLayer.Entities;
using E_Commerce.DomainLayer.Interfaces;
using E_Commerce.InfrastructureLayer;
using Microsoft.AspNetCore.Mvc;
using System.Net;
#endregion

namespace E_Commerce_Application.Controllers
{
    public class ProductController : BaseApiController
    {
        #region DBContext
        private readonly IUnitOfWork unitOfWork;
        public ProductController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }
        #endregion

        #region GetAll
        [HttpGet("GetAll")]
        [EndpointSummary("Get All")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Product>))]
        [ProducesResponseType(400)]
        public async Task<ActionResult<IEnumerable<Product>>> GetAll()
        {
            var products = await unitOfWork.productRepository.GetAllAsync();
            return HandleResult(products, "Products retrieved successfully");
        }

        [HttpGet("FilterProductByBrandOrTypeOrPrice")]
        [EndpointSummary("Filter Product By Brand Or Type Or Price")]
        [ProducesResponseType(200, Type = typeof(IReadOnlyList<Product>))]
        [ProducesResponseType(400)]
        public async Task<ActionResult<IReadOnlyList<Product>>> FilterProductByBrandOrTypeOrPrice(string? brand, string? type, string? sort)
        {
            var products = await unitOfWork.productRepository.FilterProductByBrand(brand, type, sort);
            return HandleResult(products, "Products filtered successfully");
        }
        #endregion

        #region GetAllPaginated
        [HttpGet("GetAllPaginated")]
        [EndpointSummary("Get All Product Paginated")]
        [ProducesResponseType(200, Type = typeof(PaginationResponse<Product>))]
        [ProducesResponseType(400)]
        public async Task<ActionResult<PaginationResponse<Product>>> GetAllPaged(int page = 1, int pageSize = 10)
        {
            if (page < 1 || pageSize < 1)
                return HandleResult(false, "Page and pageSize must be greater than 0");

            var response = await unitOfWork.productRepository.GetProductsPagedAsync(page, pageSize);
            return HandleResult(response, "Products retrieved successfully");
        }
        #endregion

        #region GetProduct
        [HttpGet("GetProduct/{id:int}")]
        [EndpointSummary("Get Product")]
        [ProducesResponseType(200, Type = typeof(Product))]
        [ProducesResponseType(400)]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await unitOfWork.productRepository.GetByIdAsync(id);
            return HandleResult(product, "Product retrieved successfully");
        }
        #endregion

        #region CreateProduct
        [HttpPost("CreateProduct")]
        [EndpointSummary("Create Product")]
        [ProducesResponseType(200, Type = typeof(Product))]
        [ProducesResponseType(400)]
        public async Task<ActionResult<Product>> CreateProduct(Product product)
        {
            await unitOfWork.productRepository.AddAsync(product);
            var success = await unitOfWork.SaveAsync();
            
            if (success)
                return HandleResult(product, HttpStatusCode.Created, "Product created successfully");
            
            return HandleResult(false, "Failed to create product");
        }
        #endregion

        #region UpdateProduct
        [HttpPut("UpdateProduct/{id:int}")]
        [EndpointSummary("Update Product")]
        [ProducesResponseType(200, Type = typeof(Product))]
        [ProducesResponseType(400)]
        public async Task<ActionResult<Product>> UpdateProduct(int id, Product product)
        {
            if (id != product.Id)
                return HandleResult(false, "Product ID mismatch");

            var existingProduct = await unitOfWork.productRepository.GetByIdAsync(id);
            if (existingProduct == null)
                return HandleResult(false, "Product not found");

            // Update the existing product's properties
            existingProduct.Name = product.Name;
            existingProduct.Description = product.Description;
            existingProduct.Price = product.Price;
            existingProduct.PictureUrl = product.PictureUrl;
            existingProduct.Type = product.Type;
            existingProduct.Brand = product.Brand;
            existingProduct.QuantityInStock = product.QuantityInStock;

            await unitOfWork.productRepository.UpdateAsync(existingProduct);
            var success = await unitOfWork.SaveAsync();

            if (success)
                return HandleResult(existingProduct, "Product updated successfully");

            return HandleResult(false, "Failed to update product");
        }
        #endregion

        #region Get Product Brands
        [HttpGet("GetBrands")]
        [EndpointSummary("Get Product Brands")]
        [ProducesResponseType(200, Type = typeof(string))]
        [ProducesResponseType(400)]
        public async Task<ActionResult<IReadOnlyList<string>>> GetBrands()
        {
            return Ok(await unitOfWork.productRepository.GetBrandsAsync());
        }
        #endregion

        #region Get Product Types
        [HttpGet("GetTypes")]
        [EndpointSummary("Get Product Types")]
        [ProducesResponseType(200, Type = typeof(string))]
        [ProducesResponseType(400)]
        public async Task<ActionResult<IReadOnlyList<string>>> GetTypes()
        {
            return Ok(await unitOfWork.productRepository.GetTypesAsync());
        }
        #endregion

        #region DeleteProduct
        [HttpDelete("DeleteProduct/{id:int}")]
        [EndpointSummary("Delete Product")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult> DeleteProduct(int id)
        {
            var product = await unitOfWork.productRepository.GetByIdAsync(id);
            if (product == null)
                return HandleResult(false, "Product not found");

            await unitOfWork.productRepository.DeleteAsync(product);
            var success = await unitOfWork.SaveAsync();

            if (success)
                return HandleResult(true, "Product deleted successfully");

            return HandleResult(false, "Failed to delete product");
        }
        #endregion
    }
}