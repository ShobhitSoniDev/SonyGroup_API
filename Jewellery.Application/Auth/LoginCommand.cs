using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jewellery.Application.Auth.Interfaces;
using Microsoft.AspNetCore.Identity;
using Jewellery.Domain.Entities;
using MediatR;

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

        public LoginCommandHandler(ILoginRepository loginRepository, JwtTokenService jwtService)
        {
            _loginRepository = loginRepository;
            _jwtService = jwtService;
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
                    var hasher = new PasswordHasher<string>();
                    var pass = LoginResponse.PasswordHash;
                    var result = hasher.VerifyHashedPassword(null, pass, request.password);
                    if (result == PasswordVerificationResult.Success)
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
                            Message = LoginResponse.Message
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
                return new ResponseModel
                {
                    Code = 0,
                    Message = ex.Message
                };
            }
        }
    }
}
