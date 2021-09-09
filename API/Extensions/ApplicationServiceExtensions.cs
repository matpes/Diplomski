using API.Data;
using API.Helpers;
using API.Interfaces;
using API.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace API.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddAplicationServices(this IServiceCollection services, IConfiguration config ){
            services.AddScoped<ITokenService, TokenService>();
            services.AddAutoMapper(typeof(AutoMapperProfiles).Assembly);
            services.AddScoped<IArticlesRepository, ArticlesRepository>();
            services.AddScoped<IUsersRepository, UsersRepository>();
            services.AddScoped<ICartsRepository, CartsRepository>();
            services.AddScoped<IStoresRepository, StoresRepository>();
            services.AddDbContext<DataContext>(options =>
            {
                options.UseSqlite(config.GetConnectionString("DefaultConnection"));
            });
            return services;
        }
    }
}