using AutoMapper;
using System.Reflection.PortableExecutable;
using VShop.CartApi.Models;
using WingtipToys.Models;

namespace VShop.CartApi.DTOs.Maping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CartDTO, Cart>().ReverseMap();
            CreateMap<CartHeaderDTO, CartHeader>().ReverseMap();
            CreateMap<CartItemDTO, CartItem>().ReverseMap();
            CreateMap<ProductDTO, Product>().ReverseMap();
        }
    }
}
