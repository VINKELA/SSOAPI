using SSOService.Models;
using SSOService.Models.DTOs.Audit;
using SSOService.Models.DTOs.Auth;
using System.Threading.Tasks;

namespace SSOService.Services.General.Interfaces
{
    public interface IAuth
    {
        Task<Response<TokenDTO>> Login(LoginDTO user);

    }
}
