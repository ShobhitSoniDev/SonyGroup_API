using Dapper;
using Jewellery.Application.Auth.Interfaces;
using Jewellery.Application.Common.Interfaces;
using Jewellery.Application.Master.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace Jewellery.Infrastructure.Master.Repositories
{
    public class GetMenuRepository : IGetMenuRepository
    {
        private readonly IConfiguration _configuration;
        private readonly ICurrentUserService _currentUser;
        public GetMenuRepository(IConfiguration configuration, ICurrentUserService currentUser)
        {
            _configuration = configuration;
            _currentUser = currentUser;
        }

        // 🔥 INSERT + RETURN INSERTED ROW (DYNAMIC)
        public async Task<dynamic> GetMenuReturnAsync()
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            var parameters = new DynamicParameters();
            parameters.Add("@UserId", _currentUser.UserId);
            // Stored Procedure MUST return SELECT
            return await connection.QueryAsync("Jewellery.GetMenu_ByUserId", parameters, commandType: CommandType.StoredProcedure);
        }
    }
}
