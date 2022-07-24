using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace SSOService.Services.Repositories.NonRelational.Interfaces
{
    public interface IFileProcessor
    {
        Task<string> SaveFile(IFormFile file);
        Task<byte> GetFile(string location);
    }
}
