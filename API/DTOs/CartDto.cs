namespace API.DTOs
{
    public class CartDto
    {
        public int Id { get; set; }
        public AppUserDto AppUser { get; set; }
        public int AppUserId { get; set; }
        public ArticleDto Article { get; set; }
        public int ArticleId { get; set; }
        public int kolicina { get; set; }
    }
}