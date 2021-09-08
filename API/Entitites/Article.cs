using System.Collections.Generic;

namespace API.Entitites
{
    public class Article
    {
        public int Id { get; set; }
        public string href { get; set; }
        public string name { get; set; }
        public int price { get; set; }
        public char gender { get; set; }
        public string type { get; set; }
        public ICollection<ArticleImages> imgSources { get; set; }
        public int storeId { get; set; }
        public Store store;
    }
}