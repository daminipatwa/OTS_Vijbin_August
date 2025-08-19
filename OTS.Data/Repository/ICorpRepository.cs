using Dapper;
using OTS.Data.Models;

namespace OTS.Data.Repository
{
    public interface ICorpRepository
    {
        Task<bool> AddAsync(DynamicParameters cost);
        Task<IEnumerable<CorpUserModel>> getdatasync(int clientid=0);
        Task<CorpUserModel?> getdataByIdsync(int id);
        Task<IEnumerable<UserTypeModel>> getusertype();
        Task<bool> DeleteAsync(int id);
        Task<bool> Addfeedback(DynamicParameters cost);
        Task<bool> single_reject_challan(DynamicParameters cost);
        Task<IEnumerable<challan_count>> get_challan_count(int id = 0);
        Task<bool> All_reject_Accept_challan(DynamicParameters cost);
        Task<ShippingModel> getShipping(int id);
    }
}