using Jewellery.Application.Auth.Interfaces;
using Jewellery.Application.Master.Interfaces;
using Jewellery.Application.Master.Models;
using Jewellery.Domain.Entities;
using MediatR;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Jewellery.Application.Master.Commands
{
    // ✅ Command
    public class GetLoan_MastersQuery : IRequest<ResponseModel>
    {

    }

    // ✅ Handler
    public class GetLoan_MastersQueryHandler
        : IRequestHandler<GetLoan_MastersQuery, ResponseModel>
    {
        private readonly IGetLoan_MastersRepository _getLoan_MastersRepository;
        private readonly IErrorLogRepository _errorLogRepository;
        public GetLoan_MastersQueryHandler(IGetLoan_MastersRepository getLoan_MastersRepository, IErrorLogRepository errorLogRepository)
        {
            _getLoan_MastersRepository = getLoan_MastersRepository;
            _errorLogRepository = errorLogRepository;
        }

        public async Task<ResponseModel> Handle(GetLoan_MastersQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var GetLoan_Masters = await _getLoan_MastersRepository.GetLoan_MastersAsync();
                if (GetLoan_Masters != null)
                {
                    return new ResponseModel
                    {
                        Code = 1,
                        Message = "SUCCESS",
                        Data = GetLoan_Masters
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
            catch(Exception ex)
            {
                var stackTrace = new StackTrace(ex, true);
                var frame = stackTrace.GetFrame(0);

                int? lineNumber = frame?.GetFileLineNumber();
                string? stackTraceText = ex.StackTrace;
                var errorLog = new ErrorLog
                {
                    ApiName = "GetLoan_MastersQuery",
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
