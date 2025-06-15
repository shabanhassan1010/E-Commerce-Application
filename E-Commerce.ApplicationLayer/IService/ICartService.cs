using E_Commerce.ApplicationLayer.Dtos.ProductCart;


namespace E_Commerce.ApplicationLayer.IService
{
    public interface ICartService
    {
        /// <summary>
        /// Gets the user's shopping cart with all items and calculated totals
        /// </summary>
        /// <param name="userId">The authenticated user's ID</param>
        /// <returns>CartResponseDto containing cart items and total</returns>
        Task<CartResponseDto> GetCartAsync(string userId);

        /// <summary>
        /// Adds an item to the user's cart or updates quantity if item exists
        /// </summary>
        /// <param name="userId">The authenticated user's ID</param>
        /// <param name="dto">AddToCartDto containing product details</param>
        /// <returns>CartItemDto representing the added/updated item</returns>
        Task<CartItemDto> AddItemToCartAsync(string userId, AddToCartDto dto);

        /// <summary>
        /// Removes an item from the user's cart
        /// </summary>
        /// <param name="userId">The authenticated user's ID</param>
        /// <param name="productId">ID of the product to remove</param>
        /// <returns>True if removal was successful</returns>
        Task<bool> RemoveItemFromCartAsync(string userId, int productId);

        /// <summary>
        /// Updates the quantity of a specific item in the cart
        /// </summary>
        /// <param name="userId">The authenticated user's ID</param>
        /// <param name="dto">UpdateCartItemDto containing product ID and new quantity</param>
        /// <returns>True if update was successful</returns>
        Task<bool> UpdateItemQuantityAsync(string userId, UpdateCartItemDto dto);

        /// <summary>
        /// Clears all items from the user's cart
        /// </summary>
        /// <param name="userId">The authenticated user's ID</param>
        /// <returns>True if cart was cleared successfully</returns>
        Task<bool> ClearCartAsync(string userId);
    }
}
