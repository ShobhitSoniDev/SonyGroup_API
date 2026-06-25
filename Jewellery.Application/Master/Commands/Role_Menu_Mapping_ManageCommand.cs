using MediatR;
using Jewellery.Application.Master.Interfaces;
using Jewellery.Application.Master.Models;
using Jewellery.Domain.Entities;
using System;
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

        public Role_Menu_Mapping_ManageCommandHandler(
            IMasterRepository masterRepository)
        {
            _masterRepository = masterRepository;
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
                return new ResponseModel
                {
                    Code = 0,
                    Message = ex.Message
                };
            }
        }
    }
}