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

namespace Jewellery.Application.Master.Commands
{
    public class CustomerWishlist_ManageCommand : IRequest<ResponseModel>
    {
        public int TypeId { get; set; }
        public int? ProductId { get; set; }
    }


    public class CustomerWishlist_ManageCommandHandler
        : IRequestHandler<CustomerWishlist_ManageCommand, ResponseModel>
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IErrorLogRepository _errorLogRepository;
        private readonly ICloudinaryStorageService _cloudinaryStorageService;

        public CustomerWishlist_ManageCommandHandler(
            ICustomerRepository customerRepository,
            IErrorLogRepository errorLogRepository,
            ICloudinaryStorageService cloudinaryStorageService)
        {
            _customerRepository = customerRepository;
            _errorLogRepository = errorLogRepository;
            _cloudinaryStorageService = cloudinaryStorageService;
        }

        public async Task<ResponseModel> Handle(
            CustomerWishlist_ManageCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                // =====================================================
                // BASIC VALIDATION
                // =====================================================

                if (request.TypeId != 1 && request.TypeId != 2)
                {
                    return new ResponseModel
                    {
                        Code = 0,
                        Message = "Invalid TypeId. Use 1 for Add/Remove or 2 for Get Wishlist."
                    };
                }

                //if (request.CustomerId <= 0)
                //{
                //    return new ResponseModel
                //    {
                //        Code = 0,
                //        Message = "CustomerId is required."
                //    };
                //}

                if (request.TypeId == 1 &&
                    (request.ProductId == null || request.ProductId <= 0))
                {
                    return new ResponseModel
                    {
                        Code = 0,
                        Message = "ProductId is required for wishlist add/remove."
                    };
                }

                // =====================================================
                // MODEL MAP
                // =====================================================

                var model = new CustomerWishlistManageModel
                {
                    TypeId = request.TypeId,
                    ProductId = request.ProductId
                };

                // =====================================================
                // DATABASE CALL
                // =====================================================

                var result = await _customerRepository.Customer_Wishlist_ManageAsync(model);

                // =====================================================
                // TYPE ID = 1 -> ADD / REMOVE (Output param based)
                // =====================================================

                if (request.TypeId == 1)
                {
                    return new ResponseModel
                    {
                        Code = 1,
                        Message = result.IsAdded
                            ? "Product added to wishlist."
                            : "Product removed from wishlist.",
                        Data = new { IsAdded = result.IsAdded }
                    };
                }

                // =====================================================
                // TYPE ID = 2 -> GET WISHLIST (Result set + image url)
                // =====================================================

                if (result.Wishlist != null)
                {
                    foreach (var item in (IEnumerable<dynamic>)result.Wishlist)
                    {
                        if (!string.IsNullOrWhiteSpace(item.PrimaryImage))
                        {
                            var fileUrl = _cloudinaryStorageService.GetFileUrl(((string)item.PrimaryImage).Trim());
                            ((IDictionary<string, object>)item)["PrimaryImageUrl"] = fileUrl;
                        }
                    }
                }

                return new ResponseModel
                {
                    Code = 1,
                    Message = "SUCCESS",
                    Data = result.Wishlist
                };
            }
            catch (Exception ex)
            {
                var stackTrace = new StackTrace(ex, true);
                var frame = stackTrace.GetFrame(0);
                int? lineNumber = frame?.GetFileLineNumber();

                var errorLog = new ErrorLog
                {
                    ApiName = "CustomerWishlist_ManageCommand",
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