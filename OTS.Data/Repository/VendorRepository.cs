using Dapper;
using OTS.Data.DataAccess;
using OTS.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OTS.Data.Repository
{
    public class VendorRepository : IVendorRepository
    {
        private readonly ISqlDataAccess _db;

        public VendorRepository(ISqlDataAccess db)
        {
            _db = db;
        }

        public async Task<bool> AddAsync(DynamicParameters cost)
        {
            try
            {
                await _db.SaveData("SP_insup_Vendor", cost);
                return true;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<IEnumerable<VendorModel>> getdatasync()
        {
            return await _db.GetData<VendorModel, dynamic>("SP_Get_Vendor", new { });

        }

        public async Task<VendorModel?> getdataByIdsync(int id)
        {
            IEnumerable<VendorModel> result = await _db.GetData<VendorModel, dynamic>("SP_Get_Vendor", new { Id = id });
            return result.FirstOrDefault();

        }

        public async Task<bool> DeleteAsync(int id)
        {
            await _db.SaveData("Sp_Delete_Vendor", new { Id = id });
            return true;

        }

        public async Task<bool> Addfrenchises(DynamicParameters cost)
        {
            try
            {
                await _db.SaveData("sp_instupd_frenchises", cost);
                return true;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<IEnumerable<Frenchises_Model>> getfrenchises(int id)
        {
            return await _db.GetData<Frenchises_Model, dynamic>("Sp_get_frenchises", new { id = id });

        }
        public async Task<bool> Addagreement(DynamicParameters cost)
        {
            try
            {
                await _db.SaveData("SP_InstUp_Agreement", cost);
                return true;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<IEnumerable<AgreementModel>> getagreement(int id)
        {
            return await _db.GetData<AgreementModel, dynamic>("SP_Get_Agreement", new {id=id });

        }

        public async Task<bool> AddShipping(DynamicParameters cost)
        {
            try
            {
                await _db.SaveData("sp_InstUp_Shipping", cost);
                return true;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<IEnumerable<SupplierDashboardModel>> getsuppdash()
        {
            return await _db.GetData<SupplierDashboardModel, dynamic>("Sp_Get_Supplier_Dashboard", new { });

        }
        public async Task<IEnumerable<SupplierDashboardModel>> getclientdash(int id)
        {
            return await _db.GetData<SupplierDashboardModel, dynamic>("Sp_Get_client_Dashboard", new {id=id });

        }

        public async Task<bool> update_invoice(DynamicParameters cost)
        {
            try
            {
                await _db.SaveData("SP_UPdate_Invoice", cost);
                return true;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<bool> updatestock(DynamicParameters cost)
        {
            try
            {
                await _db.SaveData("sp_update_stock", cost);
                return true;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
