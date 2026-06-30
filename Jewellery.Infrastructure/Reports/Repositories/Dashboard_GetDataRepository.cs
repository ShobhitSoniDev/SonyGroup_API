using Azure.Core;
using Dapper;
using Jewellery.Application.Common.Interfaces;
using Jewellery.Application.Master.Interfaces;
using Jewellery.Domain.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;
namespace Jewellery.Infrastructure.Master.Repositories
{
    public class Dashboard_GetDataRepository : IDashboard_GetDataRepository
    {
        private readonly IConfiguration _configuration;
        private readonly ICurrentUserService _currentUser;
        public Dashboard_GetDataRepository(IConfiguration configuration, ICurrentUserService currentUser)
        {
            _configuration = configuration;
            _currentUser = currentUser;
        }

        public async Task<dynamic> Dashboard_GetDataAsync()
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString(_currentUser.shopCode));
            //using var connection = new SqlConnection(
            //    _configuration.GetConnectionString("DefaultConnection")
            //);
            var parameters = new DynamicParameters();
            parameters.Add("@UserId", _currentUser.UserId);

            using var result = await connection.QueryMultipleAsync(
                "Jewellery.Dashboard_GetData",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            // The SP returns a single result set with (Code, Message) and exits early
            // when the user is not authorized for /dashboard — read the first
            // recordset generically and check for that shape before assuming the
            // normal 6-result-set dashboard payload follows.
            var firstSet = (await result.ReadAsync<dynamic>()).ToList();

            var firstRow = firstSet.FirstOrDefault() as IDictionary<string, object>;
            if (firstRow != null && firstRow.ContainsKey("Code") && firstRow.ContainsKey("Message"))
            {
                return new
                {
                    Authorized = false,
                    Message = firstRow["Message"]?.ToString()
                };
            }

            // Authorized — firstSet is actually SummaryCards, continue reading the
            // remaining 5 result sets in the order the SP returns them.
            var summaryCards = firstSet;
            var metalSummary = await result.ReadAsync<dynamic>();
            var lowStockItems = await result.ReadAsync<dynamic>();
            var recentTransactions = await result.ReadAsync<dynamic>();
            var girviSummary = await result.ReadAsync<dynamic>();
            var stockOverview = await result.ReadAsync<dynamic>();

            return new
            {
                Authorized = true,
                SummaryCards = summaryCards,
                MetalSummary = metalSummary,
                LowStockItems = lowStockItems,
                RecentTransactions = recentTransactions,
                GirviSummary = girviSummary,
                StockOverview = stockOverview
            };
        }
    }
}