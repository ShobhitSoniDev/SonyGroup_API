using Jewellery.Application.Auth.Interfaces;
using Jewellery.Application.Common.Security;
using Jewellery.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
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
        public ConvertPasswordCommandHandler(ILoginRepository loginRepository, JwtTokenService jwtService, PasswordSecurityHelper passSecurity)
        {
            _loginRepository = loginRepository;
            _jwtService = jwtService;
            _passSecurity = passSecurity;
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
                return new ResponseModel
                {
                    Code = 0,
                    Message = ex.Message
                };
            }
        }
    }
}
