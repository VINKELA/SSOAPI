using SSOService.Models.DTOs.Auth;
using SSOService.Models.DTOs.User;
using System.Threading.Tasks;

namespace SSOService.Services.General.Interfaces
{
    public interface IToken
    {
        Task<TokenDTO> BuildToken(GetUserDTO user);
        bool IsTokenValid(string token);

    }
}

