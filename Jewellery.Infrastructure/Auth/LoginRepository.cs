using Dapper;
using Jewellery.Application.Auth.Interfaces;
using Jewellery.Application.Master.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace Jewellery.Infrastructure.Master.Repositories
{
    public class LoginRepository : ILoginRepository
    {
        private readonly IConfiguration _configuration;

        public LoginRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // 🔥 INSERT + RETURN INSERTED ROW (DYNAMIC)
        public async Task<dynamic> LoginReturnAsync(string username)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            var parameters = new DynamicParameters();
            parameters.Add("@username", username);
            // Stored Procedure MUST return SELECT
            return await connection.QueryFirstOrDefaultAsync("Jewellery.Login_Check", parameters, commandType: CommandType.StoredProcedure);
        }
    }
}
