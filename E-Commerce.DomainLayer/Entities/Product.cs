﻿namespace E_Commerce.DomainLayer.Entities
{
    public class Product : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public  string PictureUrl { get; set; } = string.Empty;
        public  string Type { get; set; } = string.Empty;
        public  string Brand { get; set; } = string.Empty;
        public int QuantityInStock { get; set; }
    }
}
