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
    public class SupplierMaster_ManageCommand : IRequest<ResponseModel>
    {
        public int SupplierId { get; set; }
        public string SupplierName { get; set; } = "";
        public string Phone { get; set; } = "";
        public string GSTIN { get; set; } = "";
        public string Address { get; set; } = "";
        public bool IsActive { get; set; } = true;
        public int TypeId { get; set; }
    }

    public class SupplierMaster_ManageCommandHandler
        : IRequestHandler<SupplierMaster_ManageCommand, ResponseModel>
    {
        private readonly IMasterRepository _masterRepository;
        private readonly IErrorLogRepository _errorLogRepository;

        public SupplierMaster_ManageCommandHandler(
            IMasterRepository masterRepository,
            IErrorLogRepository errorLogRepository)
        {
            _masterRepository = masterRepository;
            _errorLogRepository = errorLogRepository;
        }

        public async Task<ResponseModel> Handle(
            SupplierMaster_ManageCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                var model = new SupplierModel
                {
                    SupplierId = request.SupplierId,
                    SupplierName = request.SupplierName,
                    Phone = request.Phone,
                    GSTIN = request.GSTIN,
                    Address = request.Address,
                    IsActive = request.IsActive,
                    TypeId = request.TypeId
                };

                var result = await _masterRepository.Supplier_ManageAsync(model);

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
                    ApiName = "SupplierMaster_ManageCommand",
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