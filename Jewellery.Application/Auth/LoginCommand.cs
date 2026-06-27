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
    public class LoginCommand : IRequest<ResponseModel>
    {
        public string username { get; set; }
        public string password { get; set; }
        public string shopCode { get; set; } = "JS0000";
    }

    // ✅ Handler
    public class LoginCommandHandler
     : IRequestHandler<LoginCommand, ResponseModel>
    {
        private readonly ILoginRepository _loginRepository;
        private readonly JwtTokenService _jwtService;
        private readonly PasswordSecurityHelper _passSecurity;
        private readonly IErrorLogRepository _errorLogRepository;
        public LoginCommandHandler(ILoginRepository loginRepository, JwtTokenService jwtService, PasswordSecurityHelper passSecurity,IErrorLogRepository errorLogRepository)
        {
            _loginRepository = loginRepository;
            _jwtService = jwtService;
            _passSecurity = passSecurity;
            _errorLogRepository = errorLogRepository;
        }

        public async Task<ResponseModel> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // 🔥 INSERT + RETURN ROW (dynamic)
                var error = CommonInputValidator.Validate(value: request.username, numeric: false, minLength: 2, maxLength: 20);
                if (error.Code == 0)
                    return error;
                //error = CommonInputValidator.Validate(value: request.password, numeric: false, minLength: 2, maxLength: 20);
                //if (error.Code == 0)
                //    return error;
                string SC= request.shopCode;
                request.shopCode = "JWL_" + request.shopCode;
                var LoginResponse = await _loginRepository.LoginReturnAsync(request.username, request.shopCode);
                if (LoginResponse != null)
                {
                    var pass = LoginResponse.PasswordHash;
                    string  EncryptedPassword = _passSecurity.Encrypt(request.password);

                    if (pass == EncryptedPassword)
                    {
                        var UserId = LoginResponse.UserId.ToString();
                        var userName = LoginResponse.UserName.ToString();
                        var roleName = LoginResponse.RoleName.ToString();
                        var token = _jwtService.GenerateToken(UserId, userName, roleName, request.shopCode);
                        LoginResponse.token = token;
                        LoginResponse.shopCode = SC;
                        return new ResponseModel
                        {
                            Code = 1,
                            Message = LoginResponse.Message,
                            Data = LoginResponse
                        };
                    }
                    else
                    {
                        return new ResponseModel
                        {
                            Code = 0,
                            Message = "You have entered an invalid password."
                        };
                    }
                }
                else
                {
                    return new ResponseModel
                    {
                        Code = 1,
                        Message = "User Name is Incorrect."
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
                    ApiName = "LoginCommandHandler",
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
