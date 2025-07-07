using E_Commerce.DomainLayer.Entities.Roles;

namespace E_Commerce.DomainLayer.Entities
{
    public class Order :BaseEntity
    {
        public string UserId { get; set; }
        public User User { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public decimal TotalPrice { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
    }
}
