using System;
using System.Collections;
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
            var query = _context.Articles.Include(p => p.imgSources).AsQueryable();

            if (articleParams.Gender.Equals('M') || articleParams.Gender.Equals('F'))
            {
                query = query.Where(a => a.gender == articleParams.Gender);
            }
            if (articleParams.Categories != null && articleParams.Categories.Count() > 0)
            {
                query = query.Where(a => articleParams.Categories.Contains(a.type));
            }

            query = articleParams.Sort switch
            {
                0 => query.OrderByDescending(x => x.price),
                1 => query.OrderBy(x => x.price),
                2 => query.OrderByDescending(x => x.name),
                3 => query.OrderBy(x => x.name),
                4 => query.OrderByDescending(x => x.type),
                5 => query.OrderBy(x => x.type),
                _ => query.OrderByDescending(x => x.price)
            };

            return await PagedList<ArticleDto>.CreateAsync(query.ProjectTo<ArticleDto>(_mapper.ConfigurationProvider).AsNoTracking(),
             articleParams.PageNumber, articleParams.pageSize);
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

        public async Task<string[]> getCategories()
        {
            return await _context.Articles.Select(x => x.type).Distinct().ToArrayAsync();
        }

        public void specialMethod()
        {
            // var allArticles = _context.Articles.ToList();
            // foreach (var article in allArticles)
            // {
            //     if (article.price.IndexOf(" - ") != -1)
            //     {
            //         article.price = article.price.Remove(article.price.IndexOf(" - "));
            //     }
            //     var price = article.price;
            //     price = price.Replace(" RSD", "");
            //     price = price.Replace(".", "");
            //     article.price = price;
            // }
            // _context.SaveChanges();
        }

        public void Update(ArticleDto article)
        {
            _context.Entry(article).State = EntityState.Modified;
        }


        class SortingHelper : IComparer<string>
        {
            public int Compare(string x, string y)
            {
                return int.Parse(x).CompareTo(int.Parse(y));
            }
        }
    }
}