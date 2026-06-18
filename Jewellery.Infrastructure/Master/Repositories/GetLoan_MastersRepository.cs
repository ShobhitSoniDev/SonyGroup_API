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
    public class GetLoan_MastersRepository : IGetLoan_MastersRepository
    {
        private readonly IConfiguration _configuration;
        private readonly ICurrentUserService _currentUser;
        public GetLoan_MastersRepository(IConfiguration configuration, ICurrentUserService currentUser)
        {
            _configuration = configuration;
            _currentUser = currentUser;
        }

        public async Task<dynamic> GetLoan_MastersAsync()
        {
            using var connection = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection")
            );

            var parameters = new DynamicParameters();
            parameters.Add("@UserId", _currentUser.UserId);

            using var result = await connection.QueryMultipleAsync(
                "Jewellery.GetLoan_Masters",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            var LoanType_Master = await result.ReadAsync<dynamic>();
            var LoanInterestType_Master = await result.ReadAsync<dynamic>();
            var LoanMetalType_Master = await result.ReadAsync<dynamic>();
            var LoanStatus_Master = await result.ReadAsync<dynamic>();
            var LoanTransactionType = await result.ReadAsync<dynamic>();

            return new
            {
                LoanType_Master = LoanType_Master,
                LoanInterestType_Master = LoanInterestType_Master,
                LoanMetalType_Master = LoanMetalType_Master,
                LoanStatus_Master = LoanStatus_Master,
                LoanTransactionType = LoanTransactionType
            };
        }
    }
}