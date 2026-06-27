using Jewellery.Application.Master.Interfaces;
using Jewellery.Application.Master.Models;
using Jewellery.Domain.Entities;
using MediatR;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Jewellery.Application.Master.Commands
{
    // ✅ Command
    public class ProductMaster_ManageCommand : IRequest<ResponseModel>
    {
        public int ProductId { get; set; }
        public string ProductCode { get; set; } = "";
        public string ProductName { get; set; } = "";
        public int CategoryId { get; set; }
        public int MetalId { get; set; }
        public int? SupplierId { get; set; }
        public decimal GrossWeight { get; set; }
        public decimal NetWeight { get; set; }
        public decimal MakingCharge { get; set; }
        public string MakingChargeType { get; set; } = "";
        public int TotalQuantity { get; set; }
        public bool IsActive { get; set; } = true;
        public int TypeId { get; set; }
    }

    // ✅ Handler
    public class ProductMaster_ManageCommandHandler
        : IRequestHandler<ProductMaster_ManageCommand, ResponseModel>
    {
        private readonly IMasterRepository _masterRepository;
        private readonly IErrorLogRepository _errorLogRepository;
        public ProductMaster_ManageCommandHandler(IMasterRepository masterRepository, IErrorLogRepository errorLogRepository)
        {
            _masterRepository = masterRepository;
            _errorLogRepository = errorLogRepository;
        }

        public async Task<ResponseModel> Handle(ProductMaster_ManageCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (request.TypeId == 2 || request.TypeId == 3)
                {
                    var error = CommonInputValidator.Validate(request.ProductCode, false,false, 2, 30);
                    if (error.Code == 0) return error;

                    error = CommonInputValidator.Validate(request.ProductName,false, false, 2, 200);
                    if (error.Code == 0) return error;

                    error = CommonInputValidator.Validate(request.CategoryId.ToString(), true,false, 1, 10);
                    if (error.Code == 0) return error;

                    error = CommonInputValidator.Validate(request.MetalId.ToString(), true,false, 1, 10);
                    if (error.Code == 0) return error;

                    error = CommonInputValidator.Validate(request.GrossWeight.ToString(), true, true, 1, 20);
                    if (error.Code == 0) return error;

                    error = CommonInputValidator.Validate(request.NetWeight.ToString(), true, true, 1, 20);
                    if (error.Code == 0) return error;

                    error = CommonInputValidator.Validate(request.MakingCharge.ToString(), true, true, 1, 20);
                    if (error.Code == 0) return error;

                    error = CommonInputValidator.Validate(request.TotalQuantity.ToString(), true, false, 1, 10);
                    if (error.Code == 0) return error;
                }

                var model = new ProductMasterModel
                {
                    ProductId = request.ProductId,
                    ProductCode = request.ProductCode,
                    ProductName = request.ProductName,
                    CategoryId = request.CategoryId,
                    MetalId = request.MetalId,
                    SupplierId = request.SupplierId,
                    GrossWeight = request.GrossWeight,
                    NetWeight = request.NetWeight,
                    MakingCharge = request.MakingCharge,
                    MakingChargeType = request.MakingChargeType,
                    TotalQuantity = request.TotalQuantity,
                    IsActive = request.IsActive,
                    TypeId = request.TypeId
                };

                var result = await _masterRepository.ProductMaster_ManageAsync(model);

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

                var errorLog = new ErrorLog
                {
                    ApiName = "ProductMaster_ManageCommand",
                    ErrorMessage = ex.Message,
                    StackTrace = ex.StackTrace,
                    LineNumber = frame?.GetFileLineNumber() ?? 0,
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
