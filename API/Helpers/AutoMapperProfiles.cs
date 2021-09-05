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
            CreateMap<ArticleDto, Article>();
            CreateMap<ArticleImagesDto, ArticleImages>();
            CreateMap<Article, ArticleDto>();
            CreateMap<ArticleImages, ArticleImagesDto>();
            CreateMap<Store, StoreDto>();
            CreateMap<StoreDto, Store>();
            CreateMap<AppUser, AppUserDto>().ForMember(dest => dest.age, opt => opt.MapFrom(src => src.dateOfBirth.CalculateAge()));
        }
    }
}