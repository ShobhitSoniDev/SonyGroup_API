using Dapper;
using Jewellery.Application.Auth.Interfaces;
using Jewellery.Application.Master.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace Jewellery.Infrastructure.Master.Repositories
{
    public class SignUpRepository  : ISignUpRepository
    {
        private readonly IConfiguration _configuration;

        public SignUpRepository (IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // 🔥 INSERT + RETURN INSERTED ROW (DYNAMIC)
        public async Task<dynamic> SignUpReturnAsync(string UserName, string Email, string Password, string MobileNo, int Type)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            var parameters = new DynamicParameters();
            parameters.Add("@UserName", UserName);
            parameters.Add("@Email", Email);
            parameters.Add("@Password", Password);
            parameters.Add("@MobileNo", MobileNo);
            parameters.Add("@Type", Type);
            // Stored Procedure MUST return SELECT
            return await connection.QueryFirstOrDefaultAsync("Jewellery.SignUp_User", parameters, commandType: CommandType.StoredProcedure);
        }
    }
}
