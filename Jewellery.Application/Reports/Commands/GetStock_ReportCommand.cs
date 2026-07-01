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
    // ─── Command ──────────────────────────────────────────────────────────────
    public class GetStockReportCommand : IRequest<ResponseModel>
    {
        public DateTime? AsOnDate { get; set; }   // Stock as of this date (inclusive) | NULL = as of today
        public int? ProductId { get; set; }   // NULL = all products
        public int? CategoryId { get; set; }   // NULL = all categories
        public int? MetalId { get; set; }   // NULL = all metals (Product_Master.MetalId)
        public string? MetalType { get; set; }   // "GOLD" | "SILVER" | NULL = all
        public string? StockStatus { get; set; }   // "IN_STOCK" | "OUT_OF_STOCK" | "LOW_STOCK" | NULL = all
        public int? LowStockQty { get; set; }   // Threshold used when StockStatus = "LOW_STOCK" (default 5 in SP)
        public bool? IsActive { get; set; }   // NULL = both active & inactive
    }

    // ─── Handler ──────────────────────────────────────────────────────────────
    public class GetStockReportCommandHandler
        : IRequestHandler<GetStockReportCommand, ResponseModel>
    {
        private readonly IReportsRepository _reportsRepository;
        private readonly IErrorLogRepository _errorLogRepository;

        public GetStockReportCommandHandler(
            IReportsRepository reportsRepository,
            IErrorLogRepository errorLogRepository)
        {
            _reportsRepository = reportsRepository;
            _errorLogRepository = errorLogRepository;
        }

        public async Task<ResponseModel> Handle(
            GetStockReportCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                var model = new GetStockReportModel
                {
                    AsOnDate = request.AsOnDate,
                    ProductId = request.ProductId,
                    CategoryId = request.CategoryId,
                    MetalId = request.MetalId,
                    MetalType = request.MetalType,
                    StockStatus = request.StockStatus,
                    LowStockQty = request.LowStockQty,
                    IsActive = request.IsActive
                };

                var result = await _reportsRepository.StockReportAsync(model);

                if (result != null)
                {
                    return new ResponseModel
                    {
                        Code = 1,
                        Message = "SUCCESS",
                        Data = result
                    };
                }
                else
                {
                    return new ResponseModel
                    {
                        Code = 0,
                        Message = "FAILED"
                    };
                }
            }
            catch (Exception ex)
            {
                var stackTrace = new StackTrace(ex, true);
                var frame = stackTrace.GetFrame(0);
                int? lineNumber = frame?.GetFileLineNumber();
                string? stackTraceText = ex.StackTrace;

                var errorLog = new ErrorLog
                {
                    ApiName = "GetStockReportCommand",
                    ErrorMessage = ex.Message,
                    StackTrace = stackTraceText,
                    LineNumber = lineNumber ?? 0,
                    CreatedDate = DateTime.Now
                };

                _errorLogRepository.SaveErrorAsync(errorLog);

                return new ResponseModel
                {
                    Code = 0,
                    Message = "Something went wrong. Please try again later."
                };
            }
        }
    }
}
