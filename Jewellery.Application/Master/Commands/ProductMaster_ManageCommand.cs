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
    public class ProductMaster_ManageCommand : IRequest<ResponseModel>
    {
        public int ProductId { get; set; } = 0;
        public string ProductName { get; set; } = "";
        public int CategoryId { get; set; } = 0;
        public int MetalId { get; set; } = 0;
        public decimal GrossWeight { get; set; } = 0;
        public decimal NetWeight { get; set; } = 0;
        public decimal WastageWeight { get; set; } = 0;
        public decimal MakingCharge { get; set; } = 0;
        public decimal RatePerGram { get; set; } = 0;
        public int TotalQuantity { get; set; } = 0;
        public int TypeId { get; set; } = 0;
    }

    // ✅ Handler
    public class ProductMaster_ManageCommandHandler
        : IRequestHandler<ProductMaster_ManageCommand, ResponseModel>
    {
        private readonly IProductRepository _productRepository;

        public ProductMaster_ManageCommandHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<ResponseModel> Handle(ProductMaster_ManageCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (request.TypeId == 1 || request.TypeId == 2)
                {
                    var error = CommonInputValidator.Validate(value: request.ProductName, numeric: false, minLength: 2, maxLength: 20);
                    if (error.Code == 0)
                        return error;
                    error = CommonInputValidator.Validate(value: request.MetalId.ToString(), numeric: true, minLength: 1, maxLength: 20);
                    if (error.Code == 0)
                        return error;
                    error = CommonInputValidator.Validate(value: request.CategoryId.ToString(), numeric: true, minLength: 1, maxLength: 20);
                    if (error.Code == 0)
                        return error;
                    error = CommonInputValidator.Validate(value: request.GrossWeight.ToString(), numeric: true, allowDecimal: true, minLength: 1, maxLength: 20);
                    error = CommonInputValidator.Validate(value: request.NetWeight.ToString(), numeric: true, allowDecimal: true, minLength: 1, maxLength: 20);
                    error = CommonInputValidator.Validate(value: request.WastageWeight.ToString(), numeric: true, allowDecimal: true, minLength: 1, maxLength: 20);
                    error = CommonInputValidator.Validate(value: request.MakingCharge.ToString(), numeric: true, allowDecimal: true, minLength: 1, maxLength: 20);
                    error = CommonInputValidator.Validate(value: request.RatePerGram.ToString(), numeric: true, allowDecimal: true, minLength: 1, maxLength: 20);
                    error = CommonInputValidator.Validate(value: request.TotalQuantity.ToString(), numeric: true, allowDecimal: false, minLength: 1, maxLength: 20);
                    
                }
                var productmodel = new ProductMasterModel
                {
                    ProductId = request.ProductId,
                    ProductName = request.ProductName,
                    CategoryId = request.CategoryId,
                    MetalId = request.MetalId,
                    GrossWeight = request.GrossWeight,
                    NetWeight = request.NetWeight,
                    WastageWeight = request.WastageWeight,
                    MakingCharge = request.MakingCharge,
                    RatePerGram = request.RatePerGram,
                    TotalQuantity = request.TotalQuantity,
                    TypeId = request.TypeId
                };
                var insertedproduct = await _productRepository.ProductMaster_ManageAsync(productmodel);
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
