using Jewellery.Application.Auth.Interfaces;
using Jewellery.Application.Master.Interfaces;
using Jewellery.Application.Master.Models;
using Jewellery.Application.Transactions.Interfaces;
using Jewellery.Domain.Entities;
using MediatR;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Jewellery.Application.Master.Commands
{
    // ✅ Command
    public class GetMenuQuery : IRequest<ResponseModel>
    {

    }

    // ✅ Handler
    public class GetMenuQueryHandler
        : IRequestHandler<GetMenuQuery, ResponseModel>
    {
        private readonly IAuthRepository _authRepository;
        private readonly IErrorLogRepository _errorLogRepository;
        public GetMenuQueryHandler(IAuthRepository authRepository, IErrorLogRepository errorLogRepository)
        {
            _authRepository = authRepository;
            _errorLogRepository= errorLogRepository;
        }

        public async Task<ResponseModel> Handle(GetMenuQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var GetMenu = await _authRepository.GetMenuReturnAsync();
                if (GetMenu != null)
                {
                    return new ResponseModel
                    {
                        Code = 1,
                        Message = "SUCCESS",
                        Data = GetMenu
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
                    ApiName = "GetMenuQuery",
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
