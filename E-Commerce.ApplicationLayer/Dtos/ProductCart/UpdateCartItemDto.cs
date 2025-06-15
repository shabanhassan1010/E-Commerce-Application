using System.ComponentModel.DataAnnotations;

namespace E_Commerce.ApplicationLayer.Dtos.ProductCart
{
    public class UpdateCartItemDto
    {
        /// <summary>
        /// Product identifier
        /// </summary>
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Product ID must be positive")]
        public int ProductId { get; set; }

        /// <summary>
        /// New quantity for the product
        /// </summary>
        [Required]
        [Range(1, 100, ErrorMessage = "Quantity must be between 1 and 100")]
        public int Quantity { get; set; }
    }
}
