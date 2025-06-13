using E_Commerce.DomainLayer.Entities;
using E_Commerce.DomainLayer.Interfaces;
using E_Commerce.InfrastructureLayer.Data.DBContext;
using E_Commerce.InfrastructureLayer.Data.DBContext.Repositories;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace E_Commerce.InfrastructureLayer.Data.InterfacesImplementaion
{
    public class CartRepository : GenericRepository<CartItem>, ICartRepository
    {
        private readonly ApplicationDBContext context;
        private readonly IDistributedCache _cache;
        private const string CART_PREFIX = "Cart_";

        public CartRepository(ApplicationDBContext context, IDistributedCache cache) : base(context)
        {
            this.context = context;
            _cache = cache;
        }

        public async Task<bool> DeleteCartAysnc(string key)
        {
            try
            {
                await _cache.RemoveAsync(CART_PREFIX + key);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<ShoppingCart?> GetCartAysnc(string key)
        {
            var cartData = await _cache.GetStringAsync(CART_PREFIX + key);
            if (string.IsNullOrEmpty(cartData))
                return null;

            return JsonSerializer.Deserialize<ShoppingCart>(cartData);
        }

        public async Task<ShoppingCart?> SetCartAysnc(ShoppingCart shoppingCart)
        {
            try
            {
                var cartData = JsonSerializer.Serialize(shoppingCart);
                await _cache.SetStringAsync(CART_PREFIX + shoppingCart.Id, cartData);
                return shoppingCart;
            }
            catch
            {
                return null;
            }
        }
    }
}
