using Jewellery.Application.Master.Interfaces;
using Jewellery.Application.Master.Models;
using Jewellery.Application.Services.Interfaces;
using Jewellery.Application.Transactions.Interfaces;
using Jewellery.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Jewellery.Application.Master.Queries
{
    public class GetOnline_ProductByProductIdQuery : IRequest<ResponseModel>
    {
        public int ProductId { get; set; }
    }


    public class GetOnline_ProductByProductIdQueryHandler
        : IRequestHandler<GetOnline_ProductByProductIdQuery, ResponseModel>
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IErrorLogRepository _errorLogRepository;
        private readonly ICloudinaryStorageService _cloudinaryStorageService;

        public GetOnline_ProductByProductIdQueryHandler(
            ICustomerRepository customerRepository,
            IErrorLogRepository errorLogRepository,
            ICloudinaryStorageService cloudinaryStorageService)
        {
            _customerRepository = customerRepository;
            _errorLogRepository = errorLogRepository;
            _cloudinaryStorageService = cloudinaryStorageService;
        }

        public async Task<ResponseModel> Handle(
            GetOnline_ProductByProductIdQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                // =====================================================
                // BASIC VALIDATION
                // =====================================================

                if (request.ProductId <= 0)
                {
                    return new ResponseModel
                    {
                        Code = 0,
                        Message = "ProductId is required."
                    };
                }

                
                // =====================================================
                // DATABASE CALL (Multi result-set)
                // =====================================================

                var result = await _customerRepository.GetOnline_ProductByProductIdAsync(request.ProductId);

                if (result == null)
                {
                    return new ResponseModel
                    {
                        Code = 0,
                        Message = "Product not found."
                    };
                }

                // =====================================================
                // RESOLVE CLOUDINARY IMAGE URLS
                // =====================================================

                if (result.Images != null && result.Images.Count > 0)
                {
                    foreach (var image in (IEnumerable<dynamic>)result.Images)
                    {
                        if (!string.IsNullOrWhiteSpace(image.ImagePath))
                        {
                            var fileUrl = _cloudinaryStorageService.GetFileUrl(((string)image.ImagePath).Trim());
                            ((IDictionary<string, object>)image)["ImageUrl"] = fileUrl;
                        }
                    }
                }

                return new ResponseModel
                {
                    Code = 1,
                    Message = "SUCCESS",
                    Data = result
                };
            }
            catch (Exception ex)
            {
                var stackTrace = new StackTrace(ex, true);
                var frame = stackTrace.GetFrame(0);
                int? lineNumber = frame?.GetFileLineNumber();

                var errorLog = new ErrorLog
                {
                    ApiName = "GetOnline_ProductByProductIdQuery",
                    ErrorMessage = ex.Message,
                    StackTrace = ex.StackTrace,
                    LineNumber = lineNumber ?? 0,
                    CreatedDate = DateTime.Now
                };

                await _errorLogRepository.SaveErrorAsync(errorLog);

                return new ResponseModel
                {
                    Code = 0,
                    Message = "Something went wrong. Please try again later."
                };
            }
        }
    }
}