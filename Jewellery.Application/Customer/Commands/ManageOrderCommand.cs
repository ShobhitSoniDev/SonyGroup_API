using Jewellery.Application.Master.Interfaces;
using Jewellery.Application.Transactions.Interfaces;
using Jewellery.Domain.Entities;
using MediatR;
using System.Diagnostics;

namespace Jewellery.Application.Customer.Commands
{
    // ✅ Command
    public class ManageOrderCommand : IRequest<ResponseModel>
    {
        public int TypeId { get; set; } = 0;       // 1 = Get single order, 2 = List all orders
        public int? OrderId { get; set; }          // required for TypeId 1
    }

    // ✅ Handler
    public class ManageOrderCommandHandler
     : IRequestHandler<ManageOrderCommand, ResponseModel>
    {
        private readonly ICustomerRepository _orderRepository;
        private readonly IErrorLogRepository _errorLogRepository;

        public ManageOrderCommandHandler(
            ICustomerRepository orderRepository,
            IErrorLogRepository errorLogRepository)
        {
            _orderRepository = orderRepository;
            _errorLogRepository = errorLogRepository;
        }

        public async Task<ResponseModel> Handle(ManageOrderCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // 🔥 VALIDATIONS
                if (request.TypeId != 1 && request.TypeId != 2)
                {
                    return new ResponseModel
                    {
                        Code = 0,
                        Message = "Invalid TypeId."
                    };
                }

                if (request.TypeId == 1)
                {
                    var error = CommonInputValidator.Validate(value: (request.OrderId ?? 0).ToString(), numeric: true, minLength: 1, maxLength: 20);
                    if (error.Code == 0)
                        return error;
                }

                var model = new CustomerOrderModel.OrderManageRequest
                {
                    TypeId = request.TypeId,
                    OrderId = request.OrderId
                };

                var result = await _orderRepository.Order_ManageAndReturnAsync(model);

                if (result == null || result.Code != 1)
                {
                    return new ResponseModel
                    {
                        Code = 0,
                        Message = result?.Message ?? "Unable to fetch order details."
                    };
                }

                // 🔥 TypeId 1 — single order + items
                if (request.TypeId == 1)
                {
                    return new ResponseModel
                    {
                        Code = 1,
                        Message = "SUCCESS",
                        Data = new
                        {
                            Order = result.Order,
                            Items = result.Items
                        }
                    };
                }

                // 🔥 TypeId 2 — list of orders
                return new ResponseModel
                {
                    Code = 1,
                    Message = "SUCCESS",
                    Data = result.Data
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
                    ApiName = "ManageOrderCommand",
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
