using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.DomainLayer.Interfaces
{
    public interface IUnitOfWork
    {
        IShoppingCartRepository shoppingCartRepository { get; set; }
        IProductRepository productRepository { get; set; }
        ICartRepository cartRepository { get; set; }
        IOrderRepository OrdersRepository { get; set; }
        ICartItemRepository CartItemsRepository { get; set; }
        Task<bool> SaveAsync();
    }
}
