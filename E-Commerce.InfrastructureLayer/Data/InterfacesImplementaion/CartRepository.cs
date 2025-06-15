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
        private readonly ApplicationDBContext context;
        public CartRepository(ApplicationDBContext context) : base(context)
        {
            this.context = context;
        }
        public async Task<ShoppingCart> GetCartByUserIdAsync(string userId)
        {
            return await context.shoppingCarts
                .FirstOrDefaultAsync(c => c.UserId == userId);
        }

        public async Task<ShoppingCart> GetCartWithItemsAsync(string userId)
        {
            return await context.shoppingCarts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == userId);
        }
        public async Task<CartItem> GetCartItemAsync(int productId, string shoppingCartId)
        {
            return await context.cartItems
                .FirstOrDefaultAsync(ci => ci.ProductId == productId && ci.ShoppingCartId == shoppingCartId);
        }

        public async Task<IEnumerable<CartItem>> GetCartItemsAsync(string shoppingCartId)
        {
            return await context.cartItems
                .Where(ci => ci.ShoppingCartId == shoppingCartId)
                .ToListAsync();
        }
    }
}
