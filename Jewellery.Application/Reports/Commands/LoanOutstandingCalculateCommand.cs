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
    public class LoanOutstandingCalculateCommand : IRequest<ResponseModel>
    {
        public int LoanId { get; set; }
        public DateTime CloserDate { get; set; }
    }

    // ✅ Handler
    public class LoanOutstandingCalculateCommandHandler
        : IRequestHandler<LoanOutstandingCalculateCommand, ResponseModel>
    {
        private readonly ILoanOutstandingCalculateRepository _loanOutstandingCalculateRepository;
        private readonly IErrorLogRepository _errorLogRepository;
        public LoanOutstandingCalculateCommandHandler(ILoanOutstandingCalculateRepository loanOutstandingCalculateRepository, IErrorLogRepository errorLogRepository)
        {
            _loanOutstandingCalculateRepository = loanOutstandingCalculateRepository;
            _errorLogRepository = errorLogRepository;
        }

        public async Task<ResponseModel> Handle(LoanOutstandingCalculateCommand request, CancellationToken cancellationToken)
        {
            try
            {

                
                var getloanentry = await _loanOutstandingCalculateRepository.LoanOutstandingCalculateAsync(request.LoanId, request.CloserDate);
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
                    ApiName = "LoanOutstandingCalculateCommand",
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
