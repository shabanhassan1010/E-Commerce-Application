

namespace E_Commerce.DomainLayer.Entities
{
    public class ShoppingCart : BaseEntity
    {
        public string UserId { get; set; }
        public User User { get; set; }
        public ICollection<CartItem> CartItems { get; set; }
    }
}
