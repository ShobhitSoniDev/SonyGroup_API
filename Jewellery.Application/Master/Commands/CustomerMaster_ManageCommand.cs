using MediatR;
using Jewellery.Application.Master.Interfaces;
using Jewellery.Application.Master.Models;
using System.Threading;
using System.Threading.Tasks;
using Jewellery.Domain.Entities;
using System;

namespace Jewellery.Application.Master.Commands
{
    // ✅ Command
    public class CustomerMaster_ManageCommand : IRequest<ResponseModel>
    {
        public string CustomerId { get; set; } = "";
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

        public CustomerMaster_ManageCommandHandler(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task<ResponseModel> Handle(CustomerMaster_ManageCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (request.TypeId == 1 || request.TypeId == 2)
                {
                    var error = CommonInputValidator.Validate(value: request.CustomerName, numeric: false, minLength: 2, maxLength: 20);
                    if (error.Code == 0)
                        return error;
                    error = CommonInputValidator.Validate(value: request.MobileNo.ToString(), numeric: true, minLength: 1, maxLength: 20);
                    if (error.Code == 0)
                        return error;
                    error = CommonInputValidator.Validate(value: request.Address.ToString(), numeric: false, minLength: 1, maxLength: 20);
                    if (error.Code == 0)
                        return error;
                    
                    
                }
                var customermodel = new CustomerMasterModel
                {
                    CustomerId = request.CustomerId,
                    CustomerName = request.CustomerName,
                    MobileNo = request.MobileNo,
                    Email = request.Email,
                    Address = request.Address,
                    City = request.CustomerName,
                    Pincode = request.Pincode,
                    TypeId = request.TypeId
                };
                var insertedproduct = await _customerRepository.ProductMaster_ManageAsync(customermodel);
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
                return new ResponseModel
                {
                    Code = 1,
                    Message = "FAILED"
                };
            }
        }
    }
}
