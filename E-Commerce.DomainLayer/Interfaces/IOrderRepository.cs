using E_Commerce.DomainLayer.Entities;
using E_Commerce.InfrastructureLayer.Data.DBContext.Repositories;

namespace E_Commerce.DomainLayer.Interfaces
{
    public interface IOrderRepository : IGenericRepository<Order>
    {
        public Task<Order?> GetOrderWithDetailsAsync(int orderId, string userId);
        public Task<List<Order>> GetUserOrdersWithDetailsAsync(string userId);
    }
}
