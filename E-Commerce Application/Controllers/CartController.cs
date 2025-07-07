using E_Commerce.ApplicationLayer.Dtos.ProductCart;
using E_Commerce.ApplicationLayer.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace E_Commerce_Application.Controllers
{
    //[Authorize] // Base authorization for all endpoints
    //[LogSensitiveAction]
    [Authorize(Policy = "CustomerPolicy")]
    public class CartController : BaseApiController
    {
        #region Context
        private readonly ICartService _cartService;
        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }
        #endregion

        protected string? GetUserId()
        {
            return User?.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        #region  GetCart
        [HttpGet]
        public async Task<ActionResult<CartResponseDto>> GetCart()
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId)) 
                return Unauthorized();

            var cart = await _cartService.GetCartAsync(userId);
            return Ok(cart);
        }
        #endregion

        #region AddItemToCart
        [HttpPost("items")]
        [EndpointSummary("Add Item To Cart")]
        public async Task<ActionResult<CartItemDto>> AddItemToCart(AddToCartDto dto)
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var result = await _cartService.AddItemToCartAsync(userId, dto);
            if (result == null)
                return NotFound("Cart not found for this user.");

            return Ok(result);
        }
        #endregion

        #region DeleteItemFromCart
        [HttpDelete("DeleteItem/{productId}")]
        [EndpointSummary("Delete Item From Cart")]
        public async Task<IActionResult> DeleteItemFromCart(int productId)
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var success = await _cartService.DeleteItemFromCartAsync(userId, productId);
            if (!success)
                return NotFound();
            return Ok(success);
        }
        #endregion

        #region UpdateItemQuantity
        [HttpPut("items")]
        [EndpointSummary("Update Item FromCart")]
        public async Task<IActionResult> UpdateItemQuantity(UpdateCartItemDto dto)
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var success = await _cartService.UpdateItemQuantityAsync(userId, dto);
            return success ? NoContent() : NotFound();
        }
        #endregion

        #region ClearCart
        [HttpDelete("clear")]
        [EndpointSummary("Clear All Cart")]
        public async Task<IActionResult> ClearCart()
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var success = await _cartService.ClearCartAsync(userId);
            return success ? NoContent() : NotFound();
        }
        #endregion
    }
}
