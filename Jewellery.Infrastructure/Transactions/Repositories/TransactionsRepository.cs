using Dapper;
using Jewellery.Application.Common.Interfaces;
using Jewellery.Application.Master.Interfaces;
using Jewellery.Application.Transactions.Interfaces;
using Jewellery.Domain.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
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
    }
}