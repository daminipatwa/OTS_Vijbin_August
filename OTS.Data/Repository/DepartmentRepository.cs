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
    public class DepartmentRepository: IDepartmentRepository
    {
        private readonly ISqlDataAccess _db;

        public DepartmentRepository(ISqlDataAccess db)
        {
            _db = db;
        }

        public async Task<bool> AddAsync(DynamicParameters cost)
        {
            try
            {
                await _db.SaveData("sp_instupd_department", cost);
                return true;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<IEnumerable<DepartmentModel>> getdatasync()
        {
            return await _db.GetData<DepartmentModel, dynamic>("sp_get_department", new { });

        }

        public async Task<DepartmentModel?> getdataByIdsync(int id)
        {
            IEnumerable<DepartmentModel> result = await _db.GetData<DepartmentModel, dynamic>("sp_get_department", new { Id = id });
            return result.FirstOrDefault();

        }

        public async Task<bool> DeleteAsync(int id)
        {
            await _db.SaveData("sp_del_department", new { Id = id });
            return true;

        }
    }
}
