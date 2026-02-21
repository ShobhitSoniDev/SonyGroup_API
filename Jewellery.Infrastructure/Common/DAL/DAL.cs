using Dapper;
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

public class DAL : IDAL
{
    private readonly string _connectionString;

    public DAL(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrWhiteSpace(_connectionString))
            throw new Exception("DefaultConnection not found in appsettings.json");
    }
    public async Task<int> ExecuteQuery(
            string query,
            CommandType commandType,
            DynamicParameters parameters,
            int timeout,
            string description,
            CancellationToken cancellationToken
        )
    {
        using var con = new SqlConnection(_connectionString);
        await con.OpenAsync(cancellationToken);

        return await con.ExecuteAsync(
            query,
            parameters,
            commandType: commandType,
            commandTimeout: timeout
        );
    }

public async Task<int> RunMultipleQueryAsync(
        string query,
        CommandType commandType,
        string connectionString,
        DynamicParameters parameters,
        int timeout,
        string description,
        CancellationToken cancellationToken
    )
    {
        using var con = new SqlConnection(connectionString);
        await con.OpenAsync(cancellationToken);

        var result = await con.ExecuteAsync(query, parameters, commandType: commandType);
        return result;
    }

    public async Task<object> GetScalarData(
        string query,
        CommandType commandType,
        string connectionString,
        DynamicParameters parameters,
        int timeout,
        string description,
        CancellationToken cancellationToken
    )
    {
        using var con = new SqlConnection(connectionString);
        await con.OpenAsync(cancellationToken);

        var result = await con.ExecuteScalarAsync(query, parameters, commandType: commandType);
        return result;
    }


    public async Task<string> GetScalarStringData(
        string query,
        CommandType commandType,
        string connectionString,
        DynamicParameters parameters,
        int timeout,
        string description,
        CancellationToken cancellationToken
    )
    {
        using var con = new SqlConnection(connectionString);
        await con.OpenAsync(cancellationToken);

        var result = await con.ExecuteScalarAsync(query, parameters, commandType: commandType);
        return result?.ToString();
    }

    public int SQLBulkUpload(
        DataTable dt,
        List<string> columnMappings,
        string tableName,
        DynamicParameters parameters,
        int timeout
    )
    {
        using var con = new SqlConnection(_connectionString);
        con.Open();

        using var bulkCopy = new SqlBulkCopy(con)
        {
            DestinationTableName = tableName,
            BulkCopyTimeout = timeout
        };

        foreach (var col in columnMappings)
        {
            bulkCopy.ColumnMappings.Add(col, col);
        }

        bulkCopy.WriteToServer(dt);
        return dt.Rows.Count;
    }

    public async Task<object> GetAggregatorScalarData(
        string query,
        List<string> columnMappings,
        CommandType commandType,
        DynamicParameters parameters,
        int timeout,
        string connectionString,
        CancellationToken cancellationToken
    )
    {
        using var con = new SqlConnection(connectionString);
        await con.OpenAsync(cancellationToken);

        var result = await con.ExecuteScalarAsync(query, parameters, commandType: commandType);
        return result;
    }
    public async Task<IEnumerable<T>> GetTableData<T>(
    string query,
    CommandType commandType,
    string connectionString,
    DynamicParameters parameters,
    int timeout,
    string description,
    CancellationToken cancellationToken
)
    {
        using var connection = new SqlConnection(connectionString);

        return await connection.QueryAsync<T>(
            query,
            parameters,
            commandType: commandType,
            commandTimeout: timeout
        );
    }

}
