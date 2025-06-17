using AutoMapper;
using E_Commerce.ApplicationLayer.Dtos.ProductCart;
using E_Commerce.DomainLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.ApplicationLayer.Mapper
{
    public class CartMappingProfile : Profile
    {
        public CartMappingProfile()
        {
            CreateMap<AddToCartDto, CartItem>();

            CreateMap<CartItem, CartItemDto>();
        }
    }
}
