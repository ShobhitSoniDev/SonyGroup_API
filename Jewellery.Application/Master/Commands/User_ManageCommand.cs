using Jewellery.Application.Master.Interfaces;
using Jewellery.Application.Master.Models;
using Jewellery.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Jewellery.Application.Master.Commands
{
    public class User_ManageCommand : IRequest<ResponseModel>
    {
        public int LoginId { get; set; }
        public string UserName { get; set; } = "";
        public string Password { get; set; } = "";
        public int RoleId { get; set; }
        public string Email { get; set; } = "";
        public string MobileNo { get; set; } = "";
        public bool IsActive { get; set; } = true;
        public int TypeId { get; set; }
    }

    public class User_ManageCommandHandler
        : IRequestHandler<User_ManageCommand, ResponseModel>
    {
        private readonly IMasterRepository _masterRepository;

        public User_ManageCommandHandler(IMasterRepository masterRepository)
        {
            _masterRepository = masterRepository;
        }

        public async Task<ResponseModel> Handle(
            User_ManageCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                string passwordHash = "";

                // Insert User
                if (request.TypeId == 2)
                {
                    var hasher = new PasswordHasher<string>();

                    passwordHash =
                        hasher.HashPassword(
                            null,
                            request.Password);
                }

                var model = new UserModel
                {
                    LoginId = request.LoginId,
                    UserName = request.UserName,
                    PasswordHash = passwordHash,
                    RoleId = request.RoleId,
                    Email = request.Email,
                    MobileNo = request.MobileNo,
                    IsActive = request.IsActive,
                    TypeId = request.TypeId
                };

                var result =
                    await _masterRepository.User_ManageAsync(model);

                return new ResponseModel
                {
                    Code = 1,
                    Message = "SUCCESS",
                    Data = result
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