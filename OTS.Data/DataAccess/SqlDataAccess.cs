using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace OTS.Data.DataAccess;

public class SqlDataAccess: ISqlDataAccess
{
    private readonly IConfiguration _config;

    public SqlDataAccess(IConfiguration config)
    {
        _config = config;
    }

    public async Task<IEnumerable<T>> GetData<T, U>(string sql, U parameters,string connectionId = "connection")
    {
        using IDbConnection connection = new SqlConnection(_config.GetConnectionString(connectionId));
        return await connection.QueryAsync<T>(sql, parameters, commandType: CommandType.StoredProcedure);
    }

    public async Task SaveData<T>(string sql, T parameters, string connectionId = "connection")
    {
        using IDbConnection connection = new SqlConnection(_config.GetConnectionString(connectionId));
        await connection.ExecuteAsync(sql, parameters, commandType: CommandType.StoredProcedure);
    }
}
