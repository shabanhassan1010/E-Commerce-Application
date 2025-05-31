using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.DomainLayer.Interfaces
{
    public interface IUnitOfWork
    {
        public IProductRepository productRepository { get; set; }
        Task<bool> SaveAsync();

    }
}
