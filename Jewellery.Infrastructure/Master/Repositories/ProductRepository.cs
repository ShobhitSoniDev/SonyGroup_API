using Dapper;
using Jewellery.Application.Common.Interfaces;
using Jewellery.Application.Master.Interfaces;
using Jewellery.Domain.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;
namespace Jewellery.Infrastructure.Master.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly IConfiguration _configuration;
        private readonly ICurrentUserService _currentUser;
        public ProductRepository(IConfiguration configuration, ICurrentUserService currentUser)
        {
            _configuration = configuration;
            _currentUser = currentUser;
        }

        public async Task<dynamic> ProductMaster_ManageAsync(ProductMasterModel product)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            var parameters = new DynamicParameters();
            parameters.Add("@UserId", _currentUser.UserId);
            parameters.Add("@ProductId", product.ProductId);
            parameters.Add("@ProductName", product.ProductName);
            parameters.Add("@CategoryId", product.CategoryId);
            parameters.Add("@MetalId", product.MetalId);
            parameters.Add("@GrossWeight", product.GrossWeight);
            parameters.Add("@NetWeight", product.NetWeight);
            parameters.Add("@WastageWeight", product.WastageWeight);
            parameters.Add("@MakingCharge", product.MakingCharge);
            parameters.Add("@RatePerGram", product.RatePerGram);
            parameters.Add("@TotalQuantity", product.TotalQuantity);
            parameters.Add("@TypeId", product.TypeId);

            var result = await connection.QueryAsync("Jewellery.ProductMaster_Manage",parameters,commandType: CommandType.StoredProcedure);

            return result;
        }
    }
}