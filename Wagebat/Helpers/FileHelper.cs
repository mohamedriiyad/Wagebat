using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Wagebat.Helpers
{
    public class FileHelper
    {
        public static async Task<List<string>> UploadAll(List<IFormFile> inputFiles, string pathToSave)
        {
            if (inputFiles == null)
                return new List<string>();

            var files = new List<string>();
            foreach (var file in inputFiles)
            {
                var extension = Path.GetExtension(file.FileName);
                var fileName = Guid.NewGuid().ToString() + extension;

                var currentPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                var filePath = Path.Combine(pathToSave, fileName);
                var folderPath = Path.Combine(currentPath, pathToSave);

                // Check if the Path exist
                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                // Upload the file
                using var stream = new FileStream(Path.Combine(currentPath, filePath), FileMode.Create);
                await file.CopyToAsync(stream);

                files.Add(Path.Combine("\\", filePath));
            }

            return  files;
        }

        public static async Task<string> SaveFileToServer(IFormFile file, string serverPath)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return null;

                if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", serverPath)))
                    Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", serverPath));

                var extension = Path.GetExtension(file.FileName);
                var fileName = Guid.NewGuid().ToString() + extension;

                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", serverPath, fileName);

                using (FileStream bits = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(bits);
                }

                return (serverPath + @"/" + fileName).Replace("\\", "/");
            }
            catch (Exception e)
            {
                //_logger.LogError("File Helper Error : " + e.Message + " | Details : " + e.ToString());
                return null;
            }
        }

        public static async Task<string> SaveFileToServer(byte[] fileContent, string extension, string serverPath)
        {
            try
            {
                if (fileContent == null || fileContent.Length == 0)
                    return null;

                if (!Directory.Exists(serverPath))
                    Directory.CreateDirectory(serverPath);

                var newFileName = Guid.NewGuid().ToString() + extension;

                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", serverPath, newFileName);

                await File.WriteAllBytesAsync(path, fileContent);

                return (serverPath + @"/" + newFileName).Replace("\\", "/");
            }
            catch (Exception e)
            {
                //_logger.LogError("File Helper Error : " + e.Message + " | Details : " + e.ToString());
                return null;
            }
        }

        public static bool DeleteFile(string serverPath)
        {
            if (string.IsNullOrWhiteSpace(serverPath))
                return false;

            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", serverPath);
            if (File.Exists(path))
            {
                try
                {
                    File.Delete(path);
                    return true;
                }
                catch (Exception e)
                {
                    //_logger.LogError("File Helper Error : " + e.Message + " | Details : " + e.ToString());
                    return false;
                }
            }

            return false;
        }

    }
}

