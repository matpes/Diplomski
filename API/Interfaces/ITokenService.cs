using API.Entitites;

namespace API.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(AppUser user);
    }
}