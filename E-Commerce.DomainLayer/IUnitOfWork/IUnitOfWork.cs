using E_Commerce.DomainLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.DomainLayer.IUnitOfWork
{
    public interface IUnitOfWork
    {
        public IProductRepository productRepository { get; set; }
    }
}
