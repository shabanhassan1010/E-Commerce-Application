using E_Commerce.DomainLayer.Interfaces;
using E_Commerce.InfrastructureLayer.Data.DBContext;
using E_Commerce.InfrastructureLayer.Data.GenericClass;

namespace E_Commerce.DomainLayer
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDBContext context;
        private IProductRepository _productRepository;
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
