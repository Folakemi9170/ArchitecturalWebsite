using ArchitecturalWebsite.Application.DTOs;
using ArchitecturalWebsite.Domain;
using AutoMapper;

namespace ArchitecturalWebsite
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Category, CategoryResponseDto>();
            CreateMap<CreateCategoryDto, Category>();
        }
    }
}
