using Dapper;
using Jewellery.Application.Common.Interfaces;
using Jewellery.Application.Master.Interfaces;
using Jewellery.Application.Transactions.Interfaces;
using Jewellery.Domain.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Data;

namespace Jewellery.Infrastructure.Transactions.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly IConfiguration _configuration;
        private readonly ICurrentUserService _currentUser;

        public AuthRepository(
            IConfiguration configuration,
            ICurrentUserService currentUser)
        {
            _configuration = configuration;
            _currentUser = currentUser;
        }
        public async Task<dynamic> LoginReturnAsync(string username, string shopCode)
        {
            using var connection = new SqlConnection(
                _configuration.GetConnectionString(shopCode));

            //using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            var parameters = new DynamicParameters();
            parameters.Add("@username", username);
            // Stored Procedure MUST return SELECT
            return await connection.QueryFirstOrDefaultAsync("Jewellery.Login_Check", parameters, commandType: CommandType.StoredProcedure);
        }
        public async Task<dynamic> SignUpReturnAsync(string UserName, string Email, string Password, string MobileNo, int Type, string shopCode,string Role)
        {
            shopCode = "JWL_" + shopCode;
            using var connection = new SqlConnection(_configuration.GetConnectionString(shopCode));

            //using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            var parameters = new DynamicParameters();
            parameters.Add("@UserName", UserName);
            parameters.Add("@Email", Email);
            parameters.Add("@Password", Password);
            parameters.Add("@MobileNo", MobileNo);
            parameters.Add("@Type", Type);
            parameters.Add("@Role", Role);
            // Stored Procedure MUST return SELECT
            return await connection.QueryFirstOrDefaultAsync("Jewellery.SignUp_User", parameters, commandType: CommandType.StoredProcedure);
        }
        public async Task<dynamic> GetMenuReturnAsync()
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString(_currentUser.shopCode));
            // using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            var parameters = new DynamicParameters();
            parameters.Add("@UserId", _currentUser.UserId);
            // Stored Procedure MUST return SELECT
            return await connection.QueryAsync("Jewellery.GetMenu_ByUserId", parameters, commandType: CommandType.StoredProcedure);
        }
        public async Task<dynamic> LoginCustomerReturnAsync(string username, string shopCode)
        {
            using var connection = new SqlConnection(
                _configuration.GetConnectionString(shopCode));

            //using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            var parameters = new DynamicParameters();
            parameters.Add("@username", username);
            // Stored Procedure MUST return SELECT
            return await connection.QueryFirstOrDefaultAsync("Jewellery.Login_Check", parameters, commandType: CommandType.StoredProcedure);
        }
    }
}