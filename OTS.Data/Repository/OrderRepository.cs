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
    public class OrderRepository : IOrderRepository
    {
        private readonly ISqlDataAccess _db;
        public OrderRepository(ISqlDataAccess db)
        {
            _db = db;
        }

        public async Task<bool> AddAsync(DynamicParameters cost)
        {
            try
            {
                await _db.SaveData("sp_order", cost);
                return true;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<bool> Addorder_detailsAsync(DynamicParameters cost)
        {
            try
            {
                await _db.SaveData("sp_order_details", cost);
                return true;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<bool> updateorder_detailsAsync(DynamicParameters cost)
        {
            try
            {
                await _db.SaveData("sp_update_totalqty", cost);
                return true;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<IEnumerable<OrderModel>> getsrnosync()
        {
            return await _db.GetData<OrderModel, dynamic>("sp_get_srno", new { });

        }
        public async Task<IEnumerable<order_list_model>> getorderlist(int status,int id=0,string type="",string usertype="",int userid=0,string f="",string t="",string order = "")
        {
            if (f == "")
            {
                f = DateTime.Now.AddDays(-3650).ToString("yyyy-MM-dd");
            }
            if (t == "")
            {
                t = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd");
            }
            return await _db.GetData<order_list_model, dynamic>("sp_orderlist", new { status = status,id=id,type=type, usertype= usertype,userid=userid,fromdate= f,todate=t,order=order });

        }
        public async Task<IEnumerable<order_list_model>> getorderlistforvendor(int status,string type="",int userid=0,string f="",string t="")
        {
            if (f == "")
            {
                f = DateTime.Now.AddDays(-3650).ToString("yyyy-MM-dd");
            }
            if (t == "")
            {
                t = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd");
            }
            return await _db.GetData<order_list_model, dynamic>("sp_orderlist", new {status= status,type= type, id=0,userid=0,fromdate=f,todate=t });

        }

        public async Task<IEnumerable<order_list_model>> getinvoicelistforvendor(int type)
        {
            return await _db.GetData<order_list_model, dynamic>("get_approved_invoice", new { type = type});

        }

        public async Task<IEnumerable<order_details>> get_order_details(int id)
        {
            return await _db.GetData<order_details, dynamic>("get_order_details", new { id = id });

        }
        public async Task<IEnumerable<OrderModel>> getorderbyproduct(int userid=0,string f="", string t="")
        {
            return await _db.GetData<OrderModel, dynamic>("Sp_GetorderbyProduct", new {userid=userid,fromdate=f,todate=t });

        }
        public async Task<IEnumerable<Order_department>> getorderbydepartment(string f="",string t="")
        {
            return await _db.GetData<Order_department, dynamic>("SP_GetOrderDepartmentwise", new {fromdate=f,todate=t });

        }
        public async Task<IEnumerable<Order_department>> getorderbybranch()
        {
            return await _db.GetData<Order_department, dynamic>("SP_GetOrderBranchwise", new { });

        }
        public async Task<bool> updateqty(DynamicParameters cost)
        {
            try
            {
                await _db.SaveData("sp_order_details", cost);
                return true;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<bool> Addorder_visiting_detailsAsync(DynamicParameters cost)
        {
            try
            {
                await _db.SaveData("sp_visiting_order_details", cost);
                return true;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<bool> Addorder_printing_detailsAsync(DynamicParameters cost)
        {
            try
            {
                await _db.SaveData("sp_printing_order_details", cost);
                return true;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<IEnumerable<OrderModel>> get_User_order_list(int id, int clientid, string type)
        {
            return await _db.GetData<OrderModel, dynamic>("SP_Get_userOrder", new { id = id, clientid = @clientid, type = type });

        }

        public async Task<IEnumerable<OrderModel>> getorderbyid(int id)
        {
            return await _db.GetData<OrderModel, dynamic>("sp_get_order_byid", new { id = id});
        }

        public async Task<IEnumerable<VisitingCardModel>> get_User_Visitingorder_list(int id)
        {
            return await _db.GetData<VisitingCardModel, dynamic>("SP_Get_Visiting_Details", new { id = id});

        }

        public async Task<bool> updatestatus(DynamicParameters cost)
        {
            try
            {
                await _db.SaveData("sp_accept_order", cost);
                return true;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<bool> Delete_stationaryAsync(int id)
        {
            await _db.SaveData("SP_Delete_order_stationary", new { Id = id });
            return true;

        }

        public async Task<bool> Delete_Orderdetails_stationaryAsync(int id)
        {
            await _db.SaveData("SP_Delete_orderdetails_stationary", new { Id = id });
            return true;

        }
    }
}
