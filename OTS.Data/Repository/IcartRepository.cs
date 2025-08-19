using Dapper;
using OTS.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OTS.Data.Repository
{
    public interface IcartRepository
    {
        bool Add_cart(DynamicParameters cost);
        Task<IEnumerable<CartModel>> getdatasync(int userid, int clientid);
        Task<bool> DeleteAsync(int id);
        bool Add_wishlist(DynamicParameters cost);
        Task<IEnumerable<CartModel>> getwishlist(int userid, int clientid);
        Task<bool> Deletewishlistc(int id);
    }
}
