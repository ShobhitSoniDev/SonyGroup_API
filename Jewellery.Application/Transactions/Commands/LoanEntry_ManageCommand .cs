using Jewellery.Application.Auth.Interfaces;
using Jewellery.Application.Master.Interfaces;
using Jewellery.Application.Services.Interfaces;
using Jewellery.Application.Transactions.Interfaces;
using Jewellery.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO.IsolatedStorage;

namespace Jewellery.Application.Transactions.Commands
{
    public class LoanEntry_ManageCommand : IRequest<ResponseModel>
    {
        public string LoanId { get; set; } = string.Empty;

        public string CustomerCode { get; set; } = string.Empty;

        public string LoanType { get; set; } = string.Empty;

        public decimal Amount { get; set; } = 0;

        public string InterestType { get; set; } = string.Empty;

        public decimal InterestRate { get; set; } = 0;

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Duration { get; set; } = string.Empty;
        public string? MetalType { get; set; } = string.Empty;

        public decimal? Weight { get; set; } = 0;

        public string? ItemCount { get; set; } = string.Empty;

        public string? Description { get; set; } = string.Empty;
        public string TypeId { get; set; } = string.Empty;

        public List<IFormFile>? Photos { get; set; }
    }
    public class LoanEntry_ManageCommandHandler
        : IRequestHandler<LoanEntry_ManageCommand, ResponseModel>
    {
        private readonly ILoanRepository _loanRepository;
        private readonly IBlobStorageService _blobStorageService;

        public LoanEntry_ManageCommandHandler(ILoanRepository loanRepository, IBlobStorageService blobStorageService)
        {
            _loanRepository = loanRepository;
            _blobStorageService = blobStorageService;
        }

        public async Task<ResponseModel> Handle(LoanEntry_ManageCommand request, CancellationToken cancellationToken)
        {
            try
            {
                string folderName = "LoanProof";
                // ✅ VALIDATION (FIXED)
                if (request.TypeId != "3" && request.TypeId != "5")
                {
                    if (string.IsNullOrEmpty(request.CustomerCode))
                        return new ResponseModel { Code = 0, Message = "Invalid CustomerCode" };
                }
                if (request.TypeId == "1" || request.TypeId == "2")
                {
                    if (request.Amount <= 0)
                        return new ResponseModel { Code = 0, Message = "Invalid Amount" };

                    if (request.InterestRate < 0)
                        return new ResponseModel { Code = 0, Message = "Invalid Interest Rate" };

                    if (string.IsNullOrWhiteSpace(request.LoanType))
                        return new ResponseModel { Code = 0, Message = "LoanType required" };

                    if (request.LoanType.ToLower() == "girvi")
                    {
                        if (string.IsNullOrWhiteSpace(request.MetalType))
                            return new ResponseModel { Code = 0, Message = "MetalType required for Girvi" };

                        if (request.Weight <= 0)
                            return new ResponseModel { Code = 0, Message = "Invalid Weight" };
                    }

                }
                // 📁 FILE UPLOAD (SAFE VERSION)
                var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "loan");

                if (!Directory.Exists(uploadPath))
                    Directory.CreateDirectory(uploadPath);

                var fileNames = new List<string>();
                var fileUrls = new List<string>();

                //if (request.Photos != null && request.Photos.Any())
                //{
                //    foreach (var file in request.Photos)
                //    {
                //        if (file == null || file.Length == 0) continue;

                //        var ext = Path.GetExtension(file.FileName);
                //        var allowedExt = new[] { ".jpg", ".jpeg", ".png" };

                //        if (!allowedExt.Contains(ext.ToLower()))
                //            continue;

                //        var fileName = $"{Guid.NewGuid()}{ext}";
                //        var filePath = Path.Combine(uploadPath, fileName);

                //        using (var stream = new FileStream(filePath, FileMode.Create))
                //        {
                //            await file.CopyToAsync(stream, cancellationToken);
                //        }

                //        fileNames.Add(fileName);
                //    }
                //}
                if (request.Photos != null && request.Photos.Any())
                {
                    foreach (var file in request.Photos)
                    {
                        if (file == null || file.Length == 0)
                            continue;

                        var ext = Path.GetExtension(file.FileName);
                        var allowedExt = new[] { ".jpg", ".jpeg", ".png" };

                        if (!allowedExt.Contains(ext.ToLower()))
                            continue;

                        // ☁️ CALL AZURE BLOB METHOD
                        string FileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                        var uploadResult = await _blobStorageService.UploadFileAsync(file, FileName, folderName, 0, 1, 0); // Second , Minute ,Hour

                        if (uploadResult.Success)
                        {
                            fileNames.Add(uploadResult.FileName);
                            fileUrls.Add(uploadResult.FileUrl); // optional
                        }
                        else
                        {
                            // optional: log error or skip
                        }
                    }
                }
                // 📦 MAP MODEL
                var model = new LoanEntryModel
                {
                    LoanId = request.LoanId,
                    CustomerCode = request.CustomerCode,
                    LoanType = request.LoanType,
                    Amount = request.Amount,
                    InterestType = request.InterestType,
                    InterestRate = request.InterestRate,
                    StartDate = request.StartDate?.ToString("yyyy-MM-dd") ?? "",
                    EndDate = request.EndDate?.ToString("yyyy-MM-dd") ?? "",
                    Duration = request.Duration,
                    MetalType = request.MetalType,
                    Weight = request.Weight ?? 0,
                    ItemCount = request.ItemCount,
                    Description = request.Description,
                    PhotoPath = string.Join(",", fileNames),
                    TypeId = request.TypeId
                };

                var result = await _loanRepository.LoanEntry_ManageAsync(model);

                if (request.TypeId == "5" && result != null && result.Count > 0)
                {
                    var loan = ((IEnumerable<dynamic>)result).First();

                    var photoUrls = new List<string>();

                    if (!string.IsNullOrWhiteSpace(loan.Photos))
                    {
                        foreach (var photoName in loan.Photos.Split(',', StringSplitOptions.RemoveEmptyEntries))
                        {
                            var file = await _blobStorageService.GetFileUrl(
                                photoName.Trim(),
                                folderName,
                                0,
                                5,
                                0
                            );

                            photoUrls.Add(file.Item2);
                        }
                    }
((IDictionary<string, object>)loan)["PhotoUrls"] = photoUrls;

                }
                return new ResponseModel
                {
                    Code = result != null ? 1 : 0,
                    Message = result != null ? "SUCCESS" : "FAILED",
                    Data = result
                };
            }
            catch (Exception ex)
            {
                // ✅ LOG ERROR (IMPORTANT)
                return new ResponseModel
                {
                    Code = 0,
                    Message = ex.Message
                };
            }
        }
    }
}