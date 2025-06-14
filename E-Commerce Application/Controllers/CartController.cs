using E_Commerce.ApplicationLayer.ApiResponse;
using E_Commerce.DomainLayer.Entities;
using E_Commerce.DomainLayer.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce_Application.Controllers
{
    public class CartController : BaseApiController
    {
        #region DBContext
        private readonly IUnitOfWork unitOfWork;
        public CartController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }
        #endregion

        [HttpGet("GetAll")]
        [EndpointSummary("Get All Cart")]
        [ProducesResponseType(200, Type = typeof(ApiResponse<IEnumerable<CartItem>>))]
        [ProducesResponseType(400)]
        [Authorize]
        [Authorize(Policy = "CustomerPolicy")]
        public async Task<ActionResult<ApiResponse<IEnumerable<CartItem>>>> GetAll()
        {
            var CartItems = await unitOfWork.cartRepository.GetAllAsync();
            return Ok("CartItems retrieved successfully");
        }
    }
}
