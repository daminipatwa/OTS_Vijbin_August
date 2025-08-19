using Dapper;
using OTS.Data.Models;

namespace OTS.Data.Repository
{
    public interface ICostRepository
    {
        Task<bool> AddAsync(DynamicParameters cost);
        Task<IEnumerable<CostCenter>> getdatasync();
        Task<CostCenter?> getdataByIdsync(int id);

        Task<bool> DeleteAsync(int id);
    }
}