using System.Collections.Generic;
using System.Threading.Tasks;
using API.DTOs;
using API.Entitites;
using API.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class ArticlesRepository : IArticlesRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public ArticlesRepository(DataContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }

        public Task<ArticleDto> getArticleAsync()
        {
            throw new System.NotImplementedException();
        }

        public async Task<IEnumerable<ArticleDto>> getArticlesAsync()
        {
            var articles = await _context.Articles.Include(p => p.imgSources).ToListAsync();
            return _mapper.Map<IEnumerable<ArticleDto>>(articles);
        }

        public Task<IEnumerable<ArticleImagesDto>> getPicturesForArticle(ArticleDto article)
        {
            throw new System.NotImplementedException();
        }

        public async void insertArrticles(ICollection<ArticleDto> articles)
        {
            foreach (var item in articles)
            {
                await _context.Articles.AddAsync(_mapper.Map<Article>(item));
            }
            await _context.SaveChangesAsync();
        }

        public void insertArticle(ArticleDto article)
        {
            throw new System.NotImplementedException();
        }

        public void Update(ArticleDto article)
        {
            _context.Entry(article).State = EntityState.Modified;
        }
    }
}