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
    // ✅ Command
    public class ChangePasswordCommand : IRequest<ResponseModel>
    {
        public string CurrentPassword { get; set; } = "";
        public string NewPassword { get; set; } = "";
    }

    // ✅ Handler
    public class ChangePasswordCommandHandler
        : IRequestHandler<ChangePasswordCommand, ResponseModel>
    {
        private readonly IMasterRepository _masterRepository;

        public ChangePasswordCommandHandler(
            IMasterRepository masterRepository)
        {
            _masterRepository = masterRepository;
        }

        public async Task<ResponseModel> Handle(
            ChangePasswordCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.CurrentPassword))
                {
                    return new ResponseModel
                    {
                        Code = 0,
                        Message = "Current Password is required."
                    };
                }

                if (string.IsNullOrWhiteSpace(request.NewPassword))
                {
                    return new ResponseModel
                    {
                        Code = 0,
                        Message = "New Password is required."
                    };
                }
                string CurrentPasswordHash = "";
                string NewPasswordHash = "";
                var hasher = new PasswordHasher<string>();

                CurrentPasswordHash = hasher.HashPassword(null, request.CurrentPassword);
                NewPasswordHash = hasher.HashPassword(null, request.NewPassword);
                var model = new ChangePasswordModel
                {
                    CurrentPasswordHash = CurrentPasswordHash,
                    NewPasswordHash = NewPasswordHash,
                    TypeId = 2
                };

                dynamic result = await _masterRepository.ChangePasswordAsync(model);
                if (result != null)
                {
                    IDictionary<string, object> row = result;
                    string passwordHashdb = row.ContainsKey("PasswordHash")
                        ? row["PasswordHash"]?.ToString() ?? "" : "";
                    var verify = hasher.VerifyHashedPassword(null, passwordHashdb, request.CurrentPassword);
                    if (verify == PasswordVerificationResult.Success)
                    {
                        var model_ = new ChangePasswordModel
                        {
                            CurrentPasswordHash = CurrentPasswordHash,
                            NewPasswordHash = NewPasswordHash,
                            TypeId = 1
                        };
                        var update = await _masterRepository.ChangePasswordAsync(model_);
                        return new ResponseModel
                        {
                            Code = 1,
                            Message = "SUCCESS",
                            Data = update
                        };
                    }
                    else
                    {
                        return new ResponseModel
                        {
                            Code = 1,
                            Message = "SUCCESS",
                            Data = result
                        };
                    }
                }
                else
                {
                    return new ResponseModel
                    {
                        Code = 1,
                        Message = "SUCCESS",
                        Data = result
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