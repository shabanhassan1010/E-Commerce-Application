using E_Commerce.DomainLayer.Entities;
using E_Commerce.DomainLayer.Interfaces;
using E_Commerce.InfrastructureLayer.Data.DBContext;
using E_Commerce.InfrastructureLayer.Data.DBContext.Repositories;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.InfrastructureLayer.Data.InterfacesImplementaion
{
    public class ShoppingCartRepository : GenericRepository<ShoppingCart>, IShoppingCartRepository
    {
        #region Context
        private readonly ApplicationDBContext context;
        public ShoppingCartRepository(ApplicationDBContext context) : base(context)
        {
            this.context = context;
        }
        #endregion


        public async Task<ShoppingCart> GetByUserIdAsync(string userId)
        {
            return await context.shoppingCarts.Include(c => c.CartItems).ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);
        }
    }
}
