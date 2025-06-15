
namespace E_Commerce.ApplicationLayer.Dtos.ProductCart
{
    public class AddToCartDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; } = 1;
        public string PictureUrl { get; set; }
        public string? Brand { get; set; }
        public string? Type { get; set; }
    }
}
