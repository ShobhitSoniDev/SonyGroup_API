using Jewellery.Application.Auth.Interfaces;
using Jewellery.Application.Master.Interfaces;
using Jewellery.Application.Master.Models;
using Jewellery.Application.Transactions.Interfaces;
using Jewellery.Domain.Entities;
using MediatR;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Jewellery.Application.Master.Commands
{
    // ✅ Command
    public class Customer_Cart_ManageCommand : IRequest<ResponseModel>
    {
        public int CustomerId { get; set; } = 0;
        public int ProductId { get; set; } = 0;
        public int Quantity { get; set; } = 0;
        public int TypeId { get; set; } = 0;
    }

    // ✅ Handler
    public class Customer_Cart_ManageCommandHandler
        : IRequestHandler<Customer_Cart_ManageCommand, ResponseModel>
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IErrorLogRepository _errorLogRepository;
        public Customer_Cart_ManageCommandHandler(ICustomerRepository customerRepository, IErrorLogRepository errorLogRepository)
        {
            _customerRepository = customerRepository;
            _errorLogRepository = errorLogRepository;
        }

        public async Task<ResponseModel> Handle(Customer_Cart_ManageCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var model = new CartManageRequest
                {
                    CustomerId = request.CustomerId,
                    ProductId = request.ProductId,
                    Quantity = request.Quantity,
                    TypeId = request.TypeId
                };

                var result = await _customerRepository.Customer_Cart_ManageAsync(model);

                if (result != null)
                {
                    return new ResponseModel
                    {
                        Code = 1,
                        Message = "SUCCESS",
                        Data = result
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
            catch(Exception ex)
            {
                var stackTrace = new StackTrace(ex, true);
                var frame = stackTrace.GetFrame(0);

                int? lineNumber = frame?.GetFileLineNumber();
                string? stackTraceText = ex.StackTrace;
                var errorLog = new ErrorLog
                {
                    ApiName = "Customer_Cart_ManageCommand",
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
