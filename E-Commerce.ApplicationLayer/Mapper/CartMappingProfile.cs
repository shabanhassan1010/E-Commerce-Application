#region
using AutoMapper;
using E_Commerce.ApplicationLayer.Dtos.Orders;
using E_Commerce.ApplicationLayer.Dtos.Product.Read;
using E_Commerce.ApplicationLayer.Dtos.Product.Update;
using E_Commerce.ApplicationLayer.Dtos.Product.Write;
using E_Commerce.ApplicationLayer.Dtos.ProductCart;
using E_Commerce.DomainLayer.Entities;
#endregion

namespace E_Commerce.ApplicationLayer.Mapper
{
    public class CartMappingProfile : Profile
    {
        public CartMappingProfile()
        {

            #region CartItem
            CreateMap<CartItem, AddToCartDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Product.Price))
                .ForMember(dest => dest.PictureUrl, opt => opt.MapFrom(src => src.Product.PictureUrl));

            CreateMap<CartItem, CartItemDto>()
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Product.Price))
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
                .ForMember(dest => dest.PictureUrl, opt => opt.MapFrom(src => src.Product.PictureUrl));
            #endregion

            #region ShoppingCart
            CreateMap<ShoppingCart, CartResponseDto>()
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.CartItems));
          
            #endregion

            #region Product
            CreateMap<Product, GetProductDto>()
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.QuantityInStock));

            CreateMap<CreateProductDto, Product>();

            CreateMap<UpdateProductDto, Product>();
            #endregion

            #region Order 
            CreateMap<Order, OrderDto>()
                 .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.OrderItems))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));
            #endregion

            #region OrderItem 
            CreateMap<OrderItem, OrderItemDto>()
               .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
               .ForMember(dest => dest.PictureUrl, opt => opt.MapFrom(src => src.Product.PictureUrl))
               .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.UnitPrice)); 

            //.ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.));

            #endregion
        }
    }
}
