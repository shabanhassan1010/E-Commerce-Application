using E_Commerce.DomainLayer.Entities;
using E_Commerce.DomainLayer.Interfaces;
using E_Commerce.InfrastructureLayer.Data.DBContext;
using E_Commerce.InfrastructureLayer.Data.DBContext.Repositories;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.InfrastructureLayer.Data.InterfacesImplementaion
{
    public class OrderRepository : GenericRepository<Order>, IOrderRepository
    {
        #region Context
        private readonly ApplicationDBContext context;
        public OrderRepository(ApplicationDBContext context) : base(context)
        {
            this.context = context;
        }
        #endregion

        public async Task<Order?> GetOrderWithDetailsAsync(int orderId, string userId)
        {
            return await context.orders.AsNoTracking().Include(x=>x.OrderItems).ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId);
        }

        public async Task<List<Order>> GetUserOrdersWithDetailsAsync(string userId)
        {
            return await context.orders.AsNoTracking()
                .Include(x => x.OrderItems).ThenInclude(p => p.Product)
                .Where(p => p.UserId == userId).ToListAsync();
        }
    }
}
