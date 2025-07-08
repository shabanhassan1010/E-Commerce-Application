using E_Commerce.ApplicationLayer.Dtos.Product.Read;
using E_Commerce.ApplicationLayer.Dtos.Product.Update;
using E_Commerce.ApplicationLayer.Dtos.Product.Write;
using E_Commerce.ApplicationLayer.IService;
using E_Commerce.DomainLayer.Entities;
using E_Commerce.InfrastructureLayer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace E_Commerce_Application.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        #region DBContext
        private readonly IProductService _productService;
        private readonly ILogger<ProductController> logger;

        public ProductController(IProductService productService, ILogger<ProductController> logger)
        {
            _productService = productService;
            this.logger = logger;
        }
        #endregion

        protected string? GetById()
        {
            return User?.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        #region GetAll
        [HttpGet("GetAll")]
        //[Authorize(Policy = "CustomerPolicy")]
        [EndpointSummary("Get All Products")]
        public async Task<ActionResult<IEnumerable<GetProductDto>>> GetAll()
        {
            //var userId = GetById();
            //if (string.IsNullOrEmpty(userId))
            //    return Unauthorized();

            var products = await _productService.GetAllProductAsync();
            if (products == null || !products.Any())
                return NotFound("No products found");

            return Ok(products);
        }
        #endregion

        #region GetAllPaginated
        [HttpGet("GetAllPaginated")]
        //[Authorize(Policy = "CustomerPolicy")]
        [EndpointSummary("Get All Product Paginated")]
        public async Task<ActionResult<PaginationResponse<Product>>> GetAllPaginated([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            if (page < 1 || pageSize < 1)
                return BadRequest("Page and pageSize must be greater than 0");
            logger.LogInformation("Trying to get All products");
            var response = await _productService.GetAllPaginatedAsync(page, pageSize);

            if (response == null)
            {
                logger.LogWarning("Something was happened when you get All products ");
                return NotFound("No products found");
            }

            return Ok(response);
        }
        #endregion

        #region GetProduct
        [HttpGet("GetProduct/{id:int}")]
        //[Authorize(Policy = "CustomerPolicy")]
        [EndpointSummary("Get Product by ID")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            logger.LogInformation("Trying to get product with ID: {Id}", id);
            var product = await _productService.GetProductAsync(id);
            if (product == null)
            {
                logger.LogWarning("Product with ID: {Id} not found", id);
                return NotFound($"Product with ID {id} not found");
            }

            return Ok(product);
        }
        #endregion

        #region  FilterProductByBrandOrTypeOrPrice
        [HttpGet("FilterProductByBrandOrTypeOrPrice")]
        [Authorize(Policy = "CustomerPolicy")]
        [EndpointSummary("Filter Product By Brand Or Type Or Price")]
        public async Task<ActionResult<IReadOnlyList<GetProductDto>>> FilterProductByBrandOrTypeOrPrice([FromQuery] string? brand,
                             [FromQuery] string? type, [FromQuery] string? sort)
        {
            var userId = GetById();
            if (string.IsNullOrEmpty(userId))
            {
                logger.LogWarning("Unauthorized access attempt to FilterProductByBrandOrTypeOrPrice.");
                return Unauthorized();
            }
            logger.LogInformation("User {UserId} is filtering products. Brand: {Brand}, Type: {Type}, Sort: {Sort}", userId, brand, type, sort);

            var products = await _productService.FilterProductBasedAsync(brand, type, sort);
            if (products == null || !products.Any())
            {
                logger.LogWarning("No products found for User {UserId} with Brand: {Brand}, Type: {Type}, Sort: {Sort}", userId, brand, type, sort);
                return NotFound("No products found matching the criteria");
            }

            return Ok(products);
        }
        #endregion

        #region CreateProduct
        [HttpPost("CreateProduct")]
        [Authorize(Policy = "AdminPolicy")]
        [EndpointSummary("Create New Product")]
        public async Task<ActionResult<GetProductDto>> CreateProduct(CreateProductDto createProductDto)
        {
            var userId = GetById();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var createdProduct = await _productService.CreateProduct(createProductDto);

            if (createdProduct == null)
                return BadRequest("Failed to create product");

            return CreatedAtAction(nameof(GetProduct), new { id = createdProduct.Id }, createdProduct);

        }
        #endregion

        #region UpdateProduct
        [HttpPut("UpdateProduct/{id:int}")]
        [Authorize(Policy = "AdminPolicy")]
        [EndpointSummary("Update Existing Product")]
        public async Task<ActionResult<GetProductDto>> UpdateProduct(int id, UpdateProductDto updateProductDto)
        {
            var userId = GetById();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var product = await _productService.UpdateProduct(updateProductDto, id);
            if (product == null)
                return NotFound("This Product Not Found");

            return Ok(product);

        }
        #endregion

        #region Get Product Brands
        [HttpGet("GetBrands")]
        [Authorize(Policy = "CustomerPolicy")]
        [EndpointSummary("Get All Brands")]

        public async Task<ActionResult<IReadOnlyList<GetProductDto>>> GetBrands(string brand)
        {
            var userId = GetById();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var brands = await _productService.GetProductsByBrandAsync(brand);
            if (brands == null || !brands.Any())
                return NotFound("No brands found");

            return Ok(brands);
        }
        #endregion

        #region Get Product Types
        [HttpGet("GetTypes")]
        [Authorize(Policy = "CustomerPolicy")]
        [EndpointSummary("Get All Types")]
        public async Task<ActionResult<IReadOnlyList<GetProductDto>>> GetTypes(string type)
        {
            var userId = GetById();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var types = await _productService.GetProductsByTypeAsync(type);
            if (types == null || !types.Any())
                return NotFound("No types found");

            return Ok(types);
        }
        #endregion

        #region DeleteProduct
        [HttpDelete("DeleteProduct/{id:int}")]
        [Authorize(Policy = "AdminPolicy")]
        [EndpointSummary("Delete Product using Id")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var userId = GetById();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var product = await _productService.DeleteProductAsync(id);
            if (product == null || id == 0)
                return NotFound($"Product with ID {id} not found");

            return Ok(new
            {
                message = "Product deleted successfully",
                product = product
            });
        }
        #endregion

        #region SearchProducts
        [HttpGet("Search")]
        [Authorize(Policy = "CustomerPolicy")]
        [EndpointSummary("Search For Product")]
        public async Task<ActionResult<IReadOnlyList<GetProductDto>>> SearchProducts([FromQuery] string? searchTerm = null)
        {
            var products = await _productService.SearchForProductAsync(searchTerm);
            if (products == null || !products.Any())
                return NotFound("No products found matching the search criteria");

            return Ok(products);
        }
        #endregion

        #region Fuzzy Search 

        [HttpGet("FuzzySearch")]
        [Authorize(Policy = "CustomerPolicy")]
        [EndpointSummary("Fuzzy search by product name with pagination")]
        public async Task<IActionResult> FuzzySearch([FromQuery] string query, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var user = GetById();
            if (string.IsNullOrEmpty(user))
                return Unauthorized();

            if (string.IsNullOrWhiteSpace(query))
                return BadRequest("Search query cannot be empty");

            var products = await _productService.FuzzySearchProductsAsync(query, page, pageSize);

            if (!products.Data.Any())
                return NotFound("No matching products found");

            return Ok(products);
        }
        #endregion
    }
}
