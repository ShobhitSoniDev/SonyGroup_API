using Jewellery.Application.Master.Interfaces;
using Jewellery.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics;

namespace Jewellery.API.Filters
{
    public class ExceptionFilter : IExceptionFilter
    {
        private readonly IErrorLogRepository _errorLogRepository;

        public ExceptionFilter(IErrorLogRepository errorLogRepository)
        {
            _errorLogRepository = errorLogRepository;
        }

        public void OnException(ExceptionContext context)
        {
            var exception = context.Exception;

            // ✅ Controller & API Method Name
            var controllerName =
                context.RouteData.Values["controller"]?.ToString();

            var actionName =
                context.RouteData.Values["action"]?.ToString();

            var apiName = $"{controllerName}/{actionName}";

            // ✅ Line Number Extract
            int? lineNumber = null;
            var stackTrace = new StackTrace(exception, true);

            if (stackTrace.FrameCount > 0)
            {
                lineNumber = stackTrace.GetFrame(0)?.GetFileLineNumber();
            }

            // ✅ Error Log Object
            var errorLog = new ErrorLog
            {
                ApiName = apiName,
                ErrorMessage = exception.Message,
                StackTrace = exception.StackTrace,
                LineNumber = lineNumber,
                CreatedDate = DateTime.Now
            };

            // ✅ Save Log in DB (via Infrastructure)
            _errorLogRepository.SaveErrorAsync(errorLog);

            // ✅ API Response
            context.Result = new ObjectResult(new
            {
                StatusCode = 500,
                Message = "Something went wrong. Please try again later."
            })
            {
                StatusCode = 500
            };

            context.ExceptionHandled = true;
        }
    }
}
