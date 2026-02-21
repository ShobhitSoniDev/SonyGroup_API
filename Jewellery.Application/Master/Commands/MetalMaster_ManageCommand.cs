using MediatR;
using Jewellery.Application.Master.Interfaces;
using Jewellery.Domain.Entities;

namespace Jewellery.Application.Master.Commands
{
    // ✅ Command
    public class MetalMaster_ManageCommand : IRequest<ResponseModel>
    {
        public string MetalName { get; set; } = "";
        public int Purity { get; set; } = 0;
        public string CreatedBy { get; set; } = "";
        public int TypeId { get; set; } = 0;
        public int MetalId { get; set; } = 0;
    }

    // ✅ Handler
    public class MetalMaster_ManageCommandHandler
     : IRequestHandler<MetalMaster_ManageCommand, ResponseModel>
    {
        private readonly IMetalRepository _metalRepository;

        public MetalMaster_ManageCommandHandler(IMetalRepository metalRepository)
        {
            _metalRepository = metalRepository;
        }

        public async Task<ResponseModel> Handle(MetalMaster_ManageCommand request,CancellationToken cancellationToken)
        {
            // 🔥 INSERT + RETURN ROW (dynamic)
            if (request.TypeId == 1 || request.TypeId == 2)
            {
                var error = CommonInputValidator.Validate(value: request.MetalName, numeric: false, minLength: 2, maxLength: 20);
                if (error.Code == 0)
                    return error;
            
            error = CommonInputValidator.Validate(value: request.Purity.ToString(), numeric: true, minLength: 1, maxLength: 20);
            if (error.Code == 0)
                return error;
            }

            var insertedMetal = await _metalRepository.MetalMaster_ManageAndReturnAsync(request.MetalName,request.Purity,request.CreatedBy,request.TypeId,request.MetalId);
            if (insertedMetal != null)
            {
                return new ResponseModel
                {
                    Code = 1,
                    Message = "SUCCESS",
                    Data = insertedMetal   
                };
            }
            else
            {
                return new ResponseModel
                {
                    Code = 1,
                    Message = "FAILED"
                };
            }
        }
    }

}
