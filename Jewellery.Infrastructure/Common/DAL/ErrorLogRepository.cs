using Dapper;
using Jewellery.Application.Common.Interfaces;
using Jewellery.Application.Master.Interfaces;
using Jewellery.Domain.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

public class ErrorLogRepository : IErrorLogRepository
{
    private readonly IDbConnection _db;
    private readonly ICurrentUserService _currentUser;
    private readonly IConfiguration _configuration;
    public ErrorLogRepository(IDbConnection db, ICurrentUserService currentUser, IConfiguration configuration)
    {
        _db = db;
        _currentUser = currentUser;
        _configuration = configuration;
    }

    public async Task SaveErrorAsync(ErrorLog log)
    {
        using var connection = new SqlConnection(_configuration.GetConnectionString(_currentUser.shopCode));
        log.ErrorMessage = log.ErrorMessage?.Replace("'", "");
        log.StackTrace = log.StackTrace?.Replace("'", "");
        var param = new DynamicParameters();
        param.Add("@UserId", _currentUser.UserId);
        param.Add("@ApiName", log.ApiName);
        param.Add("@ErrorMessage", log.ErrorMessage);
        param.Add("@StackTrace", log.StackTrace.ToString());
        param.Add("@LineNumber", log.LineNumber);
        await connection.QueryAsync("Jewellery.SaveErrorLog", param, commandType: CommandType.StoredProcedure);
    }
}
