using AutoMapper;
using E_Commerce.ApplicationLayer.Dtos.ProductCart;
using E_Commerce.ApplicationLayer.IService;
using E_Commerce.DomainLayer.Entities;
using E_Commerce.DomainLayer.Interfaces;


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
                cart = await CreateNewCart(userId);

            return new CartResponseDto
            {
                Id = cart.Id,
                Items = _mapper.Map<List<CartItemDto>>(cart.CartItems),
                //Total = CalculateCartTotal(cart.CartItems)
            };
        }
        public async Task<CartItemDto> AddItemToCartAsync(string userId, AddToCartDto dto)
        {
            var checkCartIsFound = await _unitOfWork.shoppingCartRepository.GetByUserIdAsync(userId);
            if (checkCartIsFound == null)
                return null; // or throw a custom NotFoundException

            var newItem = _mapper.Map<CartItem>(dto);
            newItem.ShoppingCartId = checkCartIsFound.Id;

            await _unitOfWork.cartRepository.AddAsync(newItem);
            await _unitOfWork.SaveAsync();


            return _mapper.Map<CartItemDto>(newItem);
        }
        public async Task<bool> DeleteItemFromCartAsync(string userId, int productId)
        {
            var cart = await _unitOfWork.cartRepository.GetCartByUserIdAsync(userId);
            if (cart == null) return false;  // this user have not any items in cart

            var item = await _unitOfWork.cartRepository.GetCartItemAsync(productId, cart.Id);
            if (item == null) return false;

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
                Id = Guid.NewGuid().ToString(),
                UserId = userId
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
