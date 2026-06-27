using Jewellery.Application.Auth.Interfaces;
using Jewellery.Application.Master.Interfaces;
using Jewellery.Application.Master.Models;
using Jewellery.Domain.Entities;
using MediatR;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Jewellery.Application.Reports.Queries
{
    // ✅ Command
    public class Dashboard_GetDataQuery : IRequest<ResponseModel>
    {

    }

    // ✅ Handler
    public class Dashboard_GetDataQueryHandler
        : IRequestHandler<Dashboard_GetDataQuery, ResponseModel>
    {
        private readonly IDashboard_GetDataRepository _dashboard_GetDataRepository;
        private readonly IErrorLogRepository _errorLogRepository;
        public Dashboard_GetDataQueryHandler(IDashboard_GetDataRepository dashboard_GetDataRepository, IErrorLogRepository errorLogRepository)
        {
            _dashboard_GetDataRepository = dashboard_GetDataRepository;
            _errorLogRepository = errorLogRepository;
        }

        public async Task<ResponseModel> Handle(Dashboard_GetDataQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var GetDashboardData = await _dashboard_GetDataRepository.Dashboard_GetDataAsync();
                if (GetDashboardData != null)
                {
                    return new ResponseModel
                    {
                        Code = 1,
                        Message = "SUCCESS",
                        Data = GetDashboardData
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
                    ApiName = "Dashboard_GetDataQuery",
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
