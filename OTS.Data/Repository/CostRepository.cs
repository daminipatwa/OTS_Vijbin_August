using Dapper;
using OTS.Data.DataAccess;
using OTS.Data.Models;
using System.Reflection.Metadata.Ecma335;
namespace OTS.Data.Repository;

public class CostRepository : ICostRepository
{
    private readonly ISqlDataAccess _db;

    public CostRepository(ISqlDataAccess db)
    {
        _db = db;
    }

    public async Task<bool> AddAsync(DynamicParameters cost)
    {
        try
        {
            await _db.SaveData("sp_instupd_costcenter", cost);
            return true;
        }
        catch (Exception)
        {

            throw;
        }
    }

    public async Task<IEnumerable<CostCenter>> getdatasync()
    {
        return await _db.GetData<CostCenter, dynamic>("sp_get_costcenter", new { });

    }

    public async Task<CostCenter?> getdataByIdsync(int id)
    {
        IEnumerable<CostCenter> result = await _db.GetData<CostCenter, dynamic>("sp_get_costcenter", new { Id = id });
        return result.FirstOrDefault();

    }

    public async Task<bool> DeleteAsync(int id)
    { 
        await _db.SaveData("sp_del_costcenter", new { Id = id });
        return true;
   
    }
}
