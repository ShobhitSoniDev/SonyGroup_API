using Dapper;
using Jewellery.Application.Common.Interfaces;
using Jewellery.Application.Master.Interfaces;
using Jewellery.Application.Master.Models;
using Jewellery.Domain.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Threading.Tasks;

namespace Jewellery.Infrastructure.Reports.Repositories
{
    public class ReportsRepository : IReportsRepository
    {
        private readonly IConfiguration _configuration;
        private readonly ICurrentUserService _currentUser;

        public ReportsRepository(
            IConfiguration configuration,
            ICurrentUserService currentUser)
        {
            _configuration = configuration;
            _currentUser = currentUser;
        }

        // ── CustomerLedger_Report (Detail / Summary) ──────────────────────────
        public async Task<dynamic> CustomerLedgerReportAsync(GetCustomerLedgerReportModel report)
        {
            using var connection = new SqlConnection(
                _configuration.GetConnectionString(_currentUser.shopCode));

            var parameters = new DynamicParameters();
            parameters.Add("@CustomerCode", report.CustomerCode);
            parameters.Add("@FromDate", report.FromDate);
            parameters.Add("@ToDate", report.ToDate);
            parameters.Add("@TransactionType", report.TransactionType);
            parameters.Add("@TypeId", report.TypeId);

            var result = await connection.QueryAsync(
                "Jewellery.CustomerLedger_Report",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return result;
        }
        public async Task<dynamic> BillGenerateHistoryManageAsync(BillGenerateHistoryModel model)
        {
            using var connection = new SqlConnection(
                _configuration.GetConnectionString(_currentUser.shopCode));

            var parameters = new DynamicParameters();

            parameters.Add("@TypeId", model.TypeId);
            parameters.Add("@BillGenerateId", model.BillGenerateId);
            parameters.Add("@CustomerCode", model.CustomerCode);
            parameters.Add("@BillNo", model.BillNo);
            parameters.Add("@FilePath", model.FilePath);
            parameters.Add("@Description", model.Description);
            parameters.Add("@LanguageType", model.LanguageType);
            parameters.Add("@UserId", _currentUser.UserId);

            using var result = await connection.QueryMultipleAsync(
                "Jewellery.BillGenerateHistory_Manage",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return await result.ReadFirstAsync<dynamic>();


        }
        public async Task<dynamic> PurchaseReportAsync(GetPurchaseReportModel report)
        {
            using var connection = new SqlConnection(
                _configuration.GetConnectionString(_currentUser.shopCode));

            var parameters = new DynamicParameters();
            parameters.Add("@FromDate", report.FromDate);
            parameters.Add("@ToDate", report.ToDate);
            parameters.Add("@SupplierId", report.SupplierId);
            parameters.Add("@ProductId", report.ProductId);
            parameters.Add("@CategoryId", report.CategoryId);
            parameters.Add("@MetalId", report.MetalId);
            parameters.Add("@MetalType", report.MetalType);
            parameters.Add("@PurchaseNo", report.PurchaseNo);
            parameters.Add("@PaymentStatus", report.PaymentStatus);
            parameters.Add("@IsActive", report.IsActive);

            using var multi = await connection.QueryMultipleAsync(
                "Jewellery.GetPurchase_Report",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            var details = (await multi.ReadAsync()).ToList();   // Result Set 1: Detail rows
            var oldJewellery = (await multi.ReadAsync()).ToList();   // Result Set 2: Old Jewellery rows
            var summary = await multi.ReadFirstOrDefaultAsync();// Result Set 3: Overall Summary
            var supplierSummary = (await multi.ReadAsync()).ToList();   // Result Set 4: Supplier-wise Summary
            var metalTypeSummary = (await multi.ReadAsync()).ToList();   // Result Set 5: MetalType-wise Summary

            return new
            {
                Details = details,
                OldJewellery = oldJewellery,
                Summary = summary,
                SupplierSummary = supplierSummary,
                MetalTypeSummary = metalTypeSummary
            };
        }
        public async Task<dynamic> SalesReportAsync(GetSalesReportModel report)
        {
            using var connection = new SqlConnection(
                _configuration.GetConnectionString(_currentUser.shopCode));

            var parameters = new DynamicParameters();
            parameters.Add("@FromDate", report.FromDate);
            parameters.Add("@ToDate", report.ToDate);
            parameters.Add("@CustomerId", report.CustomerId);
            parameters.Add("@CustomerType", report.CustomerType);
            parameters.Add("@ProductId", report.ProductId);
            parameters.Add("@CategoryId", report.CategoryId);
            parameters.Add("@MetalId", report.MetalId);
            parameters.Add("@MetalType", report.MetalType);
            parameters.Add("@BillNo", report.BillNo);
            parameters.Add("@PaymentMode", report.PaymentMode);
            parameters.Add("@PaymentStatus", report.PaymentStatus);
            parameters.Add("@IsActive", report.IsActive);

            using var multi = await connection.QueryMultipleAsync(
                "Jewellery.GetSales_Report",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            var details = (await multi.ReadAsync()).ToList();    // Result Set 1: Sale detail rows
            var oldJewellery = (await multi.ReadAsync()).ToList();    // Result Set 2: Old Jewellery rows
            var summary = await multi.ReadFirstOrDefaultAsync(); // Result Set 3: Overall Summary
            var customerSummary = (await multi.ReadAsync()).ToList();    // Result Set 4: Customer-wise Summary
            var metalTypeSummary = (await multi.ReadAsync()).ToList();    // Result Set 5: MetalType-wise Summary

            return new
            {
                Details = details,
                OldJewellery = oldJewellery,
                Summary = summary,
                CustomerSummary = customerSummary,
                MetalTypeSummary = metalTypeSummary
            };
        }
        public async Task<dynamic> StockReportAsync(GetStockReportModel report)
        {
            using var connection = new SqlConnection(
                _configuration.GetConnectionString(_currentUser.shopCode));

            var parameters = new DynamicParameters();
            parameters.Add("@AsOnDate", report.AsOnDate);
            parameters.Add("@ProductId", report.ProductId);
            parameters.Add("@CategoryId", report.CategoryId);
            parameters.Add("@MetalId", report.MetalId);
            parameters.Add("@MetalType", report.MetalType);
            parameters.Add("@StockStatus", report.StockStatus);
            // Only override the SP's default (5) when the caller actually sent a value
            if (report.LowStockQty.HasValue)
                parameters.Add("@LowStockQty", report.LowStockQty.Value);
            parameters.Add("@IsActive", report.IsActive);

            using var multi = await connection.QueryMultipleAsync(
                "Jewellery.GetStock_Report",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            var details = (await multi.ReadAsync()).ToList();          // Result Set 1: Product-wise stock detail
            var categorySummary = (await multi.ReadAsync()).ToList();  // Result Set 2: Category-wise summary
            var metalSummary = (await multi.ReadAsync()).ToList();     // Result Set 3: Metal-wise summary
            var summary = await multi.ReadFirstOrDefaultAsync();       // Result Set 4: Overall summary
            var lowStockAlerts = (await multi.ReadAsync()).ToList();   // Result Set 5: Low/Out-of-stock alert list

            return new
            {
                Details = details,
                CategorySummary = categorySummary,
                MetalSummary = metalSummary,
                Summary = summary,
                LowStockAlerts = lowStockAlerts
            };
        }
    }
}
