using Microsoft.AspNetCore.Http;
using SSOService.Models.Enums;
using System.Threading.Tasks;

namespace SSOService.Services.Repositories.NonRelational.Interfaces
{
    public interface IFileRepository
    {
        Task<string> Save(IFormFile file, string name, FileType fileType);
        byte[] Get(string path, FileType fileType);
    }
}
