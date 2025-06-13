using E_Commerce.DomainLayer.Entities;
using E_Commerce.DomainLayer.Interfaces;
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
        [ProducesResponseType(200, Type = typeof(IEnumerable<CartItem>))]
        [ProducesResponseType(400)]
        public async Task<ActionResult<IEnumerable<CartItem>>> GetAll()
        {
            var CartItems = await unitOfWork.cartRepository.GetAllAsync();
            return HandleResult(CartItems, "CartItems retrieved successfully");
        }
    }
}
