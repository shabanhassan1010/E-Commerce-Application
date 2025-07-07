﻿
namespace E_Commerce.ApplicationLayer.Dtos.ProductCart
{
    public class CartItemDto
    {
        public int ProductId { get; set; }         
        public string ProductName { get; set; }    
        public decimal Price { get; set; }         
        public int Quantity { get; set; }        
        public string? PictureUrl { get; set; }     
        public int ShoppingCartId { get; set; } 
    }
}
