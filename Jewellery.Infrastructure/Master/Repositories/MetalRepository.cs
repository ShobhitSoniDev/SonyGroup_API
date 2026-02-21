using Dapper;
using Jewellery.Application.Master.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace Jewellery.Infrastructure.Master.Repositories
{
    public class MetalRepository : IMetalRepository
    {
        private readonly IConfiguration _configuration;

        public MetalRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // 🔥 INSERT + RETURN INSERTED ROW (DYNAMIC)
        public async Task<dynamic> MetalMaster_ManageAndReturnAsync(string metalName,int purity,string createdBy,int TypeId,int MetalId)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            var parameters = new DynamicParameters();
            parameters.Add("@MetalName", metalName);
            parameters.Add("@Purity", purity);
            parameters.Add("@CreatedBy", createdBy);
            parameters.Add("@TypeId", TypeId);
            parameters.Add("@MetalId", MetalId);
            // Stored Procedure MUST return SELECT
            return await connection.QueryAsync("Jewellery.MetalMaster_Manage", parameters,commandType: CommandType.StoredProcedure);
        }
    }
}
