
using E_Commerce.DomainLayer.Entities.Roles;

namespace E_Commerce.ApplicationLayer.Dtos.Orders
{
    public class OrderItemDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public string? PictureUrl { get; set; }
        public decimal UnitPrice { get; set; }
        public OrderStatus Status { get; set; }
        public decimal Total => Quantity * UnitPrice;
    }
}
