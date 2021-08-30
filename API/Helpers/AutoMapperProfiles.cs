using API.DTOs;
using API.Entitites;
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
        }
    }
}