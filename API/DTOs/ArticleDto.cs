using System.Collections.Generic;

namespace API.DTOs
{
    public class ArticleDto
    {
        public int Id { get; set; }
        public string href { get; set; }
        public string name { get; set; }
        public ICollection<ArticleImagesDto> imgSources { get; set; }
        public string price { get; set; }
        public char gender { get; set; }
        public string type { get; set; }
        public int storeId;
        public StoreDto store;
    }
}