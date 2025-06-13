

namespace E_Commerce.DomainLayer.Entities
{
    public class ShoppingCart
    {
        public string Id { get; set; }
        public List<CartItem> cartItems { get; set; }
    }
}
