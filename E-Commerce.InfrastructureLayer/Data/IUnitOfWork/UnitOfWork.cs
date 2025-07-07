using E_Commerce.DomainLayer.Interfaces;
using E_Commerce.InfrastructureLayer.Data.DBContext;
using E_Commerce.InfrastructureLayer.Data.GenericClass;
using E_Commerce.InfrastructureLayer.Data.InterfacesImplementaion;

namespace E_Commerce.DomainLayer
{
    public class UnitOfWork : IUnitOfWork
    {
        #region
        private readonly ApplicationDBContext context;
        private IProductRepository _productRepository;
        private ICartRepository _cartRepository;
        private IShoppingCartRepository _shoppingCartRepository;
        private IOrderRepository _orderRepository;
        private ICartItemRepository _cartItemRepository;
        public UnitOfWork(ApplicationDBContext context)
        {
            this.context = context;
        }
        #endregion
        public IProductRepository productRepository 
        {
            get
            {
                if (_productRepository == null)
                    _productRepository = new ProductRepository(context);
                return _productRepository;
            }
            set => _productRepository = value;
        }
        public ICartRepository cartRepository
        {
            get
            {
                if (_cartRepository == null)
                    _cartRepository = new CartRepository(context);
                return _cartRepository;
            }
            set => _cartRepository = value;
        }
        public IShoppingCartRepository shoppingCartRepository
        {
            get
            {
                if(_shoppingCartRepository == null)
                    _shoppingCartRepository = new ShoppingCartRepository(context);
                return _shoppingCartRepository;
            }
            set
            {
                _shoppingCartRepository = value;
            }
        }
        public IOrderRepository OrdersRepository
        {
            get
            {
                if(_orderRepository == null)
                    _orderRepository = new OrderRepository(context);
                return _orderRepository;
            }
            set => _orderRepository = value; 
        }
        public ICartItemRepository CartItemsRepository 
        {
            get
            {
                if (_cartItemRepository == null)
                    _cartItemRepository = new CartItemRepository(context);
                return _cartItemRepository;

            }
            set => _cartItemRepository = value;
        }

        public async Task<bool> SaveAsync()
        {
            return await context.SaveChangesAsync() > 0; // return true -> if at least one row was modified on Database  
        }
    }
}
