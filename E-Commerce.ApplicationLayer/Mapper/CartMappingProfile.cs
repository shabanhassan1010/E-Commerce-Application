using AutoMapper;
using E_Commerce.ApplicationLayer.Dtos.Orders;
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
            CreateMap<CartItem, AddToCartDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Product.Price))
                .ForMember(dest => dest.PictureUrl, opt => opt.MapFrom(src => src.Product.PictureUrl));

            CreateMap<CartItem, CartItemDto>()
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Product.Price))
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
                .ForMember(dest => dest.PictureUrl, opt => opt.MapFrom(src => src.Product.PictureUrl));

            CreateMap<ShoppingCart, CartResponseDto>()
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.CartItems));

            #region Order 
            CreateMap<Order, OrderDto>()
                 .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.OrderItems))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

            CreateMap<OrderItem, OrderItemDto>()
               .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
               .ForMember(dest => dest.PictureUrl, opt => opt.MapFrom(src => src.Product.PictureUrl))
               .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.UnitPrice)); 

            //.ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.));

            #endregion
        }
    }
}
