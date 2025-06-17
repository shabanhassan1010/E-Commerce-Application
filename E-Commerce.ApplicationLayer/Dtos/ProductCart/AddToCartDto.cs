
namespace E_Commerce.ApplicationLayer.Dtos.ProductCart
{
    public class AddToCartDto
    {
        public int ProductId { get; set; }         // المنتج المرتبط
        public string ProductName { get; set; }    // اسم المنتج
        public decimal Price { get; set; }         // سعر المنتج
        public int Quantity { get; set; }          // الكمية
        public string PictureUrl { get; set; }     // صورة المنتج
    }
}
