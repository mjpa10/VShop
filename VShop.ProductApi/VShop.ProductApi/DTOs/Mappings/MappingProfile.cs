using AutoMapper;
using VShop.ProductApi.Models;

namespace VShop.ProductApi.DTOs.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Product, ProductDTO>().ReverseMap();
        CreateMap<Category, CategoryDTO>().ReverseMap();
    }
}
