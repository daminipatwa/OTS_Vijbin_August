using Dapper;
using OTS.Data.Models;

namespace OTS.Data.Repository
{
    public interface Iproductrepository
    {
        Task<bool> AddAsync(DynamicParameters cost);
        Task<IEnumerable<ProductModel>> getdataBycategorysync(string category, int row, int userid = 0, int clientid = 0);
        Task<ProductModel?> getdataByIdsync(int id, string category, int row, int userid = 0, int clientid = 0);
        Task<bool> DeleteProduct(int id, int clientid);
        Task<IEnumerable<ProductModel>> getdataUploadedProduct();
    }
}