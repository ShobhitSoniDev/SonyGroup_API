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

        public RoleMaster_ManageCommandHandler(IMasterRepository roleRepository)
        {
            _masterRepository = roleRepository;
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
                return new ResponseModel
                {
                    Code = 0,
                    Message = ex.Message
                };
            }
        }
    }
}