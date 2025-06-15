using E_Commerce.ApplicationLayer.ActionFilters;
using E_Commerce.ApplicationLayer.Dtos.ProductCart;
using E_Commerce.ApplicationLayer.IService;
using E_Commerce.DomainLayer.Entities;
using E_Commerce.DomainLayer.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce_Application.Controllers
{
    //[Authorize] // Base authorization for all endpoints
    //[LogSensitiveAction]
    public class CartController : BaseApiController
    {
        private readonly ICartService _cartService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CartController(ICartService cartService, IHttpContextAccessor httpContextAccessor)
        {
            _cartService = cartService;
            _httpContextAccessor = httpContextAccessor;
        }

        private string GetUserId() => _httpContextAccessor.HttpContext.User.FindFirst("sub")?.Value;

        [HttpGet]
        [Authorize(Policy = "CustomerPolicy")] // Only customers can view their cart
        public async Task<ActionResult<CartResponseDto>> GetCart()
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var cart = await _cartService.GetCartAsync(userId);
            return Ok(cart);
        }

        [HttpPost("items")]
        [Authorize(Policy = "CustomerPolicy")] // Only customers can add items
        public async Task<ActionResult<CartItemDto>> AddItemToCart(AddToCartDto dto)
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var result = await _cartService.AddItemToCartAsync(userId, dto);
            return Ok(result);
        }

        [HttpDelete("items/{productId}")]
        [Authorize(Policy = "CustomerPolicy")] // Only customers can remove items
        public async Task<IActionResult> RemoveItemFromCart(int productId)
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var success = await _cartService.RemoveItemFromCartAsync(userId, productId);
            return success ? NoContent() : NotFound();
        }

        [HttpPut("items")]
        [Authorize(Policy = "CustomerPolicy")] // Only customers can update quantities
        public async Task<IActionResult> UpdateItemQuantity(UpdateCartItemDto dto)
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var success = await _cartService.UpdateItemQuantityAsync(userId, dto);
            return success ? NoContent() : NotFound();
        }

        [HttpDelete("clear")]
        [Authorize(Policy = "CustomerPolicy")] // Only customers can clear carts
        public async Task<IActionResult> ClearCart()
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var success = await _cartService.ClearCartAsync(userId);
            return success ? NoContent() : NotFound();
        }
    }
}
