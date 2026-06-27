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
    public class Role_Menu_Mapping_ManageCommand : IRequest<ResponseModel>
    {
        public int RoleId { get; set; } = 0;
        public string MenuIds { get; set; } = "";
        public int TypeId { get; set; } = 0;
    }

    // ✅ Handler
    public class Role_Menu_Mapping_ManageCommandHandler
        : IRequestHandler<Role_Menu_Mapping_ManageCommand, ResponseModel>
    {
        private readonly IMasterRepository _masterRepository;
        private readonly IErrorLogRepository _errorLogRepository;
        public Role_Menu_Mapping_ManageCommandHandler(
            IMasterRepository masterRepository, IErrorLogRepository errorLogRepository)
        {
            _masterRepository = masterRepository;
            _errorLogRepository = errorLogRepository;
        }

        public async Task<ResponseModel> Handle(
            Role_Menu_Mapping_ManageCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                // Save Mapping Validation
                if (request.TypeId == 2)
                {
                    if (request.RoleId <= 0)
                    {
                        return new ResponseModel
                        {
                            Code = 0,
                            Message = "Role is required."
                        };
                    }

                    if (string.IsNullOrWhiteSpace(request.MenuIds))
                    {
                        return new ResponseModel
                        {
                            Code = 0,
                            Message = "Please select at least one menu."
                        };
                    }
                }

                var model = new RoleMenuMappingModel
                {
                    RoleId = request.RoleId,
                    MenuIds = request.MenuIds,
                    TypeId = request.TypeId
                };

                var result =
                    await _masterRepository.RoleMenuMapping_ManageAsync(model);

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
                    ApiName = "Role_Menu_Mapping_ManageCommand",
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