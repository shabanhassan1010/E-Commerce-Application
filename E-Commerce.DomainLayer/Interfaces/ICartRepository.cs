using E_Commerce.DomainLayer.Entities;
using E_Commerce.InfrastructureLayer.Data.DBContext.Repositories;

namespace E_Commerce.DomainLayer.Interfaces
{
    public interface ICartRepository : IGenericRepository<CartItem>
    {
        Task<ShoppingCart?> GetCartAysnc(string key);
        Task<ShoppingCart?> SetCartAysnc(ShoppingCart shoppingCart);
        Task<bool> DeleteCartAysnc(string key);
    }
}
