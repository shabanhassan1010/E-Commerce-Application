
using System.ComponentModel.DataAnnotations;

namespace E_Commerce.ApplicationLayer.Dtos.Product.Update
{
    public class UpdateProductDto
    {
        [Required(ErrorMessage = "Product name is required")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Product name must be between 3 and 100 characters")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Product description is required")]
        [StringLength(500, MinimumLength = 10, ErrorMessage = "Product description must be between 10 and 500 characters")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Product price is required")]
        [Range(0.01, 1000000, ErrorMessage = "Price must be greater than 0 and less than 1,000,000")]
        public decimal Price { get; set; }

        //[Required(ErrorMessage = "Product image URL is required")]
        //[Url(ErrorMessage = "Invalid URL format")]
        //[StringLength(500, ErrorMessage = "URL cannot exceed 500 characters")]
        public string PictureUrl { get; set; } = string.Empty;

        [Required(ErrorMessage = "Product type is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Product type must be between 2 and 50 characters")]
        public string Type { get; set; } = string.Empty;

        [Required(ErrorMessage = "Product brand is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Product brand must be between 2 and 50 characters")]
        public string Brand { get; set; } = string.Empty;
        public int Quantity { get; set; }
    }
}
