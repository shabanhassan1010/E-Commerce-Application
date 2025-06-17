using E_Commerce.ApplicationLayer.Dtos.ProductCart;


namespace E_Commerce.ApplicationLayer.IService
{
    public interface ICartService
    {
        Task<CartResponseDto> GetCartAsync(string userId);
        Task<CartItemDto> AddItemToCartAsync(string userId, AddToCartDto dto);
        //Task<bool> RemoveItemFromCartAsync(string userId, int productId);
        //Task<bool> UpdateItemQuantityAsync(string userId, UpdateCartItemDto dto);
        //Task<bool> ClearCartAsync(string userId);
    }
}
