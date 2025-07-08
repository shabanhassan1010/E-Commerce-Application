using AutoMapper;
using E_Commerce.ApplicationLayer.Dtos.ProductCart;
using E_Commerce.ApplicationLayer.IService;
using E_Commerce.DomainLayer.Entities;
using E_Commerce.DomainLayer.Interfaces;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;


namespace E_Commerce.ApplicationLayer.Service
{
    public class CartService : ICartService
    {
        #region MyRegion
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CartService> _logger;

        public CartService( IMapper mapper, IUnitOfWork unitOfWork , ILogger<CartService> logger)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }
        #endregion

        public async Task<CartResponseDto> GetCartAsync(string userId)
        {
            _logger.LogInformation("Retrieving cart for user {UserId}", userId);

            var cart = await _unitOfWork.cartRepository.GetCartWithItemsAsync(userId);

            if (cart == null)
            {
                _logger.LogInformation("No existing cart found. Creating new cart for user {UserId}", userId);
                cart = await CreateNewCart(userId);
                await _unitOfWork.SaveAsync();
                _logger.LogInformation("New cart {CartId} created for user {UserId}", cart.Id, userId);
            }
            Log.Information("Cart {CartId} retrieved for user {UserId}", cart.Id, userId);
            return _mapper.Map<CartResponseDto>(cart);
        }
        public async Task<CartItemDto> AddItemToCartAsync(string userId, AddToCartDto dto)
        {
            _logger.LogInformation("Adding product {ProductId} (Qty: {Qty}) to cart for user {UserId}", dto.ProductId, dto.Quantity, userId);

            var cart = await _unitOfWork.shoppingCartRepository.GetByUserIdAsync(userId);
            if (cart == null)
            {
                _logger.LogInformation("No existing cart found. Creating new cart for user {UserId}", userId);

                cart = new ShoppingCart
                {
                    UserId = userId,
                    CartItems = new List<CartItem>()
                };
                await _unitOfWork.shoppingCartRepository.AddAsync(cart);
                await _unitOfWork.SaveAsync();
                _logger.LogInformation("New cart created with ID {CartId} for user {UserId}", cart.Id, userId);
            }

            var product = await _unitOfWork.productRepository.GetByIdAsync(dto.ProductId);
            if (product == null)
            {
                _logger.LogWarning("Product {ProductId} not found for user {UserId}", dto.ProductId, userId);
                throw new Exception("Product not found");
            }

            var existingItem = await _unitOfWork.cartRepository.GetCartItemAsync(dto.ProductId, cart.Id);

            if (existingItem != null)
            {
                existingItem.Quantity += dto.Quantity;
                await _unitOfWork.cartRepository.UpdateAsync(existingItem);
                await _unitOfWork.SaveAsync();
                _logger.LogInformation("Updated quantity for product {ProductId} in cart {CartId}", dto.ProductId, cart.Id);

                var itemWithProduct = await _unitOfWork.cartRepository.GetCartItemWithProductByIdAsync(existingItem.Id);
                return _mapper.Map<CartItemDto>(itemWithProduct);
            }

            var newItem = new CartItem
            {
                ProductId = dto.ProductId,
                Quantity = dto.Quantity,
                ShoppingCartId = cart.Id
            };

            await _unitOfWork.cartRepository.AddAsync(newItem);
            await _unitOfWork.SaveAsync();
            _logger.LogInformation("Added new product {ProductId} to cart {CartId}", dto.ProductId, cart.Id);

            var addedItem = await _unitOfWork.cartRepository.GetCartItemWithProductAsync(newItem.ProductId, cart.Id);
            return _mapper.Map<CartItemDto>(addedItem);
        }
        public async Task<bool> DeleteItemFromCartAsync(string userId, int productId)
        {
            _logger.LogInformation("Deleting product {ProductId} from cart for user {UserId}", productId, userId);

            var cart = await _unitOfWork.cartRepository.GetCartByUserIdAsync(userId);
            if (cart == null)
            {
                _logger.LogWarning("No cart found for user {UserId} when trying to delete product {ProductId}", userId, productId);
                return false;  // this user have not any items in cart
            }

            var item = await _unitOfWork.cartRepository.GetCartItemAsync(productId, cart.Id);
            if (item == null)
            {
                _logger.LogWarning("No cart item found for product {ProductId} in cart {CartId}", productId, cart.Id);
                return false;
            }

            var product = await _unitOfWork.productRepository.GetByIdAsync(productId);
            if (product != null && product.QuantityInStock >= item.Quantity)
            {
                product.QuantityInStock -= item.Quantity;
                await _unitOfWork.productRepository.UpdateAsync(product);
            }

            await _unitOfWork.cartRepository.DeleteAsync(item);
            await _unitOfWork.SaveAsync();
            _logger.LogInformation("Deleted product {ProductId} from cart {CartId}", productId, cart.Id);
            return true;
        }
        public async Task<bool> UpdateItemQuantityAsync(string userId, UpdateCartItemDto dto)
        {
            _logger.LogInformation("Updating quantity of product {ProductId} to {Qty} in cart for user {UserId}", dto.ProductId, dto.Quantity, userId);
            var cart = await _unitOfWork.cartRepository.GetCartByUserIdAsync(userId);
            if (cart == null)
            {
                _logger.LogWarning("No cart found for user {UserId} while updating product {ProductId}", userId, dto.ProductId);
                return false;
            }

            var item = await _unitOfWork.cartRepository.GetCartItemAsync(dto.ProductId, cart.Id);
            if (item == null)
            {
                _logger.LogWarning("Cart item not found for product {ProductId} in cart {CartId}", dto.ProductId, cart.Id);
                return false;
            }

            item.Quantity = dto.Quantity;
            await _unitOfWork.cartRepository.UpdateAsync(item);
            await _unitOfWork.SaveAsync();
            _logger.LogInformation("Updated quantity for product {ProductId} to {Qty} in cart {CartId}", dto.ProductId, dto.Quantity, cart.Id);
            return true;
        }
        public async Task<bool> ClearCartAsync(string userId)
        {
            _logger.LogInformation("Clearing cart for user {UserId}", userId);

            var cart = await _unitOfWork.cartRepository.GetCartWithItemsAsync(userId);
            if (cart == null || !cart.CartItems.Any())
            {
                _logger.LogWarning("No cart or items found for user {UserId} during clear operation", userId);
                return false;
            }

            int itemCount = cart.CartItems.Count;
            foreach (var item in cart.CartItems.ToList())
            {
                await _unitOfWork.cartRepository.DeleteAsync(item);
            }

            await _unitOfWork.SaveAsync();
            _logger.LogInformation("Cleared {ItemCount} items from cart {CartId} for user {UserId}", itemCount, cart.Id, userId);
            return true;
        }
        private async Task<ShoppingCart> CreateNewCart(string userId)
        {
            _logger.LogInformation("Creating new cart for user {UserId}", userId);
            var newCart = new ShoppingCart
            {
                UserId = userId,
                CartItems = new List<CartItem>()
            };
            await _unitOfWork.shoppingCartRepository.AddAsync(newCart);
            await _unitOfWork.SaveAsync();
            _logger.LogInformation("New cart created with ID {CartId} for user {UserId}", newCart.Id, userId);
            return newCart;
        }

        //private decimal CalculateCartTotal(IEnumerable<CartItem> items)
        //{
        //    return items.Sum(item => item.Price * item.Quantity);
        //}
    }
}
