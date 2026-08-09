using Jewellery.Application.Auth.Interfaces;
using Jewellery.Application.Common.Security;
using Jewellery.Application.Master.Interfaces;
using Jewellery.Application.Transactions.Interfaces;
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
    public class SignUpCustomerCommand : IRequest<ResponseModel>
    {
        public string UserName { get; set; } = "";
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
        public string OldPassword { get; set; } = "";
        public string MobileNo { get; set; } = "";
        public int Type { get; set; }
        public string shopCode { get; set; } = "JS0001";
    }

    // ✅ Handler
    public class SignUpCustomerCommandHandler
     : IRequestHandler<SignUpCustomerCommand, ResponseModel>
    {
        private readonly IAuthRepository _authRepository;
        private readonly JwtTokenService _jwtService;
        private readonly PasswordSecurityHelper _passSecurity;
        private readonly IErrorLogRepository _errorLogRepository;
        public SignUpCustomerCommandHandler(IAuthRepository authRepository, JwtTokenService jwtService, PasswordSecurityHelper passSecurity, IErrorLogRepository errorLogRepository)
        {
            _authRepository = authRepository;
            _jwtService = jwtService;
            _passSecurity = passSecurity;
            _errorLogRepository = errorLogRepository;
        }

        public async Task<ResponseModel> Handle(SignUpCustomerCommand request, CancellationToken cancellationToken)
        {
            try
            {
            string hashedPassword = "";
            var hasher = new PasswordHasher<string>();
            var error = CommonInputValidator.Validate(value: request.UserName, numeric: false, minLength: 2, maxLength: 20);
            if (error.Code == 0)
                return error;
            if (request.Type == 1)
            {
                //error = CommonInputValidator.Validate(value: request.Email, numeric: false, minLength: 2, maxLength: 20);
                //if (error.Code == 0)
                //    return error;
                //error = CommonInputValidator.Validate(value: request.Password, numeric: false, minLength: 2, maxLength: 20);
                //if (error.Code == 0)
                //    return error;
                error = CommonInputValidator.Validate(value: request.MobileNo, numeric: false, minLength: 2, maxLength: 20);
                if (error.Code == 0)
                    return error;
            }
            else if (request.Type == 2)
            {
                error = CommonInputValidator.Validate(value: request.Password, numeric: false, minLength: 2, maxLength: 20);
                if (error.Code == 0)
                    return error;
                var LoginResponse = await _authRepository.LoginCustomerReturnAsync(request.UserName, request.shopCode);
                if(LoginResponse.RoleName.ToLower() != "customer")
                {
                    return new ResponseModel
                    {
                        Code = 0,
                        Message = "Invalid Role."
                    };
                }
                var pass = LoginResponse.PasswordHash;
                var result = _passSecurity.Encrypt(request.OldPassword);
                if (result != pass)
                {
                    return new ResponseModel
                    {
                        Code = 0,
                        Message = "The old password is invalid."
                    };
                }
            }
            if (request.Type == 1 || request.Type == 2)
            {
                hashedPassword = _passSecurity.Encrypt(request.Password);
            }
            var SignUpResponse = await _authRepository.SignUpReturnAsync(request.UserName, request.Email, hashedPassword, request.MobileNo, request.Type, request.shopCode,"Customer");

            if (SignUpResponse != null)
            {
                var Code = SignUpResponse.Code;
                var Message = SignUpResponse.Message;
                return new ResponseModel
                {
                    Code = Code,
                    Message = Message
                };
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
                    ApiName = "LoginCustomerCommandHandler",
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
