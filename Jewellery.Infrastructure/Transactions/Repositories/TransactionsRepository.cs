using Dapper;
using Jewellery.Application.Common.Interfaces;
using Jewellery.Application.Master.Interfaces;
using Jewellery.Application.Transactions.Interfaces;
using Jewellery.Domain.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Data;

namespace Jewellery.Infrastructure.Transactions.Repositories
{
    public class TransactionsRepository : ITransactionsRepository
    {
        private readonly IConfiguration _configuration;
        private readonly ICurrentUserService _currentUser;

        public TransactionsRepository(
            IConfiguration configuration,
            ICurrentUserService currentUser)
        {
            _configuration = configuration;
            _currentUser = currentUser;
        }

        public async Task<dynamic> CustomerLedger_ManageAsync(CustomerLedgerModel model)
        {
            using var connection = new SqlConnection(
                _configuration.GetConnectionString(_currentUser.shopCode));

            var parameters = new DynamicParameters();

            parameters.Add("@TransId", model.TransId);
            parameters.Add("@CustomerCode", model.CustomerCode);
            parameters.Add("@TransactionDate", model.TransactionDate);
            parameters.Add("@TransactionType", model.TransactionType);
            parameters.Add("@Amount", model.Amount);
            parameters.Add("@Description", model.Description);
            parameters.Add("@TypeId", model.TypeId);
            parameters.Add("@UserId", _currentUser.UserId);

            var result = await connection.QueryAsync(
                "Jewellery.CustomerLedger_Manage",
                parameters,
                commandType: CommandType.StoredProcedure);

            return result;
        }
        public async Task<dynamic> Sales_ManageAsync(SalesModel model)
        {
            using var connection = new SqlConnection(
                _configuration.GetConnectionString(_currentUser.shopCode));

            var parameters = new DynamicParameters();

            parameters.Add("@TypeId", model.TypeId);
            parameters.Add("@SaleId", model.SaleId);
            parameters.Add("@BillNo", model.BillNo);
            parameters.Add("@BillDate", model.BillDate);
            parameters.Add("@CustomerId", model.CustomerId);
            parameters.Add("@TotalAmount", model.TotalAmount);
            parameters.Add("@GSTAmount", model.GSTAmount);
            parameters.Add("@PaidAmount", model.PaidAmount);
            parameters.Add("@PaymentMode", model.PaymentMode);
            parameters.Add("@Remarks", model.Remarks);
            parameters.Add("@IsActive", model.IsActive);
            parameters.Add("@CreatedBy", _currentUser.UserName);

            // Convert Detail List to JSON
            parameters.Add("@DetailsJson",
                JsonConvert.SerializeObject(model.Details));

            using var result = await connection.QueryMultipleAsync(
                "Jewellery.Sales_Manage",
                parameters,
                commandType: CommandType.StoredProcedure);

            // TypeId = 4 (Select) returns multiple result sets
            if (model.TypeId == 4)
            {
                var header = await result.ReadAsync<dynamic>();
                var details = await result.ReadAsync<dynamic>();

                return new
                {
                    Header = header,
                    Details = details
                };
            }

            // Insert / Update / Delete
            return await result.ReadAsync<dynamic>();
        }
        public async Task<dynamic> Purchase_ManageAsync(PurchaseModel model)
        {
            using var connection = new SqlConnection(
                _configuration.GetConnectionString(_currentUser.shopCode));

            var parameters = new DynamicParameters();

            parameters.Add("@TypeId", model.TypeId);
            parameters.Add("@PurchaseId", model.PurchaseId);
            parameters.Add("@PurchaseNo", model.PurchaseNo);
            parameters.Add("@PurchaseDate", model.PurchaseDate);
            parameters.Add("@SupplierId", model.SupplierId);
            parameters.Add("@TotalAmount", model.TotalAmount);
            parameters.Add("@PaidAmount", model.PaidAmount);
            parameters.Add("@Remarks", model.Remarks);
            parameters.Add("@IsActive", model.IsActive);
            parameters.Add("@CreatedBy", _currentUser.UserId);

            // Purchase Detail List to JSON
            parameters.Add(
                "@DetailsJson",
                JsonConvert.SerializeObject(model.DetailsJson));

            using var result = await connection.QueryMultipleAsync(
                "Jewellery.Purchase_Manage",
                parameters,
                commandType: CommandType.StoredProcedure);

            // Get By Id
            if (model.TypeId == 4)
            {
                var header = await result.ReadAsync<dynamic>();
                var details = await result.ReadAsync<dynamic>();

                return new
                {
                    Header = header,
                    Details = details
                };
            }

            // Insert / Update / Delete / Get All
            return await result.ReadAsync<dynamic>();
        }
    }
}