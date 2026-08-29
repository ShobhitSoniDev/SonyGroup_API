using Jewellery.Application.Auth.Interfaces;
using Jewellery.Application.Master.Interfaces;
using Jewellery.Application.Master.Models;
using Jewellery.Application.Services.Interfaces;
using Jewellery.Application.Transactions.Interfaces;
using Jewellery.Domain.Entities;
using MediatR;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using static Jewellery.Domain.Entities.CustomerOrderModel;

namespace Jewellery.Application.Master.Commands
{
    // ✅ Command
    public class PlaceOrderCommand : IRequest<ResponseModel>
    {
        public int TypeId { get; set; } = 0;       // 1 = COD, 2 = Online (Razorpay)
        public int AddressId { get; set; } = 0;
        public string PaymentMode { get; set; } = ""; // "COD" | "CARD" | "UPI" | "NETBANKING" | "WALLET"
    }

    // ✅ Handler
    public class PlaceOrderCommandHandler
     : IRequestHandler<PlaceOrderCommand, ResponseModel>
    {
        private readonly ICustomerRepository _orderRepository;
        private readonly IRazorpayServiceRepository _razorpayService;
        private readonly IErrorLogRepository _errorLogRepository;

        public PlaceOrderCommandHandler(
            ICustomerRepository orderRepository,
            IRazorpayServiceRepository razorpayService,
            IErrorLogRepository errorLogRepository)
        {
            _orderRepository = orderRepository;
            _razorpayService = razorpayService;
            _errorLogRepository = errorLogRepository;
        }

        public async Task<ResponseModel> Handle(PlaceOrderCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // 🔥 VALIDATIONS
                var error = CommonInputValidator.Validate(value: request.AddressId.ToString(), numeric: true, minLength: 1, maxLength: 20);
                if (error.Code == 0)
                    return error;

                if (request.TypeId != 1 && request.TypeId != 2)
                {
                    return new ResponseModel
                    {
                        Code = 0,
                        Message = "Invalid order type."
                    };
                }

                // 🔥 STEP 1 — Create the order (COD confirms immediately;
                // Online stays PENDING until payment is verified)
                var placeModel = new OrderPlaceRequest
                {
                    TypeId = request.TypeId,
                    AddressId = request.AddressId,
                    PaymentMode = request.PaymentMode
                };

                var orderResult = await _orderRepository.Order_PlaceAndReturnAsync(placeModel);

                if (orderResult == null || (int)orderResult.Code != 1)
                {
                    return new ResponseModel
                    {
                        Code = 0,
                        Message = orderResult?.Message ?? "Unable to place order."
                    };
                }

                int orderId = (int)orderResult.OrderId;
                decimal amount = (decimal)orderResult.Amount;

                // 🔥 STEP 2 — COD: done, return as-is
                if (request.TypeId == 1)
                {
                    return new ResponseModel
                    {
                        Code = 1,
                        Message = "SUCCESS",
                        Data = new
                        {
                            OrderId = orderId,
                            Amount = amount,
                            PaymentMode = "COD"
                        }
                    };
                }

                // 🔥 STEP 3 — Online: create a Razorpay order via their API
                var razorpayResult = await _razorpayService.CreateOrderAsync(
                    amount,
                    "INR",
                    receipt: $"order_rcpt_{orderId}");

                if (!razorpayResult.Success || string.IsNullOrEmpty(razorpayResult.RazorpayOrderId))
                {
                    return new ResponseModel
                    {
                        Code = 0,
                        Message = razorpayResult.ErrorMessage ?? "Unable to initiate payment."
                    };
                }

                // 🔥 STEP 4 — Persist the RazorpayOrderId against our order
                var updateModel = new RazorpayOrderUpdateRequest
                {
                    OrderId = orderId,
                    RazorpayOrderId = razorpayResult.RazorpayOrderId
                };

                var updateResult = await _orderRepository.Order_UpdateRazorpayOrderIdAndReturnAsync(updateModel);

                if (updateResult == null || (int)updateResult.Code != 1)
                {
                    return new ResponseModel
                    {
                        Code = 0,
                        Message = "Order created but failed to link payment. Please try again."
                    };
                }

                return new ResponseModel
                {
                    Code = 1,
                    Message = "SUCCESS",
                    Data = new
                    {
                        OrderId = orderId,
                        RazorpayOrderId = razorpayResult.RazorpayOrderId,
                        Amount = amount,
                        Currency = "INR"
                    }
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
                    ApiName = "PlaceOrderCommand",
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
