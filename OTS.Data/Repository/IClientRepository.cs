using Dapper;
using OTS.Data.Models;

namespace OTS.Data.Repository
{
    public interface IClientRepository
    {
        Task<bool> AddAsync(DynamicParameters cost);
        Task<IEnumerable<ClientModel>> getdatasync(int id=0);
        Task<ClientModel?> getdataByIdsync(int id);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<NotificationModel>> getnotoficationsync(int id, string type);
        Task<bool> update_notification(int id);
    }
}