#region MyRegion
using E_Commerce.DomainLayer.Entities;
using E_Commerce.DomainLayer.Interfaces;
using E_Commerce.InfrastructureLayer;
using Microsoft.AspNetCore.Mvc;
#endregion

namespace E_Commerce_Application.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
        [EndpointSummary("Get All")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Product>))]
        [ProducesResponseType(400)]
        public async Task<ActionResult<IEnumerable<Product>>> GetAll()
        {
            return Ok(await unitOfWork.productRepository.GetAllAsync());
        }

        [HttpGet("FilterProductByBrandOrTypeOrPrice")]
        [EndpointSummary("Filter Product By Brand Or Type Or Price")]
        [ProducesResponseType(200, Type = typeof(IReadOnlyList<Product>))]
        [ProducesResponseType(400)]
        public async Task<ActionResult<IReadOnlyList<Product>>> FilterProductByBrandOrTypeOrPrice(string? brand , string? type , string? sort)
        {
            return Ok(await unitOfWork.productRepository.FilterProductByBrand(brand ,type , sort));
        }
        #endregion

        #region GetAllPaginated
        [HttpGet("GetAllPaginated")]
        [EndpointSummary("Get All Product Paginated")]
        [ProducesResponseType(200, Type = typeof(PaginationResponse<Product>))]
        [ProducesResponseType(400)]
        public async Task<ActionResult<PaginationResponse<Product>>> GetAllPaged(int page = 1, int pageSize = 10)
        {
            // Validate input
            if (page < 1 || pageSize < 1)
                return BadRequest("Page and pageSize must be greater than 0.");

            var response = await unitOfWork.productRepository.GetProductsPagedAsync(page, pageSize);
            return Ok(response);
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
            if (product == null)
                return NotFound();
            return Ok(product);
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
            if (await unitOfWork.productRepository.SaveAsync())
            {
                return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
            }
            return BadRequest("Can Not Create Product");
        }
        #endregion

        #region Update Product
        [HttpPut("UpdateProduct/{id:int}")]
        [EndpointSummary("Update Product")]
        [ProducesResponseType(200, Type = typeof(Product))]
        [ProducesResponseType(400)]
        public async Task<ActionResult<Product>> UpdateProduct(Product product, int id)
        {
            if (id != product.Id)
                return BadRequest("Can not Update this product");
            await unitOfWork.productRepository.UpdateAsync(product);
            if (await unitOfWork.productRepository.SaveAsync())  // if the product Already Updated And this update save in DB -> Done
            {
                return NoContent();
            }
            return BadRequest("Problem in Updating In product");

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

        #region Delete Product
        [HttpDelete("DeleteProduct/{id:int}")]
        [EndpointSummary("Delete Product")]
        [ProducesResponseType(200, Type = typeof(Product))]
        [ProducesResponseType(400)]
        public async Task<ActionResult<Product>> DeleteProduct(int id)
        {
            var Product = await unitOfWork.productRepository.GetByIdAsync(id);
            if (Product == null)
            {
                return NotFound();
            }

            await unitOfWork.productRepository.DeleteAsync(Product);
            if (await unitOfWork.productRepository.SaveAsync())
            {
                return NoContent();
            }
            return BadRequest("Problem in Deleting this product");
        }
        #endregion
    }
}