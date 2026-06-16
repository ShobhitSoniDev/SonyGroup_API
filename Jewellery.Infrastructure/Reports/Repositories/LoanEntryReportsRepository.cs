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
    public class LoanEntryReportsRepository : ILoanEntryReportRepository
    {
        private readonly IConfiguration _configuration;
        private readonly ICurrentUserService _currentUser;
        public LoanEntryReportsRepository(IConfiguration configuration, ICurrentUserService currentUser)
        {
            _configuration = configuration;
            _currentUser = currentUser;
        }

        public async Task<dynamic> LoanEntryReportsAsync(GetLoanEntryReportModel request)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            var parameters = new DynamicParameters();

            parameters.Add("@LoanId", request.LoanId);
            parameters.Add("@CustomerId", request.CustomerId);
            parameters.Add("@LoanType", request.LoanType);
            parameters.Add("@LoanStatus", request.LoanStatus);
            parameters.Add("@MetalType", request.MetalType);
            parameters.Add("@FromDate", request.FromDate);
            parameters.Add("@ToDate", request.ToDate);
            parameters.Add("@AmountFrom", request.AmountFrom);
            parameters.Add("@AmountTo", request.AmountTo);
            parameters.Add("@PageNo", request.PageNo);
            parameters.Add("@PageSize", request.PageSize);

            var result = await connection.QueryAsync("Jewellery.GetLoanEntryReport", parameters, commandType: CommandType.StoredProcedure);
            return result;
        }
    }
}