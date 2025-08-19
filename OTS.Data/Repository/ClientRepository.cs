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
    public class ClientRepository: IClientRepository
    {
        private readonly ISqlDataAccess _db;

        public ClientRepository(ISqlDataAccess db)
        {
            _db = db;
        }

        public async Task<bool> AddAsync(DynamicParameters cost)
        {
            try
            {
                await _db.SaveData("sp_instupd_client", cost);
                return true;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<IEnumerable<ClientModel>> getdatasync(int id=0)
        {
            return await _db.GetData<ClientModel, dynamic>("sp_get_client", new {id=id });

        }

        public async Task<ClientModel?> getdataByIdsync(int id)
        {
            IEnumerable<ClientModel> result = await _db.GetData<ClientModel, dynamic>("sp_get_client", new { Id = id });
            return result.FirstOrDefault();

        }

        public async Task<bool> DeleteAsync(int id)
        {
            await _db.SaveData("sp_del_client", new { Id = id });
            return true;

        }
        public async Task<bool> update_notification(int id)
        {
            await _db.SaveData("SP_Update_Notification", new { Id = id });
            return true;

        }
        public async Task<IEnumerable<NotificationModel>> getnotoficationsync(int id,string type)
        {
            return await _db.GetData<NotificationModel, dynamic>("Sp_Get_Notification", new { id = id, type=type });

        }
    }
}
