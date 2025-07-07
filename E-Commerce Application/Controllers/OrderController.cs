using E_Commerce.ApplicationLayer.ApiResponse;
using E_Commerce.ApplicationLayer.IService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace E_Commerce_Application.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        #region Context
        private readonly IOrderService orderService;
        public OrderController(IOrderService orderService)
        {
            this.orderService = orderService;
        }
        #endregion
        protected string? GetUserId()
        {
            return User?.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        #region PlaceOrder
        [HttpPost]
        [EndpointSummary("Place Orders ")]
        public async Task<IActionResult> PlaceOrder()
        {
            var userId = GetUserId();
            userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if(string.IsNullOrEmpty(userId)) 
                return Unauthorized();

            var res =  await orderService.PlaceOrderAsync(userId);
            if (!res.Success)
                return BadRequest(new { res.Message });
            return Ok(res);
        }
        #endregion

        #region GetAllOrdersForEachUser
        [HttpGet("{orderId}")]
        [EndpointSummary("Get single order details for a specific user")]
        public async Task<IActionResult> GetAllOrdersForEachUser(int orderId)
        {
            var userId = GetUserId();
            if(string.IsNullOrEmpty(userId))
                return Unauthorized();

            var GetAllOrders = await orderService.GetAllOrdersForEachUser(orderId ,userId);
            if(GetAllOrders == null || !GetAllOrders.Success)
                return NotFound(new { GetAllOrders?.Message });

            return Ok(GetAllOrders);
        }
        #endregion

        #region GetOrdersForUserAsync
        [HttpGet]
        [EndpointSummary("Get all orders for the logged-in user")]
        public async Task<IActionResult> GetOrdersForUserAsync()
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var GetOrders = await orderService.GetOrdersForUserAsync(userId);

            return Ok(GetOrders);
        }
        #endregion
    }
}
