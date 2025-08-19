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
    public class BranchRepository: IBranchRepository
    {
        private readonly ISqlDataAccess _db;

        public BranchRepository(ISqlDataAccess db)
        {
            _db = db;
        }

        public async Task<bool> AddAsync(DynamicParameters cost)
        {
            try
            {
                await _db.SaveData("sp_instupd_branch", cost);
                return true;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<IEnumerable<BranchModel>> getdatasync()
        {
            return await _db.GetData<BranchModel, dynamic>("sp_get_branch", new { });

        }

        public async Task<BranchModel?> getdataByIdsync(int id)
        {
            IEnumerable<BranchModel> result = await _db.GetData<BranchModel, dynamic>("sp_get_branch", new { Id = id });
            return result.FirstOrDefault();

        }

        public async Task<bool> DeleteAsync(int id)
        {
            await _db.SaveData("sp_del_branch", new { Id = id });
            return true;

        }
    }
}
