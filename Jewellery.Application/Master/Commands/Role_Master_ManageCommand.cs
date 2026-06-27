using Jewellery.Application.Master.Interfaces;
using Jewellery.Application.Master.Models;
using Jewellery.Domain.Entities;
using MediatR;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Jewellery.Application.Master.Commands
{
    // ✅ Command
    public class RoleMaster_ManageCommand : IRequest<ResponseModel>
    {
        public int RoleId { get; set; } = 0;
        public string RoleName { get; set; } = "";
        public string RoleCode { get; set; } = "";
        public string RoleDescription { get; set; } = "";
        public bool IsActive { get; set; } = true;
        public int TypeId { get; set; } = 0;
    }

    // ✅ Handler
    public class RoleMaster_ManageCommandHandler
        : IRequestHandler<RoleMaster_ManageCommand, ResponseModel>
    {
        private readonly IMasterRepository _masterRepository;
        private readonly IErrorLogRepository _errorLogRepository;
        public RoleMaster_ManageCommandHandler(IMasterRepository roleRepository, IErrorLogRepository errorLogRepository)
        {
            _masterRepository = roleRepository;
            _errorLogRepository = errorLogRepository;
        }

        public async Task<ResponseModel> Handle(
            RoleMaster_ManageCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                // Insert / Update Validation
                if (request.TypeId == 2 || request.TypeId == 3)
                {
                    var error = CommonInputValidator.Validate(
                        value: request.RoleName,
                        numeric: false,
                        minLength: 2,
                        maxLength: 50);

                    if (error.Code == 0)
                        return error;
                }

                var roleModel = new RoleMasterModel
                {
                    RoleId = request.RoleId,
                    RoleName = request.RoleName,
                    RoleDescription = request.RoleDescription,
                    IsActive = request.IsActive,
                    TypeId = request.TypeId,
                    RoleCode=request.RoleCode
                };

                var result = await _masterRepository.RoleMaster_ManageAsync(roleModel);

                if (result != null)
                {
                    return new ResponseModel
                    {
                        Code = 1,
                        Message = "SUCCESS",
                        Data = result
                    };
                }

                return new ResponseModel
                {
                    Code = 0,
                    Message = "FAILED"
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
                    ApiName = "RoleMaster_ManageCommand",
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