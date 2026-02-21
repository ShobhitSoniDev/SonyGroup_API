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
    public class SignUpCommand : IRequest<ResponseModel>
    {
        public string UserName { get; set; } = "";
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
        public string OldPassword { get; set; } = "";
        public string MobileNo { get; set; } = "";
        public int Type { get; set; }   
    }

    // ✅ Handler
    public class SignUpCommandHandler
     : IRequestHandler<SignUpCommand, ResponseModel>
    {
        private readonly ILoginRepository _loginRepository;
        private readonly ISignUpRepository _SignUpRepository;
        private readonly JwtTokenService _jwtService;

        public SignUpCommandHandler(ILoginRepository loginRepository, ISignUpRepository SignUpRepository, JwtTokenService jwtService)
        {
            _loginRepository = loginRepository;
            _SignUpRepository = SignUpRepository;
            _jwtService = jwtService;
        }

        public async Task<ResponseModel> Handle(SignUpCommand request, CancellationToken cancellationToken)
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
                error = CommonInputValidator.Validate(value: request.Password, numeric: false, minLength: 2, maxLength: 20);
                if (error.Code == 0)
                    return error;
                error = CommonInputValidator.Validate(value: request.MobileNo, numeric: false, minLength: 2, maxLength: 20);
                if (error.Code == 0)
                    return error;
            }
            else if (request.Type == 2)
            {
                error = CommonInputValidator.Validate(value: request.Password, numeric: false, minLength: 2, maxLength: 20);
                if (error.Code == 0)
                    return error;
                var LoginResponse = await _loginRepository.LoginReturnAsync(request.UserName);
                var pass = LoginResponse.PasswordHash;
                var result = hasher.VerifyHashedPassword(null, pass, request.OldPassword);
                if (result != PasswordVerificationResult.Success)
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
                hashedPassword = hasher.HashPassword(null, request.Password);
            }
            var SignUpResponse = await _SignUpRepository.SignUpReturnAsync(request.UserName, request.Email, hashedPassword, request.MobileNo, request.Type);

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
    }
}
