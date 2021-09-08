using API.DTOs;
using API.Entitites;
using API.Extensions;
using AutoMapper;

namespace API.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<ArticleDto, Article>().ReverseMap();
            CreateMap<ArticleImagesDto, ArticleImages>().ReverseMap();
            CreateMap<Store, StoreDto>().ReverseMap();
            CreateMap<Cart, CartDto>().ReverseMap();
            CreateMap<AppUser, AppUserDto>().ForMember(dest => dest.age, opt => opt.MapFrom(src => src.dateOfBirth.CalculateAge()));
            CreateMap<AppUserDto, AppUser>();
            CreateMap<AppUserUpdateDto, AppUser>();
            CreateMap<RegisterDto, AppUser>();
        }
    }
}