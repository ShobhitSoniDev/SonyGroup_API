using Jewellery.Application.Master.Models;
using Jewellery.Domain.Entities;

namespace Jewellery.Application.Master.Interfaces
{
    public interface IReportsRepository
    {
        Task<object> CustomerLedgerReportAsync(GetCustomerLedgerReportModel model);
        Task<dynamic> BillGenerateHistoryManageAsync(BillGenerateHistoryModel model);
        Task<dynamic> PurchaseReportAsync(GetPurchaseReportModel model);
        Task<dynamic> SalesReportAsync(GetSalesReportModel model);
        Task<dynamic> StockReportAsync(GetStockReportModel model);
    }
}

