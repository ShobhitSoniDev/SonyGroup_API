using Jewellery.Domain.Entities;

namespace Jewellery.Application.Transactions.Interfaces
{
    public interface ITransactionsRepository
    {
        Task<dynamic> CustomerLedger_ManageAsync(CustomerLedgerModel stock);
        Task<dynamic> Sales_ManageAsync(SalesModel sales);
        Task<dynamic> Purchase_ManageAsync(PurchaseModel purchase);
    }
}

