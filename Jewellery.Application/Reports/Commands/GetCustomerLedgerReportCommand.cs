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
    public class GetCustomerLedgerReportCommand : IRequest<ResponseModel>
    {
        public string? CustomerCode { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int? TransactionType { get; set; }   // NULL=All | 1=DR | 2=CR
        public int TypeId { get; set; }   // 1=Detail | 2=Summary
    }

    // ─── Handler ──────────────────────────────────────────────────────────────
    public class GetCustomerLedgerReportCommandHandler
        : IRequestHandler<GetCustomerLedgerReportCommand, ResponseModel>
    {
        private readonly IReportsRepository _reportsRepository;
        private readonly IErrorLogRepository _errorLogRepository;
        public GetCustomerLedgerReportCommandHandler(
            IReportsRepository reportsRepositoryRepository, IErrorLogRepository errorLogRepository)
        {
            _reportsRepository = reportsRepositoryRepository;
            _errorLogRepository = errorLogRepository;
        }

        public async Task<ResponseModel> Handle(
            GetCustomerLedgerReportCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                var model = new GetCustomerLedgerReportModel
                {
                    CustomerCode = request.CustomerCode,
                    FromDate = request.FromDate,
                    ToDate = request.ToDate,
                    TransactionType = request.TransactionType,
                    TypeId = request.TypeId
                };

                var result = await _reportsRepository
                                   .CustomerLedgerReportAsync(model);

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
                    ApiName = "GetCustomerLedgerReportCommand",
                    ErrorMessage = ex.Message,
                    StackTrace = stackTraceText,
                    LineNumber = lineNumber ?? 0,
                    CreatedDate = DateTime.Now
                };
                // ✅ Save Log in DB (via Infrastructure)
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
