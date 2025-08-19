using Dapper;
using OTS.Data.Models;

namespace OTS.Data.Repository
{
    public interface IVendorRepository
    {
        Task<bool> AddAsync(DynamicParameters cost);
        Task<IEnumerable<VendorModel>> getdatasync();
        Task<VendorModel?> getdataByIdsync(int id);
        Task<bool> DeleteAsync(int id);
        Task<bool> Addfrenchises(DynamicParameters cost);
        Task<bool> Addagreement(DynamicParameters cost);
        Task<IEnumerable<AgreementModel>> getagreement(int id);
        Task<IEnumerable<Frenchises_Model>> getfrenchises(int id);
        Task<bool> AddShipping(DynamicParameters cost);

        Task<IEnumerable<SupplierDashboardModel>> getsuppdash();
        Task<IEnumerable<SupplierDashboardModel>> getclientdash(int id);
        Task<bool> update_invoice(DynamicParameters cost);
        Task<bool> updatestock(DynamicParameters cost);
    }
}