using System.ComponentModel.DataAnnotations;

namespace E_Commerce.ApplicationLayer.Dtos.ProductCart
{
    public class UpdateCartItemDto
    {
        [Required]
        public int ProductId { get; set; }
        
        [Range(1, 100, ErrorMessage = "Quantity must be between 1 and 100")]
        [Required]
        public int Quantity { get; set; }
    }
}
