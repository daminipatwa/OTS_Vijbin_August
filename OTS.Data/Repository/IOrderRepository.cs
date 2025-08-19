using Dapper;
using OTS.Data.Models;

namespace OTS.Data.Repository
{
    public interface IOrderRepository
    {
        Task<bool> AddAsync(DynamicParameters cost);
        Task<IEnumerable<OrderModel>> getsrnosync();
        Task<bool> Addorder_detailsAsync(DynamicParameters cost);
        Task<IEnumerable<order_list_model>> getorderlist(int status, int id = 0, string type = "", string usertype = "", int userid = 0,string f="", string t="",string order="");
        Task<IEnumerable<order_list_model>> getorderlistforvendor(int status, string type = "", int userid = 0, string f = "", string t = "");
        Task<IEnumerable<order_details>> get_order_details(int id);
        Task<bool> updateqty(DynamicParameters cost);
        Task<bool> Addorder_visiting_detailsAsync(DynamicParameters cost);
        Task<bool> Addorder_printing_detailsAsync(DynamicParameters cost);
        Task<IEnumerable<OrderModel>> get_User_order_list(int id, int clientid, string type);
        Task<IEnumerable<VisitingCardModel>> get_User_Visitingorder_list(int id);
        Task<IEnumerable<OrderModel>> getorderbyproduct(int userid = 0,string f="", string t="");
        Task<IEnumerable<Order_department>> getorderbydepartment(string f = "", string t = "");
        Task<IEnumerable<Order_department>> getorderbybranch();
        Task<bool> updatestatus(DynamicParameters cost);
        Task<IEnumerable<order_list_model>> getinvoicelistforvendor(int type);
        Task<bool> Delete_stationaryAsync(int id);
        Task<bool> Delete_Orderdetails_stationaryAsync(int id);

        Task<bool> updateorder_detailsAsync(DynamicParameters cost);

        Task<IEnumerable<OrderModel>> getorderbyid(int id);
    }
}