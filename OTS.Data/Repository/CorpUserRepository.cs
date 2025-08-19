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
    public class CorpUserRepository : ICorpRepository
    {
        private readonly ISqlDataAccess _db;

        public CorpUserRepository(ISqlDataAccess db)
        {
            _db = db;
        }

        public async Task<bool> AddAsync(DynamicParameters cost)
        {
            try
            {
                await _db.SaveData("SP_insup_CorpUser", cost);
                return true;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<IEnumerable<CorpUserModel>> getdatasync(int clientid=0)
        {
            return await _db.GetData<CorpUserModel, dynamic>("sp_get_CorpUser", new {clientid=clientid });

        }

        public async Task<IEnumerable<UserTypeModel>> getusertype()
        {
            return await _db.GetData<UserTypeModel, dynamic>("sp_get_SuperUser", new { });

        }

        public async Task<CorpUserModel?> getdataByIdsync(int id)
        {
            IEnumerable<CorpUserModel> result = await _db.GetData<CorpUserModel, dynamic>("sp_get_CorpUser", new { Id = id });
            return result.FirstOrDefault();

        }

        public async Task<ShippingModel> getShipping(int id)
        {
           IEnumerable<ShippingModel> result = await _db.GetData<ShippingModel, dynamic>("sp_Get_Shipping", new { orderid = id });
            return result.FirstOrDefault();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            await _db.SaveData("Sp_Delete_CorpUser", new { Id = id });
            return true;

        }

        public async Task<bool> Addfeedback(DynamicParameters cost)
        {
            try
            {
                await _db.SaveData("SP_INST_Feedback", cost);
                return true;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<bool> single_reject_challan(DynamicParameters cost)
        {
            try
            {
                await _db.SaveData("sp_single_challan_Reject", cost);
                return true;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<bool> All_reject_Accept_challan(DynamicParameters cost)
        {
            try
            {
                await _db.SaveData("sp_All_challan_Accept_Reject", cost);
                return true;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<IEnumerable<challan_count>> get_challan_count(int id = 0)
        {
            return await _db.GetData<challan_count, dynamic>("sp_get_challan_count", new { id = id });


        }
    }
}
