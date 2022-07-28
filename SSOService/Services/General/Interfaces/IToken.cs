using SSOService.Models.DTOs.Auth;

namespace SSOService.Services.General.Interfaces
{
    public interface IToken
    {
        string BuildToken(string key, string issuer, LoginDTO user);
        bool ValidateToken(string key, string issuer, string audience, string token);
    }
}
