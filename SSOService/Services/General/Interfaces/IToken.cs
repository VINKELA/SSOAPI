using SSOService.Models.DTOs.Application;
using SSOService.Models.DTOs.Auth;
using SSOService.Models.DTOs.User;
using System.Threading.Tasks;

namespace SSOService.Services.General.Interfaces
{
    public interface IToken
    {
        Task<TokenDTO> BuildToken(GetUserDTO user);
        Task<TokenDTO> BuildToken(GetApplicationDTO app);
        bool IsTokenValid(string token);

    }
}

