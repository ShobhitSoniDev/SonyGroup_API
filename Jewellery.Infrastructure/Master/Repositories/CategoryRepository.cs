using Dapper;
using Jewellery.Application.Master.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace Jewellery.Infrastructure.Master.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly IConfiguration _configuration;

        public CategoryRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<dynamic> CategoryMaster_ManageAsync(int MetalId, string CategoryName, string createdBy, int TypeId, int CategoryId)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            var param = new DynamicParameters();
            param.Add("@MetalId", MetalId);
            param.Add("@CategoryName", CategoryName);
            param.Add("@CreatedBy", createdBy);
            param.Add("@TypeId", TypeId);
            param.Add("@CategoryId", CategoryId);
            // Stored Procedure MUST return SELECT
            return await connection.QueryAsync("Jewellery.CategoryMaster_Manage", param, commandType: CommandType.StoredProcedure);
        }
    }

}
