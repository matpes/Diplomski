using System.Collections.Generic;
using System.Threading.Tasks;
using API.DTOs;

namespace API.Interfaces
{
    public interface IArticlesRepository
    {
        void Update(ArticleDto article);
        void insertArticle(ArticleDto article);
        void insertArrticles(ICollection<ArticleDto> articles);
        Task<IEnumerable<ArticleDto>> getArticlesAsync();
        Task<ArticleDto> getArticleAsync();
        Task<IEnumerable<ArticleImagesDto>> getPicturesForArticle(ArticleDto article);

    }
}