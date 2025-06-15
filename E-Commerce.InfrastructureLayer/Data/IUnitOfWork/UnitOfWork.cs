using E_Commerce.DomainLayer.Interfaces;
using E_Commerce.InfrastructureLayer.Data.DBContext;
using E_Commerce.InfrastructureLayer.Data.GenericClass;
using E_Commerce.InfrastructureLayer.Data.InterfacesImplementaion;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace E_Commerce.DomainLayer
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDBContext context;
        private IProductRepository _productRepository;
        private ICartRepository _cartRepository;

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

        public UnitOfWork(ApplicationDBContext context)
        {
            this.context = context;
        }

        public async Task<bool> SaveAsync()
        {
            return await context.SaveChangesAsync() > 0; // return true -> if at least one row was modified on Database  
        }
    }
}
