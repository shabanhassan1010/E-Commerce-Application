using AutoMapper;
using E_Commerce.ApplicationLayer.Dtos.ProductCart;
using E_Commerce.ApplicationLayer.IService;
using E_Commerce.DomainLayer.Entities;
using E_Commerce.DomainLayer.Interfaces;


namespace E_Commerce.ApplicationLayer.Service
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IShoppingCartRepository _shoppingCartRepository;

        public CartService(ICartRepository cartRepository,ICartItemRepository cartItemRepository,
            IMapper mapper,IUnitOfWork unitOfWork , IShoppingCartRepository shoppingCartRepository)
        {
            _cartRepository = cartRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _shoppingCartRepository = shoppingCartRepository;
        }

        public async Task<CartResponseDto> GetCartAsync(string userId)
        {
            var cart = await _cartRepository.GetCartWithItemsAsync(userId);
            if (cart == null)
            {
                cart = await CreateNewCart(userId);
            }

            return new CartResponseDto
            {
                Id = cart.Id,
                Items = _mapper.Map<List<CartItemDto>>(cart.CartItems),
                //Total = CalculateCartTotal(cart.CartItems)
            };
        }

        public async Task<CartItemDto> AddItemToCartAsync(string userId, AddToCartDto dto)
        {
            var cart = await _cartRepository.GetCartByUserIdAsync(userId)
                       ?? await CreateNewCart(userId);

            var existingItem = await _cartRepository
                .GetCartItemAsync(dto.ProductId, cart.Id);

            if (existingItem != null)
            {
                existingItem.Quantity += dto.Quantity;
                await _cartRepository.UpdateAsync(existingItem);
            }
            else
            {
                var newItem = _mapper.Map<CartItem>(dto);
                newItem.ShoppingCartId = cart.Id;
                await _cartRepository.AddAsync(newItem);
            }

            await _unitOfWork.SaveAsync();

            var resultItem = existingItem ?? await _cartRepository
                .GetCartItemAsync(dto.ProductId, cart.Id);

            return _mapper.Map<CartItemDto>(resultItem);
        }

        public async Task<bool> RemoveItemFromCartAsync(string userId, int productId)
        {
            var cart = await _cartRepository.GetCartByUserIdAsync(userId);
            if (cart == null) return false;

            var item = await _cartRepository.GetCartItemAsync(productId, cart.Id);
            if (item == null) return false;

            await _cartRepository.DeleteAsync(item);
            await _unitOfWork.SaveAsync();
            return true;
        }

        public async Task<bool> UpdateItemQuantityAsync(string userId, UpdateCartItemDto dto)
        {
            var cart = await _cartRepository.GetCartByUserIdAsync(userId);
            if (cart == null) return false;

            var item = await _cartRepository.GetCartItemAsync(dto.ProductId, cart.Id);
            if (item == null) return false;

            item.Quantity = dto.Quantity;
            await _cartRepository.UpdateAsync(item);
            await _unitOfWork.SaveAsync();
            return true;
        }

        public async Task<bool> ClearCartAsync(string userId)
        {
            var cart = await _cartRepository.GetCartWithItemsAsync(userId);
            if (cart == null || !cart.CartItems.Any()) return false;

            foreach (var item in cart.CartItems.ToList())
            {
                await _cartRepository.DeleteAsync(item);
            }

            await _unitOfWork.SaveAsync();
            return true;
        }

        private async Task<ShoppingCart> CreateNewCart(string userId)
        {
            var newCart = new ShoppingCart
            {
                Id = Guid.NewGuid().ToString(),
                UserId = userId
            };
            await _shoppingCartRepository.AddAsync(newCart);
            await _unitOfWork.SaveAsync();
            return newCart;
        }

        //private decimal CalculateCartTotal(IEnumerable<CartItem> items)
        //{
        //    return items.Sum(item => item.Price * item.Quantity);
        //}
    }
}
