using AutoMapper;
using E_Commerce.ApplicationLayer.Dtos.ProductCart;
using E_Commerce.ApplicationLayer.IService;
using E_Commerce.DomainLayer.Entities;
using E_Commerce.DomainLayer.Interfaces;
using Microsoft.Extensions.Logging;
using Serilog;


namespace E_Commerce.ApplicationLayer.Service
{
    public class CartService : ICartService
    {
        #region MyRegion
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public CartService( IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        #endregion

        public async Task<CartResponseDto> GetCartAsync(string userId)
        {
            var cart = await _unitOfWork.cartRepository.GetCartWithItemsAsync(userId);

            if (cart == null)
            {
                cart = await CreateNewCart(userId);
                await _unitOfWork.SaveAsync();
            }
            Log.Information($"GetCartAsync {cart.Id}");
            return _mapper.Map<CartResponseDto>(cart);
        }
        public async Task<CartItemDto> AddItemToCartAsync(string userId, AddToCartDto dto)
        {
            var cart = await _unitOfWork.shoppingCartRepository.GetByUserIdAsync(userId);
            if (cart == null)
            {
                cart = new ShoppingCart
                {
                    UserId = userId,
                    CartItems = new List<CartItem>()
                };
                await _unitOfWork.shoppingCartRepository.AddAsync(cart);
                await _unitOfWork.SaveAsync();
            }

            var product = await _unitOfWork.productRepository.GetByIdAsync(dto.ProductId);
            if (product == null)
                throw new Exception("Product not found");

            var existingItem = await _unitOfWork.cartRepository.GetCartItemAsync(dto.ProductId, cart.Id);

            if (existingItem != null)
            {
                existingItem.Quantity += dto.Quantity;
                await _unitOfWork.cartRepository.UpdateAsync(existingItem);
                await _unitOfWork.SaveAsync();

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

            var addedItem = await _unitOfWork.cartRepository.GetCartItemWithProductAsync(newItem.ProductId, cart.Id);
            return _mapper.Map<CartItemDto>(addedItem);
        }
        public async Task<bool> DeleteItemFromCartAsync(string userId, int productId)
        {
            var cart = await _unitOfWork.cartRepository.GetCartByUserIdAsync(userId);
            if (cart == null) return false;  // this user have not any items in cart

            var item = await _unitOfWork.cartRepository.GetCartItemAsync(productId, cart.Id);
            if (item == null) return false;

            var product = await _unitOfWork.productRepository.GetByIdAsync(productId);
            if (product != null && product.QuantityInStock >= item.Quantity)
            {
                product.QuantityInStock -= item.Quantity;
                await _unitOfWork.productRepository.UpdateAsync(product);
            }

            await _unitOfWork.cartRepository.DeleteAsync(item);
            await _unitOfWork.SaveAsync();
            return true;
        }
        public async Task<bool> UpdateItemQuantityAsync(string userId, UpdateCartItemDto dto)
        {
            var cart = await _unitOfWork.cartRepository.GetCartByUserIdAsync(userId);
            if (cart == null) return false;

            var item = await _unitOfWork.cartRepository.GetCartItemAsync(dto.ProductId, cart.Id);
            if (item == null) return false;

            item.Quantity = dto.Quantity;
            await _unitOfWork.cartRepository.UpdateAsync(item);
            await _unitOfWork.SaveAsync();
            return true;
        }
        public async Task<bool> ClearCartAsync(string userId)
        {
            var cart = await _unitOfWork.cartRepository.GetCartWithItemsAsync(userId);
            if (cart == null || !cart.CartItems.Any()) return false;

            foreach (var item in cart.CartItems.ToList())
            {
                await _unitOfWork.cartRepository.DeleteAsync(item);
            }

            await _unitOfWork.SaveAsync();
            return true;
        }

        private async Task<ShoppingCart> CreateNewCart(string userId)
        {
            var newCart = new ShoppingCart
            {
                UserId = userId,
                CartItems = new List<CartItem>()
            };
            await _unitOfWork.shoppingCartRepository.AddAsync(newCart);
            await _unitOfWork.SaveAsync();
            return newCart;
        }

        //private decimal CalculateCartTotal(IEnumerable<CartItem> items)
        //{
        //    return items.Sum(item => item.Price * item.Quantity);
        //}
    }
}
