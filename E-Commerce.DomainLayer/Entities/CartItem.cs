namespace E_Commerce.DomainLayer.Entities
{
    public class CartItem
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string PictureUrl { get; set; } = string.Empty;
        public string? Brand { get; set; }
        public string? Type { get; set; }
        public string ShoppingCartId { get; set; } = string.Empty;
    }
}
