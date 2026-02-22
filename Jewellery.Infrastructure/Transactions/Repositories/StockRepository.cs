using Dapper;
using Jewellery.Application.Common.Interfaces;
using Jewellery.Application.Transactions.Interfaces;
using Jewellery.Domain.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;
namespace Jewellery.Infrastructure.Transactions.Repositories
{
    public class StockRepository : IStockRepository
    {
        private readonly IConfiguration _configuration;
        private readonly ICurrentUserService _currentUser;
        public StockRepository(IConfiguration configuration, ICurrentUserService currentUser)
        {
            _configuration = configuration;
            _currentUser = currentUser;
        }

        public async Task<dynamic> StockTransaction_ManageAsync(StockTransactionModel product)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            var parameters = new DynamicParameters();
            parameters.Add("@UserId", _currentUser.UserId);
            parameters.Add("@StockId", product.StockId);
            parameters.Add("@ProductId", product.ProductId);
            parameters.Add("@TransactionType", product.TransactionType);
            parameters.Add("@Quantity", product.Quantity);
            parameters.Add("@Weight", product.Weight);
            parameters.Add("@ReferenceType", product.ReferenceType);
            parameters.Add("@ReferenceNo", product.ReferenceNo);
            parameters.Add("@TransactionDate", product.TransactionDate);
            parameters.Add("@TypeId", product.TypeId);
            var result = await connection.QueryAsync("Jewellery.StockTransaction_Manage", parameters,commandType: CommandType.StoredProcedure);
            return result;
        }
    }
}