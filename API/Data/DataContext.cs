using API.Entitites;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class DataContext : DbContext
    {
        public DataContext( DbContextOptions options) : base(options)
        {
        }

        public DbSet<AppUser> Users { get; set; }
        public DbSet<Article> Articles {get; set;}
        public DbSet<ArticleImages> ArticleImages {get; set;}
        public DbSet<Store> Stores {get; set;} 
        public DbSet<Cart> Carts {get; set;} 

    }
}