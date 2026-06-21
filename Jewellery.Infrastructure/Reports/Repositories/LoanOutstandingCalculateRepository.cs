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
    public class LoanOutstandingCalculateRepository : ILoanOutstandingCalculateRepository
    {
        private readonly IConfiguration _configuration;
        private readonly ICurrentUserService _currentUser;
        public LoanOutstandingCalculateRepository(IConfiguration configuration, ICurrentUserService currentUser)
        {
            _configuration = configuration;
            _currentUser = currentUser;
        }

        public async Task<dynamic> LoanOutstandingCalculateAsync(int LoanId, DateTime CloserDate)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            var parameters = new DynamicParameters();
            parameters.Add("@UserId", _currentUser.UserId);
            parameters.Add("@LoanId", LoanId);
            parameters.Add("@CloserDate", CloserDate);

            using var result = await connection.QueryMultipleAsync(
                "Jewellery.LoanOutstandingCalculate",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            var LoanOutStanding = await result.ReadFirstAsync<dynamic>();
            var LoanSummary = await result.ReadAsync<dynamic>();
            var LoanTransaction = await result.ReadAsync<dynamic>();

            return new
            {
                LoanOutStanding = LoanOutStanding,
                LoanSummary = LoanSummary,
                LoanTransaction = LoanTransaction
            };
        }
    }
}