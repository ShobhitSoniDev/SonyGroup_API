using Dapper;
using Jewellery.Application.Common.Interfaces;
using Jewellery.Application.Master.Interfaces;
using Jewellery.Domain.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;
namespace Jewellery.Infrastructure.Master.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly IConfiguration _configuration;
        private readonly ICurrentUserService _currentUser;
        public CustomerRepository(IConfiguration configuration, ICurrentUserService currentUser)
        {
            _configuration = configuration;
            _currentUser = currentUser;
        }

        public async Task<dynamic> CustomerMaster_ManageAsync(CustomerMasterModel customer)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString(_currentUser.shopCode));
            //using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            var parameters = new DynamicParameters();
            parameters.Add("@UserId", _currentUser.UserId);
            parameters.Add("@CustomerCode", customer.CustomerCode);
            parameters.Add("@CustomerName", customer.CustomerName);
            parameters.Add("@MobileNo", customer.MobileNo);
            parameters.Add("@Email", customer.Email);
            parameters.Add("@Address", customer.Address);
            parameters.Add("@City", customer.City);
            parameters.Add("@Pincode", customer.Pincode);
            parameters.Add("@TypeId", customer.TypeId);

            var result = await connection.QueryAsync("Jewellery.CustomerMaster_Manage",parameters,commandType: CommandType.StoredProcedure);

            return result;
        }
    }
}