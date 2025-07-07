using E_Commerce.ApplicationLayer.ApiResponse;
using E_Commerce.ApplicationLayer.Dtos.Orders;
using E_Commerce.DomainLayer.Entities;

namespace E_Commerce.ApplicationLayer.IService
{
    public interface IOrderService
    {
        Task<Result<OrderDto>> PlaceOrderAsync(string userId);
        Task<IEnumerable<Result<OrderDto>>> GetOrdersForUserAsync(string userId);
        Task<Result<OrderDto>?> GetAllOrdersForEachUser(int orderId, string userId);
    }
}
