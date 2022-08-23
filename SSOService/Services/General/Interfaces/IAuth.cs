using SSOService.Models;
using SSOService.Models.DTOs.Application;
using SSOService.Models.DTOs.Auth;
using System.Threading.Tasks;

namespace SSOService.Services.General.Interfaces
{
    public interface IAuth
    {
        Task<Response<TokenDTO>> Login(LoginDTO user);
        Task<Response<TokenDTO>> Login(AppLoginDTO app);
    }
}
