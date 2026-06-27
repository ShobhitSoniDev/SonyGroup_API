using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Jewellery.Application.Common.Interfaces;
using Jewellery.Application.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Jewellery.Infrastructure.Services
{
    public class BlobStorageService : IBlobStorageService
    {
        private readonly BlobContainerClient _containerClient;
        private readonly string _accountName;
        private readonly string _accountKey;
        private readonly ICurrentUserService _currentUser;
        public BlobStorageService(IConfiguration config, ICurrentUserService currentUser)
        {
            var connectionString = config["AzureBlob:ConnectionString"];
            var containerName = config["AzureBlob:ContainerName"];

            _containerClient = new BlobContainerClient(connectionString, containerName);
            _containerClient.CreateIfNotExists();

            // extract for SAS
            _accountName = config["AzureBlob:AccountName"];
            _accountKey = config["AzureBlob:AccountKey"];
            _currentUser = currentUser;
        }

        // =========================
        // UPLOAD FILE
        // =========================
        public async Task<(bool Success, string FileUrl, string FileName, string Message)>UploadFileAsync(
    IFormFile? file, string FileName, string folderName, int expirySecond, int expiryMinutes, int expiryhour,
    byte[]? fileBytes = null,string? contentType = null)
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
                    return (false, "", "", "No file provided.");
                }

                folderName = _currentUser.shopCode + '/' + folderName.Trim().Trim('/');

                string fileNameWithFolder = $"{folderName}/{FileName}";

                var blobClient = _containerClient.GetBlobClient(fileNameWithFolder);

                using (stream)
                {
                    await blobClient.UploadAsync(stream, overwrite: true);
                }

                string fileUrl = GetSecureFileUrl(fileNameWithFolder,expirySecond,expiryMinutes,expiryhour, _currentUser.shopCode);

                return (true, fileUrl, FileName, "Uploaded successfully");
            }
            catch (Exception ex)
            {
                return (false, "", "", ex.Message);
            }
        }
        public string GetFileUrl(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                return null;

            return _containerClient.GetBlobClient(filePath).Uri.ToString();
        }
        // =========================
        // GET PUBLIC URL (NOT USED IN PRIVATE SETUP)
        // =========================
        public async Task<(bool Success, string FileUrl, string FileName, string Message)> GetFileUrl(string FileName, string folderName, int expirySecond, int expiryMinutes, int expiryhour)
        {
            if (string.IsNullOrWhiteSpace(FileName))
                return (false, "", FileName, "Please send file");

            string fileNameWithFolder = $"{folderName}/{FileName}";
            string fileUrl = GetSecureFileUrl(fileNameWithFolder, expirySecond, expiryMinutes, expiryhour);

            return (true, fileUrl, FileName, "Uploaded successfully");
        }

        // =========================
        // GET SECURE SAS URL (RECOMMENDED)
        // =========================
        public string GetSecureFileUrl(string fileName, int expirySecond = 0, int expiryMinutes = 0, int expiryHour = 0,string ShopCode="")
        {
            if(string.IsNullOrWhiteSpace(ShopCode))
            {
                fileName = _currentUser.shopCode + '/' + fileName.Trim().Trim('/');
            }
            else
            {
                fileName = fileName.Trim().Trim('/');
            }
            var blobClient = _containerClient.GetBlobClient(fileName);

            var credential = new StorageSharedKeyCredential(
                _accountName,
                _accountKey
            );

            DateTimeOffset expiryTime = DateTimeOffset.UtcNow;

            if (expirySecond > 0)
            {
                expiryTime = expiryTime.AddSeconds(expirySecond);
            }
            else if (expiryMinutes > 0)
            {
                expiryTime = expiryTime.AddMinutes(expiryMinutes);
            }
            else if (expiryHour > 0)
            {
                expiryTime = expiryTime.AddHours(expiryHour);
            }
            else
            {
                // default fallback (1 minute)
                expiryTime = expiryTime.AddMinutes(1);
            }
            var sasBuilder = new BlobSasBuilder
            {
                BlobContainerName = _containerClient.Name,
                BlobName = fileName,
                Resource = "b",
                ExpiresOn = expiryTime
            };

            sasBuilder.SetPermissions(BlobSasPermissions.Read);

            var sasToken = sasBuilder.ToSasQueryParameters(credential).ToString();

            return $"{blobClient.Uri}?{sasToken}";
        }
    }
}