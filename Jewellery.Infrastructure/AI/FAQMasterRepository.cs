using Dapper;
using Jewellery.Application.Common.Interfaces;
using Jewellery.Application.Master.Interfaces;
using Jewellery.Domain.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace Jewellery.Infrastructure.Master.Repositories
{
    public class FAQMasterRepository : IFAQMasterRepository
    {
        private readonly IConfiguration _configuration;
        private readonly ICurrentUserService _currentUser;

        public FAQMasterRepository(IConfiguration configuration, ICurrentUserService currentUser)
        {
            _configuration = configuration;
            _currentUser = currentUser;
        }

        // 🔥 INSERT + RETURN INSERTED ROW (DYNAMIC)
        public async Task<dynamic> FAQMaster_ManageAndReturnAsync(FAQMasterModel model)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            var parameters = new DynamicParameters();
            parameters.Add("@Id", model.Id);
            parameters.Add("@Question", model.Question);
            parameters.Add("@Answer", model.Answer);
            parameters.Add("@Keywords", model.Keywords);
            parameters.Add("@SearchText", model.SearchText);
            parameters.Add("@TypeId", model.TypeId);
            // Stored Procedure MUST return SELECT
            return await connection.QueryAsync("Jewellery.FAQMaster_Manage", parameters,commandType: CommandType.StoredProcedure);
        }
    }
}
