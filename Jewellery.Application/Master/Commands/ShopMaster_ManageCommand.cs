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
    public class ShopMaster_ManageCommand : IRequest<ResponseModel>
    {
        public int ShopId { get; set; }
        public string ShopCode { get; set; } = "";
        public string ShopName { get; set; } = "";
        public string TagLine { get; set; } = "";
        public string OwnerName { get; set; } = "";
        public string MobileNo { get; set; } = "";
        public string Email { get; set; } = "";
        public string Address { get; set; } = "";
        public string GSTNo { get; set; } = "";
        public string Logo { get; set; } = "";
        public bool IsActive { get; set; } = true;
        public int TypeId { get; set; }
    }

    public class ShopMaster_ManageCommandHandler
        : IRequestHandler<ShopMaster_ManageCommand, ResponseModel>
    {
        private readonly IMasterRepository _masterRepository;
        private readonly IErrorLogRepository _errorLogRepository;

        public ShopMaster_ManageCommandHandler(
            IMasterRepository masterRepository,
            IErrorLogRepository errorLogRepository)
        {
            _masterRepository = masterRepository;
            _errorLogRepository = errorLogRepository;
        }

        public async Task<ResponseModel> Handle(
            ShopMaster_ManageCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                var model = new ShopModel
                {
                    ShopId = request.ShopId,
                    ShopCode = request.ShopCode,
                    ShopName = request.ShopName,
                    TagLine = request.TagLine,
                    OwnerName = request.OwnerName,
                    MobileNo = request.MobileNo,
                    Email = request.Email,
                    Address = request.Address,
                    GSTNo = request.GSTNo,
                    Logo = request.Logo,
                    IsActive = request.IsActive,
                    TypeId = request.TypeId
                };

                var result = await _masterRepository.ShopMaster_ManageAsync(model);

                return new ResponseModel
                {
                    Code = 1,
                    Message = "SUCCESS",
                    Data = result
                };
            }
            catch (Exception ex)
            {
                var stackTrace = new StackTrace(ex, true);
                var frame = stackTrace.GetFrame(0);
                int? lineNumber = frame?.GetFileLineNumber();

                var errorLog = new ErrorLog
                {
                    ApiName = "ShopMaster_ManageCommand",
                    ErrorMessage = ex.Message,
                    StackTrace = ex.StackTrace,
                    LineNumber = lineNumber ?? 0,
                    CreatedDate = DateTime.Now
                };
                await _errorLogRepository.SaveErrorAsync(errorLog);

                return new ResponseModel
                {
                    Code = 0,
                    Message = "Something went wrong. Please try again later."
                };
            }
        }
    }
}