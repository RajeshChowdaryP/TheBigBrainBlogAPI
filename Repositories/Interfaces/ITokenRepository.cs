using Microsoft.AspNetCore.Identity;

namespace TheBigBrainBlog.API.Repositories.Interfaces
{
    public interface ITokenRepository
    {
        string CreateJwtToken(IdentityUser user, List<string> roles);
    }
}
