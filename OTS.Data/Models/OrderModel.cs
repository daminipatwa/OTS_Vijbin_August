using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OTS.Data.Models
{
    public class OrderModel
    {
        public int id { get; set; }
        public string orderid { get; set; }
        public int clientid { get; set; }
        public int userid { get; set; }
        public string category { get; set; }
        public string Description { get; set; }
        public string name { get; set; }
        public int total_item { get; set; }
        public string total_amount { get; set; }
        public int status { get; set; }
        public int createdby { get; set; }
        public int updatedby { get; set; }
        public int srno { get; set; }
        public int qty { get; set; }
        public string orderdate { get; set; }
        public string username { get; set; }

        public string gst { get; set; }

        public decimal gst_amt { get; set; }
        public string hsn { get; set; }
        public string rate { get; set; }

        public string status_name { get; set; }

        public string branchname { get; set; }
        public string approvel_1 { get; set; }
        public string approvel_2 { get; set; }
        public string approvel_3 { get; set; }
        public int updated_qty { get; set; }
    }
    public class order_list_model
    {
        public int id { get; set; }
        public string shippingaddress2 { get; set; }
        public int userid { get; set; }
        public string orderid { get; set; }
        public string approvstatus { get; set; }
        public string qty { get; set; }
        public string department { get; set; }
        public string status { get; set; }
        public string username { get; set; }
        public string amount { get; set; }
        public string del_date { get; set; }
        public string orderdate { get; set; }
        public string billing_address { get; set; }
        public string shipping_address { get; set; }
        public string remark { get; set; }
        public string category { get; set; }
        public string clientname { get; set; }
        public string gst { get; set; }
        public string approvel_1 { get; set; }
        public string approvel_2 { get; set; }
        public string approvel_3 { get; set; }
        public string approvel_4 { get; set; }

        public int approve_1 { get; set; }
        public int approve_2 { get; set; }
        public int approve_3 { get; set; }
        public int approve_4 { get; set; }

        public int is_reject {  get; set; }
        public int is_challan { get; set; }
    }

    public class order_details
    {
        public int id { get; set; }
        public int orderid { get; set; }
        public int status { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public string category { get; set; }
        public string hsncode { get; set; }
        public string rate { get; set; }
        public string remark { get; set; }
        public string qty { get; set; }
        public string total_amount { get; set; }
        public int is_reject { get; set; }
        public string updated_qty { get; set; }
        public string updated_amount { get; set; }
        public string orderno { get; set; }
        public string order_date { get; set; }
        public string username { get; set; }
        public string client { get; set; }
        public decimal? gst_amt { get; set; }
        public decimal? gst_per { get; set; }
    }

    public class Order_department { 
    public string clientname { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string orderid { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string gst { get; set; }
        public string HSN { get; set; }
        public string qty { get; set; }
        public string Rate { get; set; }
        public string orderdate { get; set; }
        public string department { get; set; }
    }

    public class order_combo { 
    
    public order_list_model order_List_Model { get; set; }
        public List<order_details> order_list { get; set; }
        public ShippingModel shipping { get; set; }
    }
}
