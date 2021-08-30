namespace API.Entitites
{
    public class ArticleImages
    {
        public int Id { get; set; }
        public string src { get; set; }
        public Article Article { get; set; }
        public int ArticleId { get; set; }
    }
}