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
    public class GetLoanEntryReportCommand : IRequest<ResponseModel>
    {
        public int? LoanId { get; set; }
        public int? CustomerId { get; set; }
        public string? LoanType { get; set; }
        public string? LoanStatus { get; set; }
        public string? MetalType { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public long? AmountFrom { get; set; }
        public long? AmountTo { get; set; }
        public int PageNo { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    // ✅ Handler
    public class GetLoanEntryReportCommandHandler
        : IRequestHandler<GetLoanEntryReportCommand, ResponseModel>
    {
        private readonly ILoanEntryReportRepository _loanEntryReportsRepository;
        private readonly IErrorLogRepository _errorLogRepository;
        public GetLoanEntryReportCommandHandler(ILoanEntryReportRepository loanEntryReportsRepository, IErrorLogRepository errorLogRepository)
        {
            _loanEntryReportsRepository = loanEntryReportsRepository;
            _errorLogRepository = errorLogRepository;
        }

        public async Task<ResponseModel> Handle(GetLoanEntryReportCommand request, CancellationToken cancellationToken)
        {
            try
            {

                var loanEntryReportModel = new GetLoanEntryReportModel
                {
                    LoanId = request.LoanId,
                    CustomerId = request.CustomerId,
                    LoanType = request.LoanType,
                    LoanStatus = request.LoanStatus,
                    MetalType = request.MetalType,
                    FromDate = request.FromDate,
                    ToDate = request.ToDate,
                    AmountFrom = request.AmountFrom,
                    AmountTo = request.AmountTo,
                    PageNo = request.PageNo,
                    PageSize = request.PageSize
                };
                var getloanentry = await _loanEntryReportsRepository.LoanEntryReportsAsync(loanEntryReportModel);
                if (getloanentry != null)
                {
                    return new ResponseModel
                    {
                        Code = 1,
                        Message = "SUCCESS",
                        Data = getloanentry
                    };
                }
                else
                {
                    return new ResponseModel
                    {
                        Code = 1,
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
                    ApiName = "GetLoanEntryReportCommand",
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
