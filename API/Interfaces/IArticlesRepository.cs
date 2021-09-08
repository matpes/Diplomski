using System.Collections.Generic;
using System.Threading.Tasks;
using API.DTOs;
using API.Helpers;

namespace API.Interfaces
{
    public interface IArticlesRepository
    {
        void Update(ArticleDto article);
        void insertArticle(ArticleDto article);
        void insertArrticles(ICollection<ArticleDto> articles);
        Task<PagedList<ArticleDto>> getArticlesAsync(ArticlesParams articleParams);
        Task<ArticleDto> getArticleByIdAsync(int id);
        Task<IEnumerable<ArticleImagesDto>> getPicturesForArticle(ArticleDto article);
        void specialMethod();
        Task<string[]> getCategories();
    }
}