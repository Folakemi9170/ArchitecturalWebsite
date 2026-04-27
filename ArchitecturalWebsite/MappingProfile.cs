using ArchitecturalWebsite.Application.DTOs;
using ArchitecturalWebsite.Domain;
using AutoMapper;
using static ArchitecturalWebsite.Application.DTOs.FAQDto;
using static ArchitecturalWebsite.Application.DTOs.ReviewDto;

namespace ArchitecturalWebsite
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Category, CategoryResponseDto>();
            CreateMap<CreateCategoryDto, Category>()
                 .ForMember(dest => dest.Id, opt => opt.Ignore())
                 .ForMember(dest => dest.Services, opt => opt.Ignore());

            CreateMap<Service, ServiceResponseDto>();
            CreateMap<Service, ServiceDetailResponseDto>();
            CreateMap<Service, ServicesByCategoryDto>();
            CreateMap<CreateServiceDto, Service>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Category, opt => opt.Ignore())
                .ForMember(dest => dest.ImageUrl, opt => opt.Ignore());

            CreateMap<Booking, BookingResponseDto>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));
            CreateMap<CreateBookingDto, Booking>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Service, opt => opt.Ignore());

            CreateMap<Enquiry, EnquiryResponseDto>();
            CreateMap<CreateEnquiryDto, Enquiry>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.IsRead, opt => opt.MapFrom(_ => false));

            CreateMap<FAQ, FAQResponseDto>();
            CreateMap<CreateFAQDto, FAQ>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());
            CreateMap<UpdateFAQDto, FAQ>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());

            CreateMap<Reviews, ReviewResponseDto>();
            CreateMap<CreateReviewDto, Reviews>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.IsApproved, opt => opt.MapFrom(_ => false));
            CreateMap<UpdateReviewDto, Reviews>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.ClientName, opt => opt.Ignore())
                .ForMember(dest => dest.IsApproved, opt => opt.Ignore());
        }
    }
}
