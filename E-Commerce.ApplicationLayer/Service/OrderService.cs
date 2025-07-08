using AutoMapper;
using E_Commerce.ApplicationLayer.ApiResponse;
using E_Commerce.ApplicationLayer.Dtos.Orders;
using E_Commerce.ApplicationLayer.IService;
using E_Commerce.DomainLayer.Entities;
using E_Commerce.DomainLayer.Entities.Roles;
using E_Commerce.DomainLayer.Interfaces;
using Microsoft.Extensions.Logging;
using System.Runtime.ConstrainedExecution;
namespace E_Commerce.ApplicationLayer.Service
{
    public class OrderService : IOrderService
    {
        #region context
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly ILogger<OrderService> logger;

        public OrderService(IUnitOfWork unitOfWork, IMapper mapper , ILogger<OrderService> logger)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.logger = logger;
        }
        #endregion

        public async Task<Result<OrderDto>> PlaceOrderAsync(string userId)
        {
            logger.LogInformation("Starting order placement for user {UserId}", userId);

            // Check if Cart have any order ot not
            var cart = await unitOfWork.shoppingCartRepository.GetByUserIdAsync(userId);

            if (cart == null || !cart.CartItems.Any() || cart.CartItems == null)
            {
                logger.LogWarning("Cart is empty or not found for user {UserId}", userId);
                return Result<OrderDto>.Failure("Cart is empty or not found.");
            }

            foreach (var item in cart.CartItems)
            {
                var product = await unitOfWork.productRepository.GetByIdAsync(item.ProductId);
                if (product == null || product.QuantityInStock < item.Quantity)
                {
                    logger.LogWarning("Product {ProductId} is out of stock or does not exist. Requested: {Qty}, Available: {Stock}",
                                   item.ProductId, item.Quantity, product?.QuantityInStock ?? 0);
                    return Result<OrderDto>.Failure($"Product {item.ProductId} is not available in requested quantity.");
                }

                product.QuantityInStock -= item.Quantity;
                await unitOfWork.productRepository.UpdateAsync(product);
            }

            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.UtcNow,
                OrderItems = cart.CartItems.Select(ci => new OrderItem
                {
                    ProductId = ci.ProductId,
                    Quantity = ci.Quantity,
                    UnitPrice = ci.Product?.Price ?? 0
                }).ToList(),
                TotalPrice = cart.CartItems.Sum(ci => ci.Quantity * (ci.Product?.Price ?? 0)),
                Status = OrderStatus.Pending
            };

            await unitOfWork.OrdersRepository.AddAsync(order);
            unitOfWork.cartRepository.RemoveRange(cart.CartItems);
            await unitOfWork.SaveAsync();
            logger.LogInformation("Order {OrderId} placed successfully for user {UserId}", order.Id, userId);

            var dto = mapper.Map<OrderDto>(order);
            return Result<OrderDto>.Ok(dto, "Order placed successfully.");
        }
        public async Task<Result<OrderDto>?> GetAllOrdersForEachUser(int orderId, string userId)  // Get order details 
        {
            logger.LogInformation("Retrieving order {OrderId} for user {UserId}", orderId, userId);

            var order = await unitOfWork.OrdersRepository.GetOrderWithDetailsAsync(orderId, userId);
            if (order == null)
            {
                logger.LogWarning("Order {OrderId} not found for user {UserId}", orderId, userId);
                return Result<OrderDto>.Failure("Order not found");
            }

            logger.LogInformation("Order {OrderId} retrieved successfully for user {UserId}", orderId, userId);

            var dto = mapper.Map<OrderDto>(order);

            return Result<OrderDto>.Ok(dto, "Order retrive succfully ");
        }
        public async Task<IEnumerable<Result<OrderDto>>> GetOrdersForUserAsync(string userId)  // return all order related to this User
        {
            logger.LogInformation("Retrieving all orders for user {UserId}", userId);

            var orders = await unitOfWork.OrdersRepository.GetUserOrdersWithDetailsAsync(userId);
            if (orders == null || !orders.Any())
            {
                logger.LogWarning("No orders found for user {UserId}", userId);
                return new List<Result<OrderDto>> { Result<OrderDto>.Failure("No orders found for this user.") };

            }

            logger.LogInformation("{OrderCount} orders found for user {UserId}", orders.Count(), userId);

            var dto = mapper.Map<List<OrderDto>>(orders);
            var resultList = dto.Select(dto => Result<OrderDto>.Ok(dto, "Order retrieved successfully")).ToList();
            return resultList;
        }
        
    }
}