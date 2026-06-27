using Jewellery.Application.Common.Interfaces;
using Jewellery.Application.Master.Interfaces;
using Jewellery.Application.Transactions.Interfaces;
using Jewellery.Domain.Entities;
using MediatR;
using System.Diagnostics;

namespace Jewellery.Application.Transactions.Commands
{
    public class CustomerLedger_ManageCommand : IRequest<ResponseModel>
    {
        public long TransId { get; set; } = 0;
        public string CustomerCode { get; set; } = "";
        public string TransactionDate { get; set; } = "";
        public int TransactionType { get; set; } = 0;
        public decimal Amount { get; set; } = 0;
        public string Description { get; set; } = "";
        public int TypeId { get; set; } = 0;
    }

    public class CustomerLedger_ManageCommandHandler
        : IRequestHandler<CustomerLedger_ManageCommand, ResponseModel>
    {
        private readonly ITransactionsRepository _transactionsRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IErrorLogRepository _errorLogRepository;
        public CustomerLedger_ManageCommandHandler(
            ITransactionsRepository customerRepository,
            ICurrentUserService currentUserService,
            IErrorLogRepository errorLogRepository)
        {
            _transactionsRepository = customerRepository;
            _currentUserService = currentUserService;
            _errorLogRepository = errorLogRepository;
        }

        public async Task<ResponseModel> Handle(
            CustomerLedger_ManageCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                var error = CommonInputValidator.Validate(
                    value: request.TypeId.ToString(),
                    numeric: true,
                    minLength: 1,
                    maxLength: 20);

                if (error.Code == 0)
                    return error;

                if (request.TypeId == 1 || request.TypeId == 2)
                {
                    error = CommonInputValidator.Validate(
                        value: request.CustomerCode.ToString(),
                        numeric: false,
                        minLength: 1,
                        maxLength: 20);

                    if (error.Code == 0)
                        return error;

                    error = CommonInputValidator.Validate(
                        value: request.TransactionType.ToString(),
                        numeric: true,
                        minLength: 1,
                        maxLength: 20);

                    if (error.Code == 0)
                        return error;

                    error = CommonInputValidator.Validate(
                        value: request.Amount.ToString(),
                        numeric: true,
                        allowDecimal: true,
                        minLength: 1,
                        maxLength: 20);

                    if (error.Code == 0)
                        return error;

                    //error = CommonInputValidator.Validate(
                    //    value: request.TransactionDate,
                    //    numeric: false,
                    //    minLength: 1,
                    //    maxLength: 20);

                    //if (error.Code == 0)
                    //    return error;
                }

                var model = new CustomerLedgerModel
                {
                    TransId = request.TransId,
                    CustomerCode = request.CustomerCode,
                    TransactionDate = request.TransactionDate,
                    TransactionType = request.TransactionType,
                    Amount = request.Amount,
                    Description = request.Description,
                    TypeId = request.TypeId,
                    UserId = _currentUserService.UserId
                };

                var result = await _transactionsRepository.CustomerLedger_ManageAsync(model);

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
            catch(Exception ex)
            {
                var stackTrace = new StackTrace(ex, true);
                var frame = stackTrace.GetFrame(0);

                int? lineNumber = frame?.GetFileLineNumber();
                string? stackTraceText = ex.StackTrace;
                var errorLog = new ErrorLog
                {
                    ApiName = "CustomerLedger_ManageCommand",
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