using Domain.Helper.Services;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Infrastructure.Helper
{
    public class FileHandlingService : IFileHandlingService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public FileHandlingService(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string folderName)
        {
            if (fileStream == null || fileStream.Length == 0)
                return null;

            // Define the path to the save folder inside wwwroot
            string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, folderName);

            // If the folder does not exist, create it
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            // Extract just the file name to prevent directory traversal attacks
            string safeFileName = Path.GetFileName(fileName);

            // Generate a unique file name to prevent overwriting existing files
            string uniqueFileName = Guid.NewGuid().ToString() + "_" + safeFileName;
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            // Copy the actual stream and save it to the destination path
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await fileStream.CopyToAsync(stream);
            }

            return uniqueFileName;
        }

        public void DeleteFile(string fileName, string folderName)
        {
            // Combine paths to get the exact location of the file
            string filePath = Path.Combine(_webHostEnvironment.WebRootPath, folderName, fileName);
            
            // Delete the file if it exists
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }
}
