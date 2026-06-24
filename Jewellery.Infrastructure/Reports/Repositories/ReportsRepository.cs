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
    }
}
