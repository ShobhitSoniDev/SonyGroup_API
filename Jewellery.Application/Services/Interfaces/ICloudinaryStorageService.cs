using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jewellery.Application.Services.Interfaces
{
    public interface ICloudinaryStorageService
    {
        // NOTE: "FileName" in the returned tuple is the FULL Cloudinary public_id
        // (shopCode/folder/name, WITHOUT extension) - save this exact value in your DB.
        // "ResourceType" ("image" | "video" | "raw") must also be saved - you need
        // it later to build a working signed URL or to delete the file.
        Task<(bool Success, string FileUrl, string FileName, string ResourceType, string Message)> UploadFileAsync(
            IFormFile? file,
            string fileName,
            string folderName,
            int expirySecond = 0,
            int expiryMinutes = 0,
            int expiryHour = 0,
            byte[]? fileBytes = null,
            string? contentType = null);

        // Returns direct (unsigned) secure_url for a given public_id / path
        string GetFileUrl(string publicId);

        // Returns a signed, time-limited URL.
        // publicId: pass the FULL public_id saved from UploadFileAsync.
        // resourceType: pass the ResourceType saved from UploadFileAsync ("image"/"video"/"raw").
        // alreadyFullPublicId: set true (recommended) when publicId already includes shopCode/folder,
        // so the method won't re-prepend shopCode and break the path.
        string GetSecureFileUrl(
            string publicId,
            string resourceType = "image",
            int expirySecond = 0,
            int expiryMinutes = 0,
            int expiryHour = 0,
            string shopCode = "",
            bool alreadyFullPublicId = false);

        // resourceType must match what was returned at upload time.
        Task<(bool Success, string Message)> DeleteFileAsync(string publicId, string resourceType = "image");
    }
}
