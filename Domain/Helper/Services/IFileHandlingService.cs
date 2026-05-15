using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Helper.Services
{
    public interface IFileHandlingService
    {
        Task<string> UploadFileAsync(Stream fileStream, string fileName, string folderName);
        void DeleteFile(string fileName, string folderName);
    }
}
