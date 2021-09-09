using System.Collections.Generic;
using System.Threading.Tasks;
using API.DTOs;

namespace API.Interfaces
{
    public interface IStoresRepository
    {
        Task<IEnumerable<StoreDto>> getStoreDetails();

        void setCrawlingDetails(StoreDto store);

        void updateLastTimeCrawled(int storeId);
        void deleteArticlesFromStore(int id);
    }
}