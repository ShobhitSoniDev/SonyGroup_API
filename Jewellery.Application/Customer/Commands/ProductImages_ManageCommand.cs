using Jewellery.Application.Auth.Interfaces;
using Jewellery.Application.Master.Interfaces;
using Jewellery.Application.Master.Models;
using Jewellery.Application.Services.Interfaces;
using Jewellery.Application.Transactions.Interfaces;
using Jewellery.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Jewellery.Application.Master.Commands
{
    public class ProductImages_ManageCommand
        : IRequest<ResponseModel>
    {
        public int TypeId { get; set; }

        public int? ProductId { get; set; }

        public int? ImageId { get; set; }

        public IFormFile? Image { get; set; }

        public bool? IsPrimary { get; set; }

        public int? DisplayOrder { get; set; }
    }


    public class ProductImages_ManageCommandHandler : IRequestHandler<ProductImages_ManageCommand, ResponseModel>
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IErrorLogRepository _errorLogRepository;
        private readonly ICloudinaryStorageService _cloudinaryStorageService;

        public ProductImages_ManageCommandHandler(ICustomerRepository customerRepository,IErrorLogRepository errorLogRepository,ICloudinaryStorageService cloudinaryStorageService)
        {
            _customerRepository = customerRepository;
            _errorLogRepository = errorLogRepository;
            _cloudinaryStorageService = cloudinaryStorageService;
        }


        public async Task<ResponseModel> Handle(ProductImages_ManageCommand request,CancellationToken cancellationToken)
        {
            try
            {
                // =====================================================
                // BASIC VALIDATION
                // =====================================================

                if (request.TypeId < 1 ||
                    request.TypeId > 4)
                {
                    return new ResponseModel
                    {
                        Code = 0,
                        Message = "Invalid TypeId. Use 1-4."
                    };
                }
                // =====================================================
                // TYPE ID = 1
                // ADD / UPDATE SINGLE IMAGE
                // =====================================================

                if (request.TypeId == 1)
                {
                    if (request.ProductId == null ||
                        request.ProductId <= 0)
                    {
                        return new ResponseModel
                        {
                            Code = 0,
                            Message = "ProductId is required."
                        };
                    }

                    // New image ke liye Image required
                    if (!request.ImageId.HasValue &&
                        (request.Image == null ||
                         request.Image.Length == 0))
                    {
                        return new ResponseModel
                        {
                            Code = 0,
                            Message =
                                "Image is required for new image."
                        };
                    }


                    string? imagePath = null;


                    // =================================================
                    // IMAGE UPLOAD
                    // =================================================

                    if (request.Image != null && request.Image.Length > 0)
                    {
                        var extension =Path.GetExtension(request.Image.FileName).ToLowerInvariant();


                        var allowedExtensions =
                            new[]
                            {
                                ".jpg",
                                ".jpeg",
                                ".png"
                            };

                        if (!allowedExtensions.Contains(extension))
                        {
                            return new ResponseModel
                            {
                                Code = 0,
                                Message =
                                    "Only jpg, jpeg and png images are allowed."
                            };
                        }


                        // Dynamic 4 digit code
                        int randomCode =Random.Shared.Next(1000,10000);


                        string fileName =
                            $"Product_{randomCode}{extension}";

                        string folderName = "ProductImages";
                        // Cloudinary upload
                        var uploadResult =await _cloudinaryStorageService.UploadFileAsync(request.Image,fileName,folderName,0,1,0);


                        if (!uploadResult.Success)
                        {
                            return new ResponseModel
                            {
                                Code = 0,
                                Message =
                                    "Image upload failed."
                            };
                        }


                        imagePath =
                            uploadResult.FileName;
                    }


                    // =================================================
                    // DB MODEL
                    // =================================================

                    var model =
                        new ProductImagesManageModel
                        {
                            TypeId =
                                request.TypeId,

                            ProductId =
                                request.ProductId,

                            ImageId =
                                request.ImageId,

                            ImagePath =
                                imagePath,

                            IsPrimary =
                                request.IsPrimary,

                            DisplayOrder =
                                request.DisplayOrder
                        };


                    // =================================================
                    // DATABASE
                    // =================================================

                    var result =await _customerRepository.Product_Images_ManageAsync(model);


                    if (request.TypeId == 3 && result != null && result.Count > 0)
                    {
                        foreach (var productimage in (IEnumerable<dynamic>)result)
                        {
                            if (!string.IsNullOrWhiteSpace(productimage.ImagePath))
                            {
                                var files = _cloudinaryStorageService.GetFileUrl(productimage.ImagePath.Trim());
                                ((IDictionary<string, object>)productimage)["PrimaryImageUrl"] = files;
                            }
                        }
                    }

                    return new ResponseModel
                    {
                        Code =
                            result != null ? 1 : 0,

                        Message =
                            result != null
                                ? "Product image saved successfully."
                                : "Failed to save product image.",

                        Data =
                            result
                    };
                }


                // =====================================================
                // TYPE ID = 2
                // DELETE IMAGE
                // =====================================================

                if (request.TypeId == 2)
                {
                    if (request.ImageId == null ||
                        request.ImageId <= 0)
                    {
                        return new ResponseModel
                        {
                            Code = 0,
                            Message =
                                "ImageId is required."
                        };
                    }


                    var model =
                        new ProductImagesManageModel
                        {
                            TypeId =
                                request.TypeId,

                            ProductId =
                                request.ProductId,

                            ImageId =
                                request.ImageId,

                            ImagePath =
                                null,

                            IsPrimary =
                                null,

                            DisplayOrder =
                                null
                        };


                    var result =
                        await _customerRepository
                            .Product_Images_ManageAsync(
                                model);


                    return new ResponseModel
                    {
                        Code =
                            result != null ? 1 : 0,

                        Message =
                            result != null
                                ? "Image deleted successfully."
                                : "Failed to delete image.",

                        Data =
                            result
                    };
                }


                // =====================================================
                // TYPE ID = 3
                // SET PRIMARY IMAGE
                // =====================================================

                if (request.TypeId == 3)
                {
                    if (request.ImageId == null ||
                        request.ImageId <= 0)
                    {
                        return new ResponseModel
                        {
                            Code = 0,
                            Message =
                                "ImageId is required."
                        };
                    }


                    var model =
                        new ProductImagesManageModel
                        {
                            TypeId =
                                request.TypeId,

                            ProductId =
                                request.ProductId,

                            ImageId =
                                request.ImageId,

                            ImagePath =
                                null,

                            IsPrimary =
                                true,

                            DisplayOrder =
                                null
                        };


                    var result =
                        await _customerRepository
                            .Product_Images_ManageAsync(
                                model);


                    return new ResponseModel
                    {
                        Code =
                            result != null ? 1 : 0,

                        Message =
                            result != null
                                ? "Primary image updated successfully."
                                : "Failed to update primary image.",

                        Data =
                            result
                    };
                }


                // =====================================================
                // TYPE ID = 4
                // GET PRODUCT IMAGES
                // =====================================================

                if (request.TypeId == 4)
                {
                    if (request.ProductId == null ||
                        request.ProductId <= 0)
                    {
                        return new ResponseModel
                        {
                            Code = 0,
                            Message =
                                "ProductId is required."
                        };
                    }


                    var model =
                        new ProductImagesManageModel
                        {
                            TypeId =
                                request.TypeId,

                            ProductId =
                                request.ProductId,

                            ImageId =
                                null,

                            ImagePath =
                                null,

                            IsPrimary =
                                null,

                            DisplayOrder =
                                null
                        };


                    var result =
                        await _customerRepository
                            .Product_Images_ManageAsync(
                                model);

                    if (request.TypeId == 4 && result != null && result.Count > 0)
                    {
                        foreach (var productimage in (IEnumerable<dynamic>)result)
                        {
                            if (!string.IsNullOrWhiteSpace(productimage.ImagePath))
                            {
                                    var files = _cloudinaryStorageService.GetFileUrl(productimage.ImagePath.Trim());
                                ((IDictionary<string, object>)productimage)["ImagePathUrls"] = files;
                            }
                        }
                    }
                    return new ResponseModel
                    {
                        Code =
                            result != null ? 1 : 0,

                        Message =
                            result != null
                                ? "SUCCESS"
                                : "FAILED",

                        Data =
                            result
                    };
                }


                return new ResponseModel
                {
                    Code = 0,
                    Message = "Invalid TypeId."
                };
            }
            catch (Exception ex)
            {
                var stackTrace =
                    new StackTrace(
                        ex,
                        true);

                var frame =
                    stackTrace.GetFrame(0);

                int? lineNumber =
                    frame?.GetFileLineNumber();


                var errorLog =
                    new ErrorLog
                    {
                        ApiName =
                            "ProductImages_ManageCommand",

                        ErrorMessage =
                            ex.Message,

                        StackTrace =
                            ex.StackTrace,

                        LineNumber =
                            lineNumber ?? 0,

                        CreatedDate =
                            DateTime.Now
                    };


                await _errorLogRepository
                    .SaveErrorAsync(
                        errorLog);


                return new ResponseModel
                {
                    Code = 0,
                    Message =
                        "Something went wrong. Please try again later."
                };
            }
        }
    }
}
