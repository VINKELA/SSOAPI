using Microsoft.AspNetCore.Http;
using SSOService.Models.Constants;
using SSOService.Models.Enums;
using SSOService.Services.Repositories.NonRelational.Interfaces;
using System.IO;
using System.Threading.Tasks;

namespace SSOService.Services.Repositories.NonRelational.Implementations
{
    public class FileServer : IFileRepository
    {
        public async Task<string> Save(IFormFile file, string name, FileType fileType)
        {

            var folderPath = GetFilePath(fileType);
            var filePath = Path.Combine(folderPath, name);
            if (file != null)
            {
                using var stream = File.Create(filePath);
                await file.CopyToAsync(stream);
            }
            // Process uploaded files
            // Don't rely on or trust the FileName property without validation.

            return filePath;
        }
        public byte[] Get(string path, FileType fileType)
        {
            if (string.IsNullOrEmpty(path)) return null;
            var file = File.ReadAllBytes(path);
            // Process uploaded files
            // Don't rely on or trust the FileName property without validation.
            return file;
        }

        private static string GetFilePath(FileType fileType)
        {
            string path = null;
            Initialize();
            switch (fileType)
            {
                case FileType.UserImage:
                    path = Path.Combine(FileConstants.StaticFolder, FileConstants.UserFolder);
                    break;
                case FileType.ClientLogo:
                    path = Path.Combine(FileConstants.StaticFolder, FileConstants.ClientFolder);
                    break;
            }
            return path;
        }
        private static void Initialize()
        {
            var directory = Directory.GetCurrentDirectory();
            if (!Directory.Exists(FileConstants.StaticFolder)) Directory.CreateDirectory(FileConstants.StaticFolder);
            var userFolder = Path.Combine(directory, FileConstants.StaticFolder, FileConstants.UserFolder);
            if (!Directory.Exists(userFolder)) Directory.CreateDirectory(userFolder);
            var clientFolder = Path.Combine(directory, FileConstants.StaticFolder, FileConstants.ClientFolder);
            if (!Directory.Exists(clientFolder)) Directory.CreateDirectory(clientFolder);
            return;
        }

    }

}

