using Jewellery.Application.Common.Interfaces;
using Jewellery.Application.Master.Interfaces;
using Jewellery.Application.Transactions.Interfaces;
using Jewellery.Domain.Entities;
using MediatR;
using System.Diagnostics;

namespace Jewellery.Application.Transactions.Commands
{
    public class Sales_ManageCommand : IRequest<ResponseModel>
    {
        public int SaleId { get; set; }
        public string BillNo { get; set; } = "";
        public DateTime? BillDate { get; set; }
        public int CustomerId { get; set; }
        public string CustomerType { get; set; } = "";
        public decimal TotalAmount { get; set; }
        public decimal GSTAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public string PaymentMode { get; set; } = "";
        public string Remarks { get; set; } = "";
        public int TypeId { get; set; }
        public bool IsActive { get; set; } = true;

        // Details
        public List<SalesDetailModel> DetailsJson { get; set; } = new();
        public List<OldJewelleryModel> OldJewelleryJson { get; set; } = new();
    }

    public class Sales_ManageCommandHandler
        : IRequestHandler<Sales_ManageCommand, ResponseModel>
    {
        private readonly ITransactionsRepository _transactionsRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IErrorLogRepository _errorLogRepository;

        public Sales_ManageCommandHandler(
            ITransactionsRepository transactionsRepository,
            ICurrentUserService currentUserService,
            IErrorLogRepository errorLogRepository)
        {
            _transactionsRepository = transactionsRepository;
            _currentUserService = currentUserService;
            _errorLogRepository = errorLogRepository;
        }

        public async Task<ResponseModel> Handle(
            Sales_ManageCommand request,
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
                        value: request.BillNo,
                        numeric: false,
                        minLength: 1,
                        maxLength: 20);

                    if (error.Code == 0)
                        return error;

                    error = CommonInputValidator.Validate(
                        value: request.CustomerId.ToString(),
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

                    if (request.DetailsJson == null || request.DetailsJson.Count == 0)
                    {
                        return new ResponseModel
                        {
                            Code = 0,
                            Message = "Sale details are required."
                        };
                    }
                }

                var model = new SalesModel
                {
                    SaleId = request.SaleId,
                    BillNo = request.BillNo,
                    BillDate = request.BillDate,
                    CustomerId = request.CustomerId,
                    CustomerType=request.CustomerType,
                    TotalAmount = request.TotalAmount,
                    GSTAmount = request.GSTAmount,
                    PaidAmount = request.PaidAmount,
                    PaymentMode = request.PaymentMode,
                    Remarks = request.Remarks,
                    TypeId = request.TypeId,
                    IsActive=request.IsActive,
                    Details = request.DetailsJson,
                    OldJewelleryDetails=request.OldJewelleryJson,
                };

                var result = await _transactionsRepository.Sales_ManageAsync(model);

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
                    ApiName = "Sales_ManageCommand",
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