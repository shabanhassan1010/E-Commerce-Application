using E_Commerce.DomainLayer.Entities;
using E_Commerce.InfrastructureLayer.Data.DBContext.Repositories;

namespace E_Commerce.DomainLayer.Interfaces
{
    public interface ICartRepository : IGenericRepository<CartItem>
    {
        Task<ShoppingCart> GetCartByUserIdAsync(string userId);
        Task<ShoppingCart> GetCartWithItemsAsync(string userId);
        Task<CartItem> GetCartItemAsync(int productId, int shoppingCartId);
        Task<IEnumerable<CartItem>> GetCartItemsAsync(int shoppingCartId);
        void RemoveRange(IEnumerable<CartItem> entities);
        Task<CartItem?> GetCartItemWithProductByIdAsync(int cartItemId);
        Task<CartItem?> GetCartItemWithProductAsync(int productId, int shoppingCartId);
        IQueryable<CartItem> GetQueryable();
    }
}
