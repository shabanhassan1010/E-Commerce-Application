using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace E_Commerce.ApplicationLayer.Dtos.ProductCart
{
    public class CartResponseDto
    {
        [Required]
        public string Id { get; set; }

        /// List of items in the cart
        [Required]
        public List<CartItemDto> Items { get; set; } = new List<CartItemDto>();

        /// Total value of all items in the cart
        [Range(0, double.MaxValue)]
        public decimal Total { get; set; }

        /// Total number of items in the cart
        [JsonIgnore] // Useful for business logic but not sent in API responses
        public int TotalItems => Items.Sum(i => i.Quantity);

        /// Calculates the cart total based on items
        public void CalculateTotal()
        {
            Total = Items.Sum(item => item.Quantity);
        }
    }
}
