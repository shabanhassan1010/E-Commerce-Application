using E_Commerce.DomainLayer.Interfaces;
using E_Commerce.InfrastructureLayer.Data.DBContext;
using E_Commerce.InfrastructureLayer.Data.GenericClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.DomainLayer.IUnitOfWork
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
    }
}
