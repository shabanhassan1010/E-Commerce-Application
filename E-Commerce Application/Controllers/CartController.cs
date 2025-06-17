using E_Commerce.ApplicationLayer.ActionFilters;
using E_Commerce.ApplicationLayer.Dtos.ProductCart;
using E_Commerce.ApplicationLayer.IService;
using E_Commerce.ApplicationLayer.MiddleWares;
using E_Commerce.DomainLayer.Entities;
using E_Commerce.DomainLayer.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace E_Commerce_Application.Controllers
{
    //[Authorize] // Base authorization for all endpoints
    //[LogSensitiveAction]
    public class CartController : BaseApiController
    {
        #region Context
        private readonly ICartService _cartService;
        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }
        #endregion

        protected string GetUserIdFromDB => User.FindFirstValue(ClaimTypes.NameIdentifier);

        [HttpGet]
        //[Authorize(Policy = "CustomerPolicy")] 
        public async Task<ActionResult<CartResponseDto>> GetCart()
        {
            var userId = GetUserIdFromDB;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var cart = await _cartService.GetCartAsync(userId);
            return Ok(cart);
        }

        [HttpPost("items")]
        //[Authorize(Policy = "CustomerPolicy")]
        [EndpointSummary("Add Item To Cart")]
        public async Task<ActionResult<CartItemDto>> AddItemToCart(AddToCartDto dto)
        {
            var userId = GetUserIdFromDB;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var result = await _cartService.AddItemToCartAsync(userId, dto);
            if (result == null)
                return NotFound("Cart not found for this user.");

            return Ok(result);
        }

        //[HttpDelete("items/{productId}")]
        ////[Authorize(Policy = "CustomerPolicy")]
        //[EndpointSummary("Remove Item From Cart")]
        //public async Task<IActionResult> RemoveItemFromCart(int productId)
        //{
        //    var userId = GetUserIdFromDB;
        //    if (string.IsNullOrEmpty(userId)) return Unauthorized();

        //    var success = await _cartService.RemoveItemFromCartAsync(userId, productId);
        //    return success ? NoContent() : NotFound();
        //}

        //[HttpPut("items")]
        ////[Authorize(Policy = "CustomerPolicy")]
        //[EndpointSummary("Update Item FromCart")]
        //public async Task<IActionResult> UpdateItemQuantity(UpdateCartItemDto dto)
        //{
        //    var userId = GetUserIdFromDB;
        //    if (string.IsNullOrEmpty(userId)) return Unauthorized();

        //    var success = await _cartService.UpdateItemQuantityAsync(userId, dto);
        //    return success ? NoContent() : NotFound();
        //}

        //[HttpDelete("clear")]
        ////[Authorize(Policy = "CustomerPolicy")]
        //[EndpointSummary("Clear Cart")]
        //public async Task<IActionResult> ClearCart()
        //{
        //    var userId = GetUserIdFromDB;
        //    if (string.IsNullOrEmpty(userId)) return Unauthorized();

        //    var success = await _cartService.ClearCartAsync(userId);
        //    return success ? NoContent() : NotFound();
        //}
    }
}
