using System.ComponentModel.DataAnnotations.Schema;

namespace E_Commerce.DomainLayer.Entities
{
    public class CartItem : BaseEntity
    {

        [ForeignKey("Product")]
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public int Quantity { get; set; }

        [ForeignKey("ShoppingCart")]
        public int ShoppingCartId { get; set; }
        public ShoppingCart ShoppingCart { get; set; }
    }
}
