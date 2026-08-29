using Jewellery.Application.Master.Interfaces;
using Jewellery.Application.Transactions.Interfaces;
using Jewellery.Domain.Entities;
using MediatR;
using System.Diagnostics;
using static Jewellery.Domain.Entities.CustomerOrderModel;

namespace Jewellery.Application.Customer.Commands
{
    // ✅ Command
    public class VerifyPaymentCommand : IRequest<ResponseModel>
    {
        public int OrderId { get; set; } = 0;
        public string RazorpayOrderId { get; set; } = "";
        public string RazorpayPaymentId { get; set; } = "";
        public string RazorpaySignature { get; set; } = "";
    }

    // ✅ Handler
    public class VerifyPaymentCommandHandler
     : IRequestHandler<VerifyPaymentCommand, ResponseModel>
    {
        private readonly ICustomerRepository _orderRepository;
        private readonly IRazorpayServiceRepository _razorpayService;
        private readonly IErrorLogRepository _errorLogRepository;

        public VerifyPaymentCommandHandler(
            ICustomerRepository orderRepository,
            IRazorpayServiceRepository razorpayService,
            IErrorLogRepository errorLogRepository)
        {
            _orderRepository = orderRepository;
            _razorpayService = razorpayService;
            _errorLogRepository = errorLogRepository;
        }

        public async Task<ResponseModel> Handle(VerifyPaymentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // 🔥 VALIDATIONS
                var error = CommonInputValidator.Validate(value: request.OrderId.ToString(), numeric: true, minLength: 1, maxLength: 20);
                if (error.Code == 0)
                    return error;

                if (string.IsNullOrWhiteSpace(request.RazorpayOrderId) ||
                    string.IsNullOrWhiteSpace(request.RazorpayPaymentId) ||
                    string.IsNullOrWhiteSpace(request.RazorpaySignature))
                {
                    return new ResponseModel
                    {
                        Code = 0,
                        Message = "Missing payment verification details."
                    };
                }

                // 🔥 STEP 1 — Verify signature server-side. NEVER trust the
                // client-sent "success" flag; the signature is the only
                // proof the payment actually belongs to this order.
                bool isValid = _razorpayService.VerifySignature(
                    request.RazorpayOrderId,
                    request.RazorpayPaymentId,
                    request.RazorpaySignature);

                // 🔥 STEP 2 — Persist verified/failed outcome & update order
                var model = new PaymentVerifyRequest
                {
                    OrderId = request.OrderId,
                    RazorpayOrderId = request.RazorpayOrderId,
                    RazorpayPaymentId = request.RazorpayPaymentId,
                    RazorpaySignature = request.RazorpaySignature,
                    IsValid = isValid
                };

                var result = await _orderRepository.Payment_VerifyAndReturnAsync(model);

                if (!isValid)
                {
                    return new ResponseModel
                    {
                        Code = 0,
                        Message = "Payment verification failed."
                    };
                }

                if (result == null || (int)result.Code != 1)
                {
                    return new ResponseModel
                    {
                        Code = 0,
                        Message = result?.Message ?? "Unable to confirm payment."
                    };
                }

                return new ResponseModel
                {
                    Code = 1,
                    Message = "SUCCESS",
                    Data = new
                    {
                        OrderId = request.OrderId,
                        PaymentStatus = "PAID",
                        OrderStatus = "CONFIRMED"
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
                    ApiName = "VerifyPaymentCommand",
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
