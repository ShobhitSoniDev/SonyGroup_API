using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jewellery.Application.Services.Interfaces
{
    public interface IBlobStorageService
    {
        Task<(bool Success, string FileUrl, string FileName, string Message)> UploadFileAsync(IFormFile file,string FileName,string FolderName, int expirySecond, int expiryMinutes, int expiryhour);
        Task<(bool Success, string FileUrl, string FileName, string Message)> GetFileUrl(string FileName, string FolderName, int expirySecond, int expiryMinutes, int expiryhour);
    }
}
