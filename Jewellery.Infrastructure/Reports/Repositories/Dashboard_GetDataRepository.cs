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
            using var connection = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection")
            );

            var parameters = new DynamicParameters();
            parameters.Add("@UserId", _currentUser.UserId);

            using var result = await connection.QueryMultipleAsync(
                "Jewellery.Dashboard_GetData",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            var summaryCards = await result.ReadAsync<dynamic>();
            var metalSummary = await result.ReadAsync<dynamic>();
            var lowStockItems = await result.ReadAsync<dynamic>();
            var recentTransactions = await result.ReadAsync<dynamic>();
            var girviSummary = await result.ReadAsync<dynamic>();
            var stockOverview = await result.ReadAsync<dynamic>();

            return new
            {
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