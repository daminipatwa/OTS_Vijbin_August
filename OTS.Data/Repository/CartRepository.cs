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
    public class CartRepository:IcartRepository
    {
        private readonly ISqlDataAccess _db;
        public CartRepository(ISqlDataAccess db)
        {
            _db = db;
        }

        public bool Add_cart(DynamicParameters cost)
        {
            try
            {
                 _db.SaveData("sp_instupd_cart", cost);
                return true;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<IEnumerable<CartModel>> getdatasync(int userid,int clientid)
        {
            return await _db.GetData<CartModel, dynamic>("sp_getcartlist", new {userid= userid,clientid= clientid });

        }

        public async Task<bool> DeleteAsync(int id)
        {
            await _db.SaveData("sp_delete_cart", new { Id = id });
            return true;

        }

        public bool Add_wishlist(DynamicParameters cost)
        {
            try
            {
                _db.SaveData("sp_instupd_wishlist", cost);
                return true;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<IEnumerable<CartModel>> getwishlist(int userid, int clientid)
        {
            return await _db.GetData<CartModel, dynamic>("sp_getwishlist", new { userid = userid, clientid = clientid });

        }

        public async Task<bool> Deletewishlistc(int id)
        {
            await _db.SaveData("sp_delete_wishlist", new { Id = id });
            return true;

        }
    }
}
