using E_Commerce.ApplicationLayer.Dtos.Product;
using E_Commerce.DomainLayer.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce_Application.Controllers
{
    public class BuggyController : BaseApiController
    {
        [HttpGet("NotFound")]
        public IActionResult GetNotFound()
        {
            return NotFound();
        }
        [HttpGet("Unauthorized")]
        public IActionResult GetUnauthorized()
        {
            return Unauthorized();
        }
        [HttpGet("BadRequest")]
        public IActionResult GetBadRequest()
        {
            return BadRequest("This Requst must have a problem");
        }
        [HttpGet("InternalError")]
        public IActionResult GetInternalError()
        {
            throw new Exception("This is a test Expection ");
        }

        [HttpPost("ValidationErrors")]
        public IActionResult GetValidationErrors(CreateProductDto productDto)
        {
            return Ok();
        }
    }
}
