using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entitites;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class StoresRepository : IStoresRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        public StoresRepository(DataContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }

        public void deleteArticlesFromStore(int id)
        {
            _context.Articles.RemoveRange(_context.Articles.Where(x=>x.storeId == id));
        }

        public async Task<IEnumerable<StoreDto>> getStoreDetails()
        {
            return await _context.Stores.ProjectTo<StoreDto>(_mapper.ConfigurationProvider).ToListAsync();
        }

        public async void setCrawlingDetails(StoreDto store)
        {
            Store st = await _context.Stores.FindAsync(store.Id);
            st.crawling = store.crawling;
            await _context.SaveChangesAsync();
        }

        public async void updateLastTimeCrawled(int storeId)
        {
            Store st = await _context.Stores.FindAsync(storeId);
            st.lastTimeCrawled = DateTime.Now;
            await _context.SaveChangesAsync();
        }
    }
}