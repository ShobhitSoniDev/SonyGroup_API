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
    public class GetPurchaseReportCommand : IRequest<ResponseModel>
    {
        public DateTime? FromDate { get; set; }   // Purchase date range start (inclusive)
        public DateTime? ToDate { get; set; }   // Purchase date range end   (inclusive)
        public int? SupplierId { get; set; }   // NULL = all suppliers
        public int? ProductId { get; set; }   // NULL = all products
        public int? CategoryId { get; set; }   // NULL = all categories
        public int? MetalId { get; set; }   // NULL = all metals (Product_Master.MetalId)
        public string? MetalType { get; set; }   // "GOLD" | "SILVER" | NULL = all
        public string? PurchaseNo { get; set; }   // Partial match | NULL = all
        public string? PaymentStatus { get; set; }   // "PAID" | "PARTIAL" | "UNPAID" | NULL = all
        public bool? IsActive { get; set; }   // NULL = both active & inactive
    }

    // ─── Handler ──────────────────────────────────────────────────────────────
    public class GetPurchaseReportCommandHandler
        : IRequestHandler<GetPurchaseReportCommand, ResponseModel>
    {
        private readonly IReportsRepository _reportsRepository;
        private readonly IErrorLogRepository _errorLogRepository;

        public GetPurchaseReportCommandHandler(
            IReportsRepository reportsRepository,
            IErrorLogRepository errorLogRepository)
        {
            _reportsRepository = reportsRepository;
            _errorLogRepository = errorLogRepository;
        }

        public async Task<ResponseModel> Handle(
            GetPurchaseReportCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                var model = new GetPurchaseReportModel
                {
                    FromDate = request.FromDate,
                    ToDate = request.ToDate,
                    SupplierId = request.SupplierId,
                    ProductId = request.ProductId,
                    CategoryId = request.CategoryId,
                    MetalId = request.MetalId,
                    MetalType = request.MetalType,
                    PurchaseNo = request.PurchaseNo,
                    PaymentStatus = request.PaymentStatus,
                    IsActive = request.IsActive
                };

                var result = await _reportsRepository.PurchaseReportAsync(model);

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
                    ApiName = "GetPurchaseReportCommand",
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
