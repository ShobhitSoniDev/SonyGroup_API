using Jewellery.Application.Auth.Interfaces;
using Jewellery.Application.Common.Security;
using Jewellery.Application.Master.Interfaces;
using Jewellery.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jewellery.Application.Auth
{
    // ✅ Command
    public class ConvertPasswordCommand : IRequest<ResponseModel>
    {
        public string Password { get; set; } = "";
        public bool IsEncrypted { get; set; }
    }

    // ✅ Handler
    public class ConvertPasswordCommandHandler
     : IRequestHandler<ConvertPasswordCommand, ResponseModel>
    {
        private readonly ILoginRepository _loginRepository;
        private readonly JwtTokenService _jwtService;
        private readonly PasswordSecurityHelper _passSecurity;
        private readonly IErrorLogRepository _errorLogRepository;
        public ConvertPasswordCommandHandler(ILoginRepository loginRepository, JwtTokenService jwtService, PasswordSecurityHelper passSecurity, IErrorLogRepository errorLogRepository)
        {
            _loginRepository = loginRepository;
            _jwtService = jwtService;
            _passSecurity = passSecurity;
            _errorLogRepository = errorLogRepository;
        }

        public async Task<ResponseModel> Handle(ConvertPasswordCommand request, CancellationToken cancellationToken)
        {
            try
            {
                string result = request.IsEncrypted
                     ? _passSecurity.Decrypt(request.Password)
                     : _passSecurity.Encrypt(request.Password);

                return new ResponseModel
                {
                    Code = 1,
                    Data = result,
                    Message = "SUCCESS"
                };
            }
            catch (Exception ex)
            {
                var stackTrace = new StackTrace(ex, true);
                var frame = stackTrace.GetFrame(0);

                int? lineNumber = frame?.GetFileLineNumber();
                string? stackTraceText = ex.StackTrace;
                var errorLog = new ErrorLog
                {
                    ApiName = "ConvertPasswordCommand",
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
