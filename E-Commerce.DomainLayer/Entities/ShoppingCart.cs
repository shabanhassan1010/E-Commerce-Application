﻿

namespace E_Commerce.DomainLayer.Entities
{
    public class ShoppingCart
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string UserId { get; set; }
        public User User { get; set; }
        public List<CartItem> CartItems { get; set; } = new();
    }
}
