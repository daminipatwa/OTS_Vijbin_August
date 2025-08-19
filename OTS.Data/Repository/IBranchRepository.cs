using Dapper;
using OTS.Data.Models;

namespace OTS.Data.Repository
{
    public interface IBranchRepository
    {
        Task<bool> AddAsync(DynamicParameters cost);
        Task<IEnumerable<BranchModel>> getdatasync();
        Task<BranchModel?> getdataByIdsync(int id);
        Task<bool> DeleteAsync(int id);
    }
}