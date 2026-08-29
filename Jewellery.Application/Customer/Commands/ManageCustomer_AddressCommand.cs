using Jewellery.Application.Master.Interfaces;
using Jewellery.Application.Transactions.Interfaces;
using Jewellery.Domain.Entities;
using MediatR;
using System.Diagnostics;

namespace Jewellery.Application.Customer.Commands
{
    // ✅ Command
    public class ManageCustomer_AddressCommand : IRequest<ResponseModel>
    {
        public int TypeId { get; set; } = 0;          // 1=Add, 2=Update, 3=Delete, 4=Bind All
        public int AddressId { get; set; } = 0;        // required for Update / Delete
        public string AddressLabel { get; set; } = "HOME";
        public string AddressLine { get; set; } = "";
        public string MobileNo { get; set; } = "";
        public string City { get; set; } = "";
        public string? State { get; set; } = "";
        public string Pincode { get; set; } = "";
        public bool IsDefault { get; set; } = false;
    }

    // ✅ Handler
    public class ManageCustomer_AddressCommandHandler
     : IRequestHandler<ManageCustomer_AddressCommand, ResponseModel>
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IErrorLogRepository _errorLogRepository;

        public ManageCustomer_AddressCommandHandler(ICustomerRepository customerRepository, IErrorLogRepository errorLogRepository)
        {
            _customerRepository = customerRepository;
            _errorLogRepository = errorLogRepository;
        }

        public async Task<ResponseModel> Handle(ManageCustomer_AddressCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // 🔥 CustomerId is mandatory for every operation
                var error = CommonInputValidator.Validate(value:"", numeric: true, minLength: 1, maxLength: 20);
                // 🔥 ADD / UPDATE VALIDATIONS
                if (request.TypeId == 1 || request.TypeId == 2)
                {
                    error = CommonInputValidator.Validate(value: request.AddressLine, numeric: false, minLength: 5, maxLength: 250);
                    if (error.Code == 0)
                        return error;

                    error = CommonInputValidator.Validate(value: request.City, numeric: false, minLength: 2, maxLength: 100);
                    if (error.Code == 0)
                        return error;

                    error = CommonInputValidator.Validate(value: request.Pincode, numeric: true, minLength: 4, maxLength: 10);
                    if (error.Code == 0)
                        return error;
                }

                // 🔥 UPDATE / DELETE VALIDATION
                if (request.TypeId == 2 || request.TypeId == 3)
                {
                    error = CommonInputValidator.Validate(value: request.AddressId.ToString(), numeric: true, minLength: 1, maxLength: 20);
                    if (error.Code == 0)
                        return error;
                }
                var model = new CustomerAddressRequest
                {
                    TypeId = request.TypeId,
                    AddressId = request.AddressId,
                    AddressLabel = request.AddressLabel,
                    AddressLine = request.AddressLine,
                    MobileNo=request.MobileNo,
                    City = request.City,
                    State = request.State,
                    Pincode = request.Pincode,
                    IsDefault = request.IsDefault
                };
                // 🔥 ADD / UPDATE / DELETE / BIND ALL — SP RETURNS ROW(S) (dynamic)
                var result = await _customerRepository.CustomerAddress_ManageAndReturnAsync(model);

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
            catch (Exception ex)
            {
                var stackTrace = new StackTrace(ex, true);
                var frame = stackTrace.GetFrame(0);
                int? lineNumber = frame?.GetFileLineNumber();
                string? stackTraceText = ex.StackTrace;

                var errorLog = new ErrorLog
                {
                    ApiName = "ManageCustomer_AddressCommand",
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
