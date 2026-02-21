using Dapper;
using System.Data;
using System.Threading;

public interface IDAL
{
    Task<int> ExecuteQuery(
        string query,
        CommandType commandType,
        DynamicParameters parameters,
        int timeout,
        string description,
        CancellationToken cancellationToken
    );
    Task<int> RunMultipleQueryAsync(
        string query,
        CommandType commandType,
        string connectionString,
        DynamicParameters parameters,
        int timeout,
        string description,
        CancellationToken cancellationToken
    );

    Task<object> GetScalarData(
        string query,
        CommandType commandType,
        string connectionString,
        DynamicParameters parameters,
        int timeout,
        string description,
        CancellationToken cancellationToken
    );

    Task<string> GetScalarStringData(
        string query,
        CommandType commandType,
        string connectionString,
        DynamicParameters parameters,
        int timeout,
        string description,
        CancellationToken cancellationToken
    );

    int SQLBulkUpload(
        DataTable dt,
        List<string> columnMappings,
        string tableName,
        DynamicParameters parameters,
        int timeout
    );

    Task<object> GetAggregatorScalarData(
        string query,
        List<string> columnMappings,
        CommandType commandType,
        DynamicParameters parameters,
        int timeout,
        string connectionString,
        CancellationToken cancellationToken
    );
    Task<IEnumerable<T>> GetTableData<T>(
    string query,
    CommandType commandType,
    string connectionString,
    DynamicParameters parameters,
    int timeout,
    string description,
    CancellationToken cancellationToken
);

}
