namespace OTS.Data.DataAccess
{
    public interface ISqlDataAccess
    {
        Task<IEnumerable<T>> GetData<T, U>(string sql, U parameters, string connectionId = "connection");
        Task SaveData<T>(string sql, T parameters, string connectionId = "connection");
    }
}