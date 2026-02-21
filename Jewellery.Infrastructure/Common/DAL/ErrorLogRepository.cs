using Dapper;
using Jewellery.Application.Master.Interfaces;
using Jewellery.Domain.Entities;
using System.Data;

public class ErrorLogRepository : IErrorLogRepository
{
    private readonly IDbConnection _db;

    public ErrorLogRepository(IDbConnection db)
    {
        _db = db;
    }

    public async Task SaveErrorAsync(ErrorLog log)
    {
        var param = new DynamicParameters();
        param.Add("@ApiName", log.ApiName);
        param.Add("@ErrorMessage", log.ErrorMessage);
        param.Add("@StackTrace", log.StackTrace);
        param.Add("@LineNumber", log.LineNumber);
        await _db.ExecuteAsync("Jewellery.SaveErrorLog", param, commandType: CommandType.StoredProcedure);
    }
}
