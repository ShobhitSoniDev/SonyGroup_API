using MediatR;
using Jewellery.Application.Transactions.Interfaces;
using Jewellery.Domain.Entities;


namespace Jewellery.Application.Transactions.Commands
{
    // ✅ Command
    public class StockTransaction_ManageCommand : IRequest<ResponseModel>
    {
        public int StockId { get; set; } = 0;
        public int ProductId { get; set; } = 0;
        public int TransactionType { get; set; } = 0;
        public int Quantity { get; set; } = 0;
        public decimal Weight { get; set; } = 0;
        public int ReferenceType { get; set; } = 0;
        public string ReferenceNo { get; set; } = "";
        public string TransactionDate { get; set; } = "";
        public int TypeId { get; set; } = 0;
    }

    // ✅ Handler
    public class StockTransaction_ManageCommandHandler
        : IRequestHandler<StockTransaction_ManageCommand, ResponseModel>
    {
        private readonly IStockRepository _stockRepository;

        public StockTransaction_ManageCommandHandler(IStockRepository stockRepository)
        {
            _stockRepository = stockRepository;
        }

        public async Task<ResponseModel> Handle(StockTransaction_ManageCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var error = CommonInputValidator.Validate(value: request.TypeId.ToString(), numeric: true, minLength: 2, maxLength: 20);
                if (error.Code == 0)
                    return error;
                if (request.TypeId == 1 || request.TypeId == 2)
                {
                    error = CommonInputValidator.Validate(value: request.ProductId.ToString(), numeric: true, minLength: 2, maxLength: 20);
                    if (error.Code == 0)
                        return error;
                    error = CommonInputValidator.Validate(value: request.TransactionType.ToString(), numeric: true, minLength: 1, maxLength: 20);
                    if (error.Code == 0)
                        return error;
                    error = CommonInputValidator.Validate(value: request.Quantity.ToString(), numeric: true, minLength: 1, maxLength: 20);
                    if (error.Code == 0)
                        return error;
                    error = CommonInputValidator.Validate(value: request.Weight.ToString(), numeric: true, allowDecimal: true, minLength: 1, maxLength: 20);
                    error = CommonInputValidator.Validate(value: request.ReferenceType.ToString(), numeric: true, allowDecimal: false, minLength: 1, maxLength: 20);
                    error = CommonInputValidator.Validate(value: request.ReferenceNo.ToString(), numeric: false, allowDecimal: false, minLength: 1, maxLength: 20);
                    error = CommonInputValidator.Validate(value: request.TransactionDate.ToString(), numeric: false, allowDecimal: false, minLength: 1, maxLength: 20);
                }
                var stockmodel = new StockTransactionModel
                {
                    StockId = request.StockId,
                    ProductId = request.ProductId,
                    TransactionType = request.TransactionType,
                    Quantity = request.Quantity,
                    Weight = request.Weight,
                    ReferenceType = request.ReferenceType,
                    ReferenceNo = request.ReferenceNo,
                    TransactionDate = request.TransactionDate,
                    TypeId = request.TypeId
                };
                var insertedproduct = await _stockRepository.StockTransaction_ManageAsync(stockmodel);
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
