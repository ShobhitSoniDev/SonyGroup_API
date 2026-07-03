using Jewellery.Application.Common.Security;
using Jewellery.Application.Master.Interfaces;
using Jewellery.Application.Master.Models;
using Jewellery.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Diagnostics;
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
        private readonly IErrorLogRepository _errorLogRepository;
        private readonly PasswordSecurityHelper _passSecurity;
        public ChangePasswordCommandHandler(
            IMasterRepository masterRepository,IErrorLogRepository errorLogRepository, PasswordSecurityHelper passSecurity)
        {
            _masterRepository = masterRepository;
            _errorLogRepository = errorLogRepository;
            _passSecurity = passSecurity;
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
                if(request.CurrentPassword== request.NewPassword)
                {
                    return new ResponseModel
                    {
                        Code = 0,
                        Message = "The new password must be different from the old password.",
                        Data = null
                    };
                }

                CurrentPasswordHash = _passSecurity.Encrypt(request.CurrentPassword);
                NewPasswordHash = _passSecurity.Encrypt(request.NewPassword);
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
                    if (passwordHashdb == CurrentPasswordHash)
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
                            Message = "Password updated successfully.",
                            Data = update
                        };
                    }
                    else
                    {
                        return new ResponseModel
                        {
                            Code = 0,
                            Message = "The old password does not match.",
                            Data = result
                        };
                    }
                }
                else
                {
                    return new ResponseModel
                    {
                        Code = 0,
                        Message = "SUCCESS",
                        Data = result
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