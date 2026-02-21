using Jewellery.Domain.Entities;

namespace Jewellery.Application.Transactions.Interfaces
{
    public interface IStockRepository
    {
        Task<dynamic> StockTransaction_ManageAsync(StockTransactionModel stock);
    }
}

