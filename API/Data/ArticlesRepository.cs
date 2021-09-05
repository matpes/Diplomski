using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entitites;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
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

        public async Task<ArticleDto> getArticleByIdAsync(int id)
        {
            var article = await _context.Articles.Include(p => p.imgSources).ProjectTo<ArticleDto>(_mapper.ConfigurationProvider).SingleAsync(x => x.Id == id);
            //return _mapper.Map<ArticleDto>(article);
            return article;
        }

        public async Task<PagedList<ArticleDto>> getArticlesAsync(ArticlesParams articleParams)
        {
            var query = _context.Articles.OrderBy(p => p.gender).ThenBy(p => p.type).ThenBy(p => p.price).Include(p => p.imgSources).ProjectTo<ArticleDto>(_mapper.ConfigurationProvider).AsNoTracking();
            return await PagedList<ArticleDto>.CreateAsync(query,articleParams.PageNumber, articleParams.pageSize);
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