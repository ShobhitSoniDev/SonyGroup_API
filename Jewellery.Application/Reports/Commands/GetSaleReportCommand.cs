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
    public class GetSalesReportCommand : IRequest<ResponseModel>
    {
        public DateTime? FromDate { get; set; }   // Bill date range start (inclusive)
        public DateTime? ToDate { get; set; }   // Bill date range end   (inclusive)
        public int? CustomerId { get; set; } = 0;   // NULL = all customers
        public string? CustomerType { get; set; }   // "FULKAR" | "HOLESALE" | NULL = all
        public int? ProductId { get; set; }   // NULL = all products
        public int? CategoryId { get; set; }   // NULL = all categories
        public int? MetalId { get; set; }   // NULL = all metals (Product_Master.MetalId)
        public string? MetalType { get; set; }   // "GOLD" | "SILVER" | NULL = all
        public string? BillNo { get; set; }   // Partial match | NULL = all
        public string? PaymentMode { get; set; }   // "CASH"|"CARD"|"UPI"|"CHEQUE"|"MIXED"|NULL=all
        public string? PaymentStatus { get; set; }   // "PAID" | "PARTIAL" | "UNPAID" | NULL = all
        public bool? IsActive { get; set; }   // NULL = both active & inactive
    }

    // ─── Handler ──────────────────────────────────────────────────────────────
    public class GetSalesReportCommandHandler
        : IRequestHandler<GetSalesReportCommand, ResponseModel>
    {
        private readonly IReportsRepository _reportsRepository;
        private readonly IErrorLogRepository _errorLogRepository;

        public GetSalesReportCommandHandler(
            IReportsRepository reportsRepository,
            IErrorLogRepository errorLogRepository)
        {
            _reportsRepository = reportsRepository;
            _errorLogRepository = errorLogRepository;
        }

        public async Task<ResponseModel> Handle(
            GetSalesReportCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                var model = new GetSalesReportModel
                {
                    FromDate = request.FromDate,
                    ToDate = request.ToDate,
                    CustomerId = request.CustomerId,
                    CustomerType = request.CustomerType,
                    ProductId = request.ProductId,
                    CategoryId = request.CategoryId,
                    MetalId = request.MetalId,
                    MetalType = request.MetalType,
                    BillNo = request.BillNo,
                    PaymentMode = request.PaymentMode,
                    PaymentStatus = request.PaymentStatus,
                    IsActive = request.IsActive
                };

                var result = await _reportsRepository.SalesReportAsync(model);

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
                    ApiName = "GetSalesReportCommand",
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
