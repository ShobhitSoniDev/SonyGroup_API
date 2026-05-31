using Dapper;
using Jewellery.Application.Common.Interfaces;
using Jewellery.Application.Master.Interfaces;
using Jewellery.Application.Transactions.Interfaces;
using Jewellery.Domain.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace Jewellery.Infrastructure.Transactions.Repositories
{
    public class LoanRepository : ILoanRepository
    {
        private readonly IConfiguration _configuration;
        private readonly ICurrentUserService _currentUser;

        public LoanRepository(IConfiguration configuration, ICurrentUserService currentUser)
        {
            _configuration = configuration;
            _currentUser = currentUser;
        }

        public async Task<dynamic> LoanEntry_ManageAsync(LoanEntryModel loan)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            var parameters = new DynamicParameters();

            parameters.Add("@UserId", _currentUser.UserId);
            parameters.Add("@LoanId", loan.LoanId);
            parameters.Add("@CustomerId", loan.CustomerId);
            parameters.Add("@LoanType", loan.LoanType);
            parameters.Add("@Amount", loan.Amount);
            parameters.Add("@InterestType", loan.InterestType);
            parameters.Add("@InterestRate", loan.InterestRate);
            parameters.Add("@StartDate", loan.StartDate);
            parameters.Add("@EndDate", loan.EndDate);
            parameters.Add("@Duration", loan.Duration);
            parameters.Add("@MetalType", loan.MetalType);
            parameters.Add("@Weight", loan.Weight);
            parameters.Add("@ItemCount", loan.ItemCount);

            parameters.Add("@Description", loan.Description);
            parameters.Add("@Remark", loan.Remark);

            parameters.Add("@PhotoPath", loan.PhotoPath); // 👈 CSV of images

            parameters.Add("@TypeId", loan.TypeId);

            var result = await connection.QueryAsync(
                "Jewellery.LoanEntry_Manage",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return result;
        }
    }
}