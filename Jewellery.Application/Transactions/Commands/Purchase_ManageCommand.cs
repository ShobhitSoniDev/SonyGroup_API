using Jewellery.Application.Common.Interfaces;
using Jewellery.Application.Master.Interfaces;
using Jewellery.Application.Transactions.Interfaces;
using Jewellery.Domain.Entities;
using MediatR;
using System.Diagnostics;

namespace Jewellery.Application.Transactions.Commands
{
    public class Purchase_ManageCommand : IRequest<ResponseModel>
    {
        public int PurchaseId { get; set; }
        public string PurchaseNo { get; set; } = "";
        public DateTime? PurchaseDate { get; set; }
        public int SupplierId { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public string Remarks { get; set; } = "";
        public int TypeId { get; set; }

        // Purchase Details
        public List<PurchaseDetailModel> Details { get; set; } = new();
    }

    public class Purchase_ManageCommandHandler
        : IRequestHandler<Purchase_ManageCommand, ResponseModel>
    {
        private readonly ITransactionsRepository _transactionsRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IErrorLogRepository _errorLogRepository;

        public Purchase_ManageCommandHandler(
            ITransactionsRepository transactionsRepository,
            ICurrentUserService currentUserService,
            IErrorLogRepository errorLogRepository)
        {
            _transactionsRepository = transactionsRepository;
            _currentUserService = currentUserService;
            _errorLogRepository = errorLogRepository;
        }

        public async Task<ResponseModel> Handle(
            Purchase_ManageCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                var error = CommonInputValidator.Validate(
                    value: request.TypeId.ToString(),
                    numeric: true,
                    minLength: 1,
                    maxLength: 2);

                if (error.Code == 0)
                    return error;

                if (request.TypeId == 1 || request.TypeId == 2)
                {
                    error = CommonInputValidator.Validate(
                        value: request.PurchaseNo,
                        numeric: false,
                        minLength: 1,
                        maxLength: 20);

                    if (error.Code == 0)
                        return error;

                    error = CommonInputValidator.Validate(
                        value: request.SupplierId.ToString(),
                        numeric: true,
                        minLength: 1,
                        maxLength: 10);

                    if (error.Code == 0)
                        return error;

                    error = CommonInputValidator.Validate(
                        value: request.TotalAmount.ToString(),
                        numeric: true,
                        allowDecimal: true,
                        minLength: 1,
                        maxLength: 20);

                    if (error.Code == 0)
                        return error;

                    error = CommonInputValidator.Validate(
                        value: request.PaidAmount.ToString(),
                        numeric: true,
                        allowDecimal: true,
                        minLength: 1,
                        maxLength: 20);

                    if (error.Code == 0)
                        return error;

                    if (request.Details == null || request.Details.Count == 0)
                    {
                        return new ResponseModel
                        {
                            Code = 0,
                            Message = "Purchase details are required."
                        };
                    }
                }

                var model = new PurchaseModel
                {
                    PurchaseId = request.PurchaseId,
                    PurchaseNo = request.PurchaseNo,
                    PurchaseDate = request.PurchaseDate,
                    SupplierId = request.SupplierId,
                    TotalAmount = request.TotalAmount,
                    PaidAmount = request.PaidAmount,
                    Remarks = request.Remarks,
                    CreatedBy = _currentUserService.UserName,
                    TypeId = request.TypeId,
                    Details = request.Details
                };

                var result = await _transactionsRepository.Purchase_ManageAsync(model);

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

                var errorLog = new ErrorLog
                {
                    ApiName = "Purchase_ManageCommand",
                    ErrorMessage = ex.Message,
                    StackTrace = ex.StackTrace,
                    LineNumber = frame?.GetFileLineNumber() ?? 0,
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