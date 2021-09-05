using System.Collections.Generic;
using System.Threading.Tasks;
using API.DTOs;
using API.Entitites;

namespace API.Interfaces
{
    public interface IUsersRepository
    {

        void Update(AppUser user);

        Task<bool> SaveAllAsync();

        Task<IEnumerable<AppUser>> GetUsersAsync();

        Task<AppUser> GetUserByIdAsync(int id);

        Task<AppUser> GetUserByUsernameAsync(string username);
        
        Task<IEnumerable<AppUserDto>> GetUsersDtoAsync();

        Task<AppUserDto> GetUserDtoByUsernameAsync(string username);

    }
}