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
    public class ProductRepository: Iproductrepository
    {
        private readonly ISqlDataAccess _db;

        public ProductRepository(ISqlDataAccess db)
        {
            _db = db;
        }

        public async Task<bool> AddAsync(DynamicParameters cost)
        {
            try
            {
                await _db.SaveData("SP_SaveProduct", cost);
                return true;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<IEnumerable<ProductModel>> getdatasync()
        {
            return await _db.GetData<ProductModel, dynamic>("sp_get_branch", new { });

        }

        public async Task<IEnumerable<ProductModel>> getdataBycategorysync(string category,int row,int userid=0,int clientid=0)
        {
            return  await _db.GetData<ProductModel, dynamic>("sp_get_product", new { category = category,row=row,id=0,userid=userid,clientid=clientid });
             
        }

        public async Task<IEnumerable<ProductModel>> getdataUploadedProduct()
        {
            return await _db.GetData<ProductModel, dynamic>("sp_get_product", new {id = -1});

        }

        public async Task<ProductModel?> getdataByIdsync(int id, string category, int row, int userid = 0, int clientid = 0)
        {
            IEnumerable<ProductModel> result = await _db.GetData<ProductModel, dynamic>("sp_get_product", new { Id = id, category = category, row = row,  userid = userid, clientid = clientid });
            return result.FirstOrDefault();

        }

        public async Task<bool> DeleteAsync(int id)
        {
            await _db.SaveData("sp_del_branch", new { Id = id });
            return true;

        }

        public async Task<bool> DeleteProduct(int id,int clientid)
        {
            await _db.SaveData("SP_Delete_product", new { Id = id, clientid= clientid });
            return true;

        }
    }
}
