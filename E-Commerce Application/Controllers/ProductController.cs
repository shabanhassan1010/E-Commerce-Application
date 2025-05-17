using E_Commerce.DomainLayer.Entities;
using E_Commerce.InfrastructureLayer.Data.DBContext.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce_Application.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        #region DBContext
        private readonly IRepository<Product> repository;

        public ProductController(IRepository<Product> repository)
        {
            this.repository = repository;
        }
        #endregion

        [HttpGet("GetAll")]
        public async Task<ActionResult<IEnumerable<Product>>> GetAll()
        { 
            return Ok(await repository.GetAllAsync());
        }

        [HttpGet("GetProduct/{id:int}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await repository.GetByIdAsync(id);
            if (product == null)
                return NotFound();
            return Ok(product);
        }

        [HttpPost("CreateProduct")]
        public async Task<ActionResult<Product>> CreateProduct(Product product)
        {
            repository.AddAsync(product);
            if (await repository.SaveAsync())
            {
                return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
            }
            return BadRequest("Can Not Create Product");
        }

        [HttpPost("UpdateProduct/{id:int}")]
        public async Task<ActionResult<Product>> UpdateProduct(Product product, int id)
        {
            if (id != product.Id)
                return BadRequest("Can not Update this product");
            repository.UpdateAsync(product);
            if (await repository.SaveAsync())
            {
                return NoContent();
            }
            return BadRequest("Problem in Updating In product");

        }
        [HttpDelete("DeleteProduct/{id:int}")]
        public async Task<ActionResult<Product>> DeleteProduct(int id)
        {
            var Product = await repository.GetByIdAsync(id);
            if (Product == null)
            {
                return NotFound();
            }

            repository.DeleteAsync(Product);
            if (await repository.SaveAsync())
            {
                return NoContent();
            }
            return BadRequest("Problem in Deleting this product");
        }
    }
}
