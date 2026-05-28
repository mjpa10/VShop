using AutoMapper;

using VShop.ProductApi.Models;

namespace VShop.ProductApi.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Product, DTOs.ProductDTO>().ReverseMap();
        CreateMap<Category, DTOs.CategoryDTO>().ReverseMap();
    }
}
