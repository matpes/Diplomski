namespace API.Entitites
{
    public class Cart
    {
        public int Id { get; set; }
        public AppUser AppUser { get; set; }
        public int AppUserId { get; set; }
        public Article Article { get; set; }
        public int ArticleId { get; set; }
        public int kolicina { get; set; }
    }
}