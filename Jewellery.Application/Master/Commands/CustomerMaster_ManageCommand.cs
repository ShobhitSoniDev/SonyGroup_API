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
    // ✅ Command
    public class CustomerMaster_ManageCommand : IRequest<ResponseModel>
    {
        public string CustomerCode { get; set; } = "";
        public string CustomerName { get; set; } = "";
        public string MobileNo { get; set; } = "";
        public string Email { get; set; } = "";
        public string Address { get; set; } = "";
        public string City { get; set; } = "";
        public int Pincode { get; set; } = 0;
        public int TypeId { get; set; } = 0;
    }

    // ✅ Handler
    public class CustomerMaster_ManageCommandHandler
        : IRequestHandler<CustomerMaster_ManageCommand, ResponseModel>
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IErrorLogRepository _errorLogRepository;
        public CustomerMaster_ManageCommandHandler(ICustomerRepository customerRepository, IErrorLogRepository errorLogRepository)
        {
            _customerRepository = customerRepository;
            _errorLogRepository = errorLogRepository;
        }

        public async Task<ResponseModel> Handle(CustomerMaster_ManageCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (request.TypeId == 1 || request.TypeId == 2)
                {
                    var error = CommonInputValidator.Validate(value: request.CustomerName, numeric: false, minLength: 2, maxLength: 20,allowHindi:true);
                    if (error.Code == 0)
                        return error;
                    error = CommonInputValidator.Validate(value: request.MobileNo.ToString(), numeric: true, minLength: 1, maxLength: 20);
                    if (error.Code == 0)
                        return error;
                    error = CommonInputValidator.Validate(value: request.Address.ToString(), numeric: false, minLength: 1, maxLength: 20, allowHindi: true);
                    if (error.Code == 0)
                        return error;
                    
                    
                }
                var customermodel = new CustomerMasterModel
                {
                    CustomerCode = request.CustomerCode,
                    CustomerName = request.CustomerName,
                    MobileNo = request.MobileNo,
                    Email = request.Email,
                    Address = request.Address,
                    City = request.City,
                    Pincode = request.Pincode,
                    TypeId = request.TypeId
                };
                var insertedproduct = await _customerRepository.CustomerMaster_ManageAsync(customermodel);
                if (insertedproduct != null)
                {
                    return new ResponseModel
                    {
                        Code = 1,
                        Message = "SUCCESS",
                        Data = insertedproduct
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
                    ApiName = "CustomerMaster_ManageCommandHandler",
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
