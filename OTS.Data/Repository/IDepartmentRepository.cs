using Dapper;
using OTS.Data.Models;

namespace OTS.Data.Repository
{
    public interface IDepartmentRepository
    {
        Task<bool> AddAsync(DynamicParameters cost);
        Task<IEnumerable<DepartmentModel>> getdatasync();
        Task<DepartmentModel?> getdataByIdsync(int id);
        Task<bool> DeleteAsync(int id);
    }
}