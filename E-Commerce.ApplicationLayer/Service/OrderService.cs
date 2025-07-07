using AutoMapper;
using E_Commerce.ApplicationLayer.ApiResponse;
using E_Commerce.ApplicationLayer.Dtos.Orders;
using E_Commerce.ApplicationLayer.IService;
using E_Commerce.DomainLayer.Entities;
using E_Commerce.DomainLayer.Entities.Roles;
using E_Commerce.DomainLayer.Interfaces;
using System.Runtime.ConstrainedExecution;
namespace E_Commerce.ApplicationLayer.Service
{
    public class OrderService : IOrderService
    {
        #region context
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        public OrderService(IUnitOfWork unitOfWork, IMapper mapper )
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }
        #endregion

        public async Task<Result<OrderDto>> PlaceOrderAsync(string userId)
        {
            // Check if Cart have any order ot not
            var cart = await unitOfWork.shoppingCartRepository.GetByUserIdAsync(userId);
            if (cart == null || !cart.CartItems.Any() || cart.CartItems == null)
                return Result<OrderDto>.Failure("Cart is empty or not found.");

            foreach (var item in cart.CartItems)
            {
                var product = await unitOfWork.productRepository.GetByIdAsync(item.ProductId);
                if (product == null || product.QuantityInStock < item.Quantity)
                    return Result<OrderDto>.Failure($"Product {item.ProductId} is not available in requested quantity.");

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

            var dto = mapper.Map<OrderDto>(order);
            return Result<OrderDto>.Ok(dto, "Order placed successfully.");
        }
        public async Task<Result<OrderDto>?> GetAllOrdersForEachUser(int orderId, string userId)  // Get order details 
        {
            var order = await unitOfWork.OrdersRepository.GetOrderWithDetailsAsync(orderId, userId);
            if (order == null)
                return Result<OrderDto>.Failure("Order not found");

            var dto = mapper.Map<OrderDto>(order);

            return Result<OrderDto>.Ok(dto, "Order retrive succfully ");
        }
        public async Task<IEnumerable<Result<OrderDto>>> GetOrdersForUserAsync(string userId)  // return all order related to this User
        {
            var orders = await unitOfWork.OrdersRepository.GetUserOrdersWithDetailsAsync(userId);
            if (orders == null || !orders.Any())
                return new List<Result<OrderDto>> { Result<OrderDto>.Failure("No orders found for this user.") };

            var dto = mapper.Map<List<OrderDto>>(orders);
            var resultList = dto.Select(dto => Result<OrderDto>.Ok(dto, "Order retrieved successfully")).ToList();

            return resultList;
        }
        
    }
}