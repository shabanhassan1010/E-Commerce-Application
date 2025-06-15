using E_Commerce.DomainLayer.Entities;
using E_Commerce.DomainLayer.Interfaces;
using E_Commerce.InfrastructureLayer.Data.DBContext;
using E_Commerce.InfrastructureLayer.Data.DBContext.Repositories;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.InfrastructureLayer.Data.InterfacesImplementaion
{
    public class ShoppingCartRepository : GenericRepository<ShoppingCart>, IShoppingCartRepository
    {
        private readonly ApplicationDBContext context;

        public ShoppingCartRepository(ApplicationDBContext context) : base(context) 
        {
            this.context = context;
        }

        public async Task<ShoppingCart> GetByUserIdAsync(string userId)
        {
            return await context.shoppingCarts.FirstOrDefaultAsync(c => c.UserId == userId);
        }
    }
}
