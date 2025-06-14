#region MyRegion
using E_Commerce.DomainLayer.Entities;
using E_Commerce.DomainLayer.Interfaces;
using E_Commerce.InfrastructureLayer;
using Microsoft.AspNetCore.Mvc;
using System.Net;
#endregion

namespace E_Commerce_Application.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
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
        [EndpointSummary("Get All Products")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Product>))]
        [ProducesResponseType(404)]
        public async Task<ActionResult<IEnumerable<Product>>> GetAll()
        {
            var products = await unitOfWork.productRepository.GetAllAsync();
            if (products == null || !products.Any())
                return NotFound("No products found");

            return Ok(products);
        }

        [HttpGet("FilterProductByBrandOrTypeOrPrice")]
        [EndpointSummary("Filter Product By Brand Or Type Or Price")]
        [ProducesResponseType(200, Type = typeof(IReadOnlyList<Product>))]
        [ProducesResponseType(404)]
        public async Task<ActionResult<IReadOnlyList<Product>>> FilterProductByBrandOrTypeOrPrice(
            [FromQuery] string? brand, 
            [FromQuery] string? type, 
            [FromQuery] string? sort)
        {
            var products = await unitOfWork.productRepository.FilterProductByBrand(brand, type, sort);
            if (products == null || !products.Any())
                return NotFound("No products found matching the criteria");

            return Ok(products);
        }
        #endregion

        #region GetAllPaginated
        [HttpGet("GetAllPaginated")]
        [EndpointSummary("Get All Product Paginated")]
        [ProducesResponseType(200, Type = typeof(PaginationResponse<Product>))]
        [ProducesResponseType(400)]
        public async Task<ActionResult<PaginationResponse<Product>>> GetAllPaginated(
            [FromQuery] int page = 1, 
            [FromQuery] int pageSize = 10)
        {
            if (page < 1 || pageSize < 1)
                return BadRequest("Page and pageSize must be greater than 0");

            var response = await unitOfWork.productRepository.GetProductsPagedAsync(page, pageSize);
            if (response == null)
                return NotFound("No products found");

            return Ok(response);
        }
        #endregion

        #region GetProduct
        [HttpGet("GetProduct/{id:int}")]
        [EndpointSummary("Get Product by ID")]
        [ProducesResponseType(200, Type = typeof(Product))]
        [ProducesResponseType(404)]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await unitOfWork.productRepository.GetByIdAsync(id);
            if (product == null)
                return NotFound($"Product with ID {id} not found");

            return Ok(product);
        }
        #endregion

        #region CreateProduct
        [HttpPost("CreateProduct")]
        [EndpointSummary("Create New Product")]
        [ProducesResponseType(201, Type = typeof(Product))]
        [ProducesResponseType(400)]
        public async Task<ActionResult<Product>> CreateProduct([FromBody] Product product)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(errors);
            }

            await unitOfWork.productRepository.AddAsync(product);
            var success = await unitOfWork.SaveAsync();
            
            if (success)
                return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);

            return BadRequest("Failed to create product");
        }
        #endregion

        #region UpdateProduct
        [HttpPut("UpdateProduct/{id:int}")]
        [EndpointSummary("Update Existing Product")]
        [ProducesResponseType(200, Type = typeof(Product))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<Product>> UpdateProduct(int id, [FromBody] Product product)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            if (id != product.Id)
                return BadRequest("Product ID mismatch");

            var existingProduct = await unitOfWork.productRepository.GetByIdAsync(id);
            if (existingProduct == null)
                return NotFound($"Product with ID {id} not found");

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
                return Ok(existingProduct);

            return BadRequest("Failed to update product");
        }
        #endregion

        #region Get Product Brands
        [HttpGet("GetBrands")]
        [EndpointSummary("Get All Product Brands")]
        [ProducesResponseType(200, Type = typeof(IReadOnlyList<string>))]
        [ProducesResponseType(404)]
        public async Task<ActionResult<IReadOnlyList<string>>> GetBrands()
        {
            var brands = await unitOfWork.productRepository.GetBrandsAsync();
            if (brands == null || !brands.Any())
                return NotFound("No brands found");

            return Ok(brands);
        }
        #endregion

        #region Get Product Types
        [HttpGet("GetTypes")]
        [EndpointSummary("Get All Product Types")]
        [ProducesResponseType(200, Type = typeof(IReadOnlyList<string>))]
        [ProducesResponseType(404)]
        public async Task<ActionResult<IReadOnlyList<string>>> GetTypes()
        {
            var types = await unitOfWork.productRepository.GetTypesAsync();
            if (types == null || !types.Any())
                return NotFound("No types found");

            return Ok(types);
        }
        #endregion

        #region DeleteProduct
        [HttpDelete("DeleteProduct/{id:int}")]
        [EndpointSummary("Delete Product")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await unitOfWork.productRepository.GetByIdAsync(id);
            if (product == null)
                return NotFound($"Product with ID {id} not found");

            await unitOfWork.productRepository.DeleteAsync(product);
            var success = await unitOfWork.SaveAsync();

            if (success)
                return Ok();

            return BadRequest("Failed to delete product");
        }
        #endregion

        #region SearchProducts
        [HttpGet("Search")]
        [EndpointSummary("Search Products")]
        [ProducesResponseType(200, Type = typeof(IReadOnlyList<Product>))]
        [ProducesResponseType(404)]
        public async Task<ActionResult<IReadOnlyList<Product>>> SearchProducts([FromQuery] string? searchTerm = null)
        {
            var products = await unitOfWork.productRepository.SearchProductsAsync(searchTerm);
            if (products == null || !products.Any())
                return NotFound("No products found matching the search criteria");

            return Ok(products);
        }
        #endregion
    }
}