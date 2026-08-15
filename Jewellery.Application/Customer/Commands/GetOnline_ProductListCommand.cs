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
    public class GetOnline_ProductListCommand : IRequest<ResponseModel>
    {
        public int? CategoryId { get; set; }

        public int? MetalId { get; set; }

        public string? SearchText { get; set; }

        public bool? OnlyFeatured { get; set; }

        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 20;
    }


    public class GetOnline_ProductListCommandHandler
        : IRequestHandler<GetOnline_ProductListCommand, ResponseModel>
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IErrorLogRepository _errorLogRepository;
        private readonly ICloudinaryStorageService _cloudinaryStorageService;

        public GetOnline_ProductListCommandHandler(
            ICustomerRepository customerRepository,
            IErrorLogRepository errorLogRepository,
            ICloudinaryStorageService cloudinaryStorageService)
        {
            _customerRepository = customerRepository;
            _errorLogRepository = errorLogRepository;
            _cloudinaryStorageService = cloudinaryStorageService;
        }

        public async Task<ResponseModel> Handle(
            GetOnline_ProductListCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                // =====================================================
                // BASIC VALIDATION
                // =====================================================

                if (request.PageNumber < 1)
                {
                    request.PageNumber = 1;
                }

                if (request.PageSize < 1 || request.PageSize > 100)
                {
                    request.PageSize = 20;
                }

                // =====================================================
                // MODEL MAP
                // =====================================================

                var model = new GetOnlineProductListModel
                {
                    CategoryId = request.CategoryId,
                    MetalId = request.MetalId,
                    SearchText = request.SearchText,
                    OnlyFeatured = request.OnlyFeatured,
                    PageNumber = request.PageNumber,
                    PageSize = request.PageSize
                };

                // =====================================================
                // DATABASE CALL
                // =====================================================

                var result = await _customerRepository.GetOnline_ProductListAsync(model);

                // =====================================================
                // RESOLVE CLOUDINARY IMAGE URL
                // =====================================================

                if (result != null && result.Count > 0)
                {
                    foreach (var product in (IEnumerable<dynamic>)result)
                    {
                        if (!string.IsNullOrWhiteSpace(product.PrimaryImage))
                        {
                            var fileUrl = _cloudinaryStorageService.GetFileUrl(product.PrimaryImage.Trim());
                            ((IDictionary<string, object>)product)["PrimaryImageUrl"] = fileUrl;
                        }
                    }
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
                var stackTrace = new StackTrace(ex, true);
                var frame = stackTrace.GetFrame(0);
                int? lineNumber = frame?.GetFileLineNumber();

                var errorLog = new ErrorLog
                {
                    ApiName = "GetOnline_ProductListCommand",
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