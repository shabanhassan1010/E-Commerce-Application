using E_Commerce.DomainLayer.Entities;
using E_Commerce.DomainLayer.Interfaces;
using E_Commerce.InfrastructureLayer.Data.DBContext;
using E_Commerce.InfrastructureLayer.Data.DBContext.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace E_Commerce.InfrastructureLayer.Data.InterfacesImplementaion
{
    public class CartRepository : GenericRepository<CartItem>, ICartRepository
    {
        #region Context
        private readonly ApplicationDBContext context;
        public CartRepository(ApplicationDBContext context) : base(context)
        {
            this.context = context;
        }
        #endregion

        public async Task<ShoppingCart> GetCartByUserIdAsync(string userId)
        {
            return await context.shoppingCarts.AsNoTracking().FirstOrDefaultAsync(c => c.UserId == userId);
        }
        public async Task<ShoppingCart> GetCartWithItemsAsync(string userId)
        {
            return await context.shoppingCarts.AsNoTracking().Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == userId);
        }
        public async Task<CartItem> GetCartItemAsync(int productId, string shoppingCartId)
        {
            return await context.cartItems.AsNoTracking()
                .FirstOrDefaultAsync(ci => ci.ProductId == productId && ci.ShoppingCartId == shoppingCartId);
        }
        public async Task<IEnumerable<CartItem>> GetCartItemsAsync(string shoppingCartId)
        {
            return await context.cartItems.AsNoTracking()
                .Where(ci => ci.ShoppingCartId == shoppingCartId)
                .ToListAsync();
        }
    }
}
