using System.ComponentModel.DataAnnotations.Schema;
namespace E_Commerce.DomainLayer.Entities
{
    public class OrderItem : BaseEntity
    {
        [ForeignKey("Order")]
        public int OrderId { get; set; }

        [ForeignKey("Product")]
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; } // snapshot of product price at order time
        public decimal Total => Quantity * UnitPrice;
        public Order Order { get; set; }
        public Product Product { get; set; }
    }
}
