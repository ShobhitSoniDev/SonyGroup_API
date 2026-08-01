using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Jewellery.Application.Common.Interfaces;
using Jewellery.Application.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace Jewellery.Infrastructure.Services
{
    public class CloudinaryStorageService : ICloudinaryStorageService
    {
        private readonly Cloudinary _cloudinary;
        private readonly ICurrentUserService _currentUser;
        private readonly string _apiSecret;

        public CloudinaryStorageService(IConfiguration config, ICurrentUserService currentUser)
        {
            var cloudName = config["Cloudinary:CloudName"];
            var apiKey = config["Cloudinary:ApiKey"];
            var apiSecret = config["Cloudinary:ApiSecret"];

            var account = new Account(cloudName, apiKey, apiSecret);
            _cloudinary = new Cloudinary(account);
            _cloudinary.Api.Secure = true;

            _apiSecret = apiSecret;
            _currentUser = currentUser;
        }

        // =========================
        // UPLOAD FILE
        // =========================
        // NOTE: "FileName" in the returned tuple now carries the FULL Cloudinary
        // public_id (including shopCode/folder), because that is exactly what you
        // need later to build a correct signed URL. Save this value in your DB,
        // not just the original file name.
        // Also returns ResourceType ("image" | "video" | "raw") because that is
        // REQUIRED to build a correct URL later (Cloudinary URLs differ per resource type).
        public async Task<(bool Success, string FileUrl, string FileName, string ResourceType, string Message)> UploadFileAsync(
            IFormFile? file, string fileName, string folderName, int expirySecond = 0, int expiryMinutes = 0, int expiryHour = 0,
            byte[]? fileBytes = null, string? contentType = null)
        {
            try
            {
                Stream stream;

                if (file != null)
                {
                    stream = file.OpenReadStream();
                    contentType ??= file.ContentType;
                }
                else if (fileBytes != null)
                {
                    stream = new MemoryStream(fileBytes);
                    contentType ??= "application/pdf";
                }
                else
                {
                    return (false, "", "", "", "No file provided.");
                }

                folderName = _currentUser.shopCode + "/" + folderName.Trim().Trim('/');

                // Cloudinary public_id should not include the extension
                var fileNameNoExt = Path.GetFileNameWithoutExtension(fileName);

                using (stream)
                {
                    var uploadParams = new RawUploadParams
                    {
                        File = new FileDescription(fileName, stream),
                        PublicId = fileNameNoExt,
                        Folder = folderName,
                        Overwrite = true,
                        Type = "authenticated"
                    };



                    // resourceType "auto" works for images, pdfs, videos, etc.
                    var uploadResult = await _cloudinary.UploadAsync(uploadParams, "auto");

                    if (uploadResult.Error != null)
                    {
                        return (false, "", "", "", uploadResult.Error.Message);
                    }

                    // uploadResult.PublicId already includes the folder (e.g. shop1/products/abc)
                    // uploadResult.ResourceType tells us image/video/raw - store both!
                    var fullPublicId = uploadResult.PublicId;
                    var resourceType = uploadResult.ResourceType; // "image", "video" or "raw"

                    // Build an initial short-lived URL just so caller has *something* to show immediately.
                    // For actual access, always call GetSecureFileUrl(fullPublicId, resourceType, ...) fresh,
                    // since these URLs expire.
                    var fileUrl = GetSecureFileUrl(fullPublicId, resourceType, expirySecond, expiryMinutes, expiryHour, alreadyFullPublicId: true);

                    return (true, fileUrl, fullPublicId, resourceType, "Uploaded successfully");
                }
            }
            catch (Exception ex)
            {
                return (false, "", "", "", ex.Message);
            }
        }

        // =========================
        // GET PUBLIC (UNSIGNED) URL
        // =========================
        public string GetFileUrl(string publicId)
        {
            if (string.IsNullOrWhiteSpace(publicId))
                return null;

            var url = _cloudinary.Api.Url.ResourceType("raw").Type("authenticated").Secure(true).Signed(true).BuildUrl(publicId);
            return url; 
        }

        // =========================
        // GET SIGNED / EXPIRING URL (RECOMMENDED for private assets)
        // =========================
        // Requires the asset to be uploaded with Type = "authenticated".
        //
        // IMPORTANT PARAMS:
        // - publicId: pass the FULL public_id you got back from UploadFileAsync
        //   (the "FileName" field in the tuple). Do NOT pass just the original
        //   file name - it must include the folder/shopCode path exactly as
        //   stored in Cloudinary, otherwise the signature will never match and
        //   you'll get an invalid/expired-looking URL even right after upload.
        // - resourceType: "image", "video" or "raw" - also returned by UploadFileAsync.
        //   Passing the wrong resource type is the #1 reason these URLs "don't work".
        public string GetSecureFileUrl(
            string publicId,
            string resourceType = "image",
            int expirySecond = 0,
            int expiryMinutes = 0,
            int expiryHour = 0,
            string shopCode = "",
            bool alreadyFullPublicId = false)
        {
            if (!alreadyFullPublicId)
            {
                // Only prepend shopCode if the caller is giving us a bare/relative id.
                // If you already stored the FULL public_id (recommended), pass
                // alreadyFullPublicId: true and skip this entirely.
                var prefix = string.IsNullOrWhiteSpace(shopCode) ? _currentUser.shopCode : shopCode;
                publicId = prefix + "/" + publicId.Trim().Trim('/');
            }
            else
            {
                publicId = publicId.Trim().Trim('/');
            }

            long expirySeconds;
            if (expirySecond > 0) expirySeconds = expirySecond;
            else if (expiryMinutes > 0) expirySeconds = expiryMinutes * 60L;
            else if (expiryHour > 0) expirySeconds = expiryHour * 3600L;
            else expirySeconds = 300; // default fallback (5 minutes)

            var expiryTimestamp = DateTimeOffset.UtcNow.AddSeconds(expirySeconds).ToUnixTimeSeconds();

            var authToken = new AuthToken(_apiSecret)
            {
                expiration = expiryTimestamp
            };

            // Use the generic Url builder (NOT UrlImgUp) and explicitly set the
            // resource type that matches how the file was actually uploaded.
            var url = _cloudinary.Api.Url
                .ResourceType(resourceType)
                .Type("authenticated")
                .Signed(true)
                .Secure(true)
                .BuildUrl(publicId);


            // FIX: AuthToken.Generate() internally does culture-sensitive number
            // parsing/formatting. On machines/threads where CurrentCulture uses
            // non-standard digits (e.g. hi-IN), this throws
            // "Could not find any recognizable digits". Forcing InvariantCulture
            // just for this call guarantees it always works, regardless of
            // server/OS locale settings.
            //string token;
            //var originalCulture = CultureInfo.CurrentCulture;
            //var originalUiCulture = CultureInfo.CurrentUICulture;
            //try
            //{
            //    CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
            //    CultureInfo.CurrentUICulture = CultureInfo.InvariantCulture;

            //    token = authToken.Generate(new Uri(url).AbsolutePath);
            //}
            //finally
            //{
            //    CultureInfo.CurrentCulture = originalCulture;
            //    CultureInfo.CurrentUICulture = originalUiCulture;
            //}

            //return $"{url}?__cld_token__={token}";
            return url;
        }

        // =========================
        // DELETE FILE
        // =========================
        // publicId must be the FULL public_id (same value you saved from upload).
        // resourceType must match what was returned at upload time - otherwise
        // Cloudinary will report "not found" even though the file exists.
        public async Task<(bool Success, string Message)> DeleteFileAsync(string publicId, string resourceType = "image")
        {
            try
            {
                var deleteParams = new DeletionParams(publicId)
                {
                    ResourceType = resourceType switch
                    {
                        "video" => ResourceType.Video,
                        "raw" => ResourceType.Raw,
                        _ => ResourceType.Image
                    },
                    Type = "authenticated"
                };

                var result = await _cloudinary.DestroyAsync(deleteParams);

                return result.Result == "ok"
                    ? (true, "Deleted successfully")
                    : (false, result.Result);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }
    }
}
