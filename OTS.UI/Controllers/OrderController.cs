using ClosedXML.Excel;
using Dapper;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using MailKit.Search;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Hosting;
using OTS.Data.Models;
using OTS.Data.Repository;
using System.Data;

namespace OTS.UI.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IcartRepository _cartrepository;
        private readonly ICorpRepository _corpRepository;
        private readonly ISendMailRepositery _sendMail;

        public OrderController(IOrderRepository orderRepository,IcartRepository cartrepository,ICorpRepository corpRepository,ISendMailRepositery sendMail)
        {
            _orderRepository = orderRepository;
            _cartrepository = cartrepository;
            _corpRepository = corpRepository;
            _sendMail = sendMail;
        }
        public async Task<ActionResult> AllOrder()
        {
            var result = await _orderRepository.getorderlist(1);
            return View(result);
        }

        public ActionResult ApprovedOrder()
        {
            return View();
        }
        public ActionResult RejectOrder()
        {
            return View();
        }
        public ActionResult PendingOrder()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> saveorder(string remark, string shipping_address, string billing_address, string shippingaddress2) {
            try
            {
                
                    var newString = "";
                    int srno = 0;
                    var result = await _orderRepository.getsrnosync();
                    foreach (var itm in result)
                    {
                        if (itm.srno == 0)
                        {
                             newString ="1".PadLeft(6, '0');
                            srno = 1;
                        }
                        else {
                            newString = (itm.srno+1).ToString().PadLeft(6, '0');
                            srno = itm.srno + 1;
                        }


                    }
                    int totalqty = 0;
                    decimal total_price = 0;
                     

                CorpUserModel user = new CorpUserModel();
                CorpUserModel user1 = new CorpUserModel();
                user = await _corpRepository.getdataByIdsync(Convert.ToInt32(Request.Cookies["userid"].ToString()));

                user1 = await _corpRepository.getdataByIdsync(Convert.ToInt32(user.approveby));

                var result1 = await _cartrepository.getdatasync(Convert.ToInt32(Request.Cookies["userid"].ToString()), Convert.ToInt32(Request.Cookies["clientid"].ToString()));
                    foreach (var item in result1)
                    {
                    decimal gst = 0;
                    totalqty = totalqty + Convert.ToInt32(item.qty);
                    gst = (Convert.ToDecimal(item.rate) * Convert.ToInt32(item.qty)) * Convert.ToDecimal(item.gst) / 100;
                     total_price = total_price + (Convert.ToDecimal(item.rate) * Convert.ToInt32(item.qty))+gst;
                    }

                    //var newString = result.PadLeft(4, '0');
                    DynamicParameters ObjParm = new DynamicParameters();
                    ObjParm.Add("@id", "0");
                    ObjParm.Add("@orderid", "O"+ newString);
                    ObjParm.Add("@clientid", Convert.ToInt32(Request.Cookies["clientid"].ToString()));
                    ObjParm.Add("@userid", Convert.ToInt32(Request.Cookies["userid"].ToString()));
                    ObjParm.Add("@category","Stationery");
                    ObjParm.Add("@total_amount", total_price);
                    ObjParm.Add("@total_item", totalqty);
                    ObjParm.Add("@status", "1");
                    ObjParm.Add("@remark", remark);
                    ObjParm.Add("@shipping_address", shipping_address);
                    ObjParm.Add("@billing_address", billing_address);
                    ObjParm.Add("@shippingaddress2", shippingaddress2);
                    ObjParm.Add("@srno", srno);
                    ObjParm.Add("@approval_for", user.approveby);
                    ObjParm.Add("@createdby", Convert.ToInt32(Request.Cookies["userid"].ToString()));
                   
                    ObjParm.Add("@result", dbType: DbType.Int32, direction: ParameterDirection.Output, size: 5215585);

                    await _orderRepository.AddAsync(ObjParm);
                    var id = ObjParm.Get<dynamic>("@result");
                    if (id > 0)
                    {
                        if (result1.Count() > 0)
                        {
                            foreach (var item in result1)
                        {
                            DynamicParameters ObjParm1 = new DynamicParameters();
                            ObjParm1.Add("@id", "0");
                            ObjParm1.Add("@orderid", id);
                            ObjParm1.Add("@productid", item.productid);
                            ObjParm1.Add("@rate", item.rate);
                            ObjParm1.Add("@qty", item.qty);
                            ObjParm1.Add("@gst_per", item.gst);
                            ObjParm1.Add("@gst", (Convert.ToDecimal(item.rate) * Convert.ToInt32(item.qty)) * Convert.ToDecimal(item.gst) / 100);
                            ObjParm1.Add("@total_amount", (Convert.ToDecimal(item.rate) * Convert.ToInt32(item.qty))+ (Convert.ToDecimal(item.rate) * Convert.ToInt32(item.qty)) * Convert.ToDecimal(item.gst) / 100);

                             ObjParm1.Add("@createdby", Convert.ToInt32(Request.Cookies["userid"].ToString()));
                            
                            await _orderRepository.Addorder_detailsAsync(ObjParm1);

                            await _cartrepository.DeleteAsync(item.Id);
                        }
                            TempData["MsgType"] = "Success";

                            TempData["Msg"] = "Order Placed Successfully";

                        var result2 = await _corpRepository.getdataByIdsync(Convert.ToInt32(user.approveby));

                        StreamReader sr = new StreamReader("Challan/order_summary.html");
                        string s = sr.ReadToEnd();
                        s = s.Replace("{adminname}", result2.firstname + " " + result2.lastname).Replace("{clientname}", user.client_name).Replace("{orderno}", "O" + newString).Replace("{Username}", user.firstname + " " + user.lastname).Replace("{department}", user.department_name).Replace("{branch}", user.branch_name).Replace("{level}", user.approval_type.ToString()).Replace("{totalitem}", totalqty.ToString()).Replace("{totalprice}", "0").Replace("{remark}", "").Replace("{url}", "<a href=\"http://erp.vijbin.com/Order/update_order_statusByEmail?id=" + id + "&status="+ user1.approval_type + "&userid="+ Convert.ToInt32(Request.Cookies["userid"].ToString()) + "&clientid="+ user.approveby + "\" class=\"btn btn-success\">Accept</a> &nbsp;&nbsp;&nbsp;  <a href=\"\" class=\"btn btn-danger\">Reject</a>");
                        string tblitem = "";
                        foreach (var item in result1)
                        {
                            tblitem += "<tr><td><img src='http://erp.vijbin.com/uploads/" + item.ImageUrl+"' style='width:100%'/></td><td>"+item.Description+"</td><td>"+item.qty+"</td><td>0</td><td>0</td></tr>";
                        }
                        s = s.Replace("{item}", tblitem);

                        string msg = s;

                        _sendMail.Send_Mail(result2.emailid, "Order Approval Required for Order No. O" + newString + "", msg);
                       

                    }

                    }
                    else
                    {
                        TempData["MsgType"] = "Error";
                        TempData["Msg"] = "Something went wrong";
                    }
                
                return Redirect("../User/AllOrder");
            }
            catch (Exception ex)
            {
                TempData["MsgType"] = "Error";
                TempData["Msg"] = ex.Message;
                return Redirect("../User/AllOrder");
            }
        }
        [HttpPost]
        public async Task<ActionResult> saveorder_client(string remark, string shipping_address, string billing_address, string shippingaddress2)
        {
            try
            {

                var newString = "";
                int srno = 0;
                var result = await _orderRepository.getsrnosync();
                foreach (var itm in result)
                {
                    if (itm.srno == 0)
                    {
                        newString = "1".PadLeft(6, '0');
                        srno = 1;
                    }
                    else
                    {
                        newString = (itm.srno + 1).ToString().PadLeft(6, '0');
                        srno = itm.srno + 1;
                    }


                }
                int totalqty = 0;
                decimal total_price = 0;


                CorpUserModel user = new CorpUserModel();
                user = await _corpRepository.getdataByIdsync(Convert.ToInt32(Request.Cookies["userid"].ToString()));
                var result1 = await _cartrepository.getdatasync(Convert.ToInt32(Request.Cookies["userid"].ToString()), Convert.ToInt32(Request.Cookies["clientid"].ToString()));
                foreach (var item in result1)
                {
                    decimal gst = 0;
                    totalqty = totalqty + Convert.ToInt32(item.qty);
                    gst = (Convert.ToDecimal(item.rate) * Convert.ToInt32(item.qty)) * Convert.ToDecimal(item.gst) / 100;
                    total_price = total_price + (Convert.ToDecimal(item.rate) * Convert.ToInt32(item.qty)) + gst;
                }

                //var newString = result.PadLeft(4, '0');
                DynamicParameters ObjParm = new DynamicParameters();
                ObjParm.Add("@id", "0");
                ObjParm.Add("@orderid", "O" + newString);
                ObjParm.Add("@clientid", Convert.ToInt32(Request.Cookies["clientid"].ToString()));
                ObjParm.Add("@userid", Convert.ToInt32(Request.Cookies["userid"].ToString()));
                ObjParm.Add("@category", "Stationery");
                ObjParm.Add("@remark", remark);
                ObjParm.Add("@shipping_address", shipping_address);
                ObjParm.Add("@billing_address", billing_address);
                ObjParm.Add("@shippingaddress2", shippingaddress2);
                ObjParm.Add("@total_amount", total_price);
                ObjParm.Add("@total_item", totalqty);
                ObjParm.Add("@status", "6");
                ObjParm.Add("@srno", srno);
                ObjParm.Add("@approval_for", user.approveby);
                ObjParm.Add("@createdby", Convert.ToInt32(Request.Cookies["userid"].ToString()));

                ObjParm.Add("@result", dbType: DbType.Int32, direction: ParameterDirection.Output, size: 5215585);

                await _orderRepository.AddAsync(ObjParm);
                var id = ObjParm.Get<dynamic>("@result");
                if (id > 0)
                {
                    if (result1.Count() > 0)
                    {
                        foreach (var item in result1)
                        {
                            DynamicParameters ObjParm1 = new DynamicParameters();
                            ObjParm1.Add("@id", "0");
                            ObjParm1.Add("@orderid", id);
                            ObjParm1.Add("@productid", item.productid);
                            ObjParm1.Add("@rate", item.rate);
                            ObjParm1.Add("@qty", item.qty);
                            ObjParm1.Add("@gst_per", item.gst);
                            ObjParm1.Add("@gst", (Convert.ToDecimal(item.rate) * Convert.ToInt32(item.qty)) * Convert.ToDecimal(item.gst) / 100);
                            ObjParm1.Add("@total_amount", (Convert.ToDecimal(item.rate) * Convert.ToInt32(item.qty)) + (Convert.ToDecimal(item.rate) * Convert.ToInt32(item.qty)) * Convert.ToDecimal(item.gst) / 100);

                            ObjParm1.Add("@createdby", Convert.ToInt32(Request.Cookies["userid"].ToString()));

                            await _orderRepository.Addorder_detailsAsync(ObjParm1);

                            await _cartrepository.DeleteAsync(item.Id);
                        }
                        TempData["MsgType"] = "Success";

                        TempData["Msg"] = "Order Placed Successfully";

                        var result2 = await _corpRepository.getdataByIdsync(Convert.ToInt32(user.approveby));

                        StreamReader sr = new StreamReader("Challan/order_summary.html");
                        string s = sr.ReadToEnd();
                        s = s.Replace("{adminname}", result2.firstname + " " + result2.lastname).Replace("{clientname}", user.client_name).Replace("{orderno}", "O" + newString).Replace("{Username}", user.firstname + " " + user.lastname).Replace("{department}", user.department_name).Replace("{branch}", user.branch_name).Replace("{level}", user.approval_type.ToString()).Replace("{totalitem}", totalqty.ToString()).Replace("{totalprice}", total_price.ToString()).Replace("{remark}", "").Replace("{url}", "http://erp.vijbin.com/supplier/allorder");
                        string tblitem = "";
                        foreach (var item in result1)
                        {
                            tblitem += "<tr><td><img src='http://erp.vijbin.com/uploads/" + item.ImageUrl + "'/></td><td>" + item.Description + "</td><td>" + item.qty + "</td><td>" + item.rate + "</td><td>" + (item.qty * item.rate) + "</td></tr>";
                        }
                        s = s.Replace("{item}", tblitem);

                        string msg = s;

                        _sendMail.Send_Mail(result2.emailid, "Order Approval Required for Order No. O" + newString + "", msg);


                    }

                }
                else
                {
                    TempData["MsgType"] = "Error";
                    TempData["Msg"] = "Something went wrong";
                }

                return Redirect("../Client/all_order");
            }
            catch (Exception ex)
            {
                TempData["MsgType"] = "Error";
                TempData["Msg"] = ex.Message;
                return View();
            }
        }

        [HttpPost]
        public async Task<ActionResult> saveorder_visiting(VisitingCardModel Model, IFormFile file)
        {

            try
            {
                CorpUserModel user = new CorpUserModel();
                user = await _corpRepository.getdataByIdsync(Convert.ToInt32(Request.Cookies["userid"].ToString()));
                var newString = "";
                int srno = 0;
                var result = await _orderRepository.getsrnosync();
                foreach (var itm in result)
                {
                    if (itm.srno == 0)
                    {
                        newString = "1".PadLeft(6, '0');
                        srno = 1;
                    }
                    else
                    {
                        newString = (itm.srno + 1).ToString().PadLeft(6, '0');
                        srno = itm.srno + 1;
                    }


                }
                int totalqty = 0;
                decimal total_price = 0;
                

                //var newString = result.PadLeft(4, '0');
                DynamicParameters ObjParm = new DynamicParameters();
                ObjParm.Add("@id", Model.id);
                ObjParm.Add("@orderid", "O" + newString);
                ObjParm.Add("@clientid", Convert.ToInt32(Request.Cookies["clientid"].ToString()));
                ObjParm.Add("@userid", Convert.ToInt32(Request.Cookies["userid"].ToString()));
                ObjParm.Add("@category", "Visiting Card");
                ObjParm.Add("@total_amount", "0");
                ObjParm.Add("@total_item", "0");
                ObjParm.Add("@status", "1");
                ObjParm.Add("@srno", srno);
                ObjParm.Add("@approval_for", user.approveby);
                ObjParm.Add("@createdby", Convert.ToInt32(Request.Cookies["userid"].ToString()));

                ObjParm.Add("@result", dbType: DbType.Int32, direction: ParameterDirection.Output, size: 5215585);

                await _orderRepository.AddAsync(ObjParm);
                var id = ObjParm.Get<dynamic>("@result");
                if (id > 0)
                {
                    if (file != null && file.Length > 0)
                    {
                        var productlist = ParseExcelFile(file.OpenReadStream());
                        foreach (var item in productlist)
                        {
                            DynamicParameters ObjParm1 = new DynamicParameters();
                            ObjParm1.Add("@id", "0");
                            ObjParm1.Add("@orderid", id);
                            ObjParm1.Add("@productid", Model.productid);
                            ObjParm1.Add("@company_name", item.company_name);
                            ObjParm1.Add("@first_name", item.first_name);
                            ObjParm1.Add("@last_name", item.last_name);
                            ObjParm1.Add("@contactno1", item.contactno1);
                            ObjParm1.Add("@contactno2", item.contactno2);
                            ObjParm1.Add("@emailid", item.emailid);
                            ObjParm1.Add("@designation", item.designation);
                            ObjParm1.Add("@address", item.address);
                            ObjParm1.Add("@qty", item.qty);
                            ObjParm1.Add("@createdby", Convert.ToInt32(Request.Cookies["userid"].ToString()));

                            await _orderRepository.Addorder_visiting_detailsAsync(ObjParm1);
                        }
                    }
                    else
                    {

                        DynamicParameters ObjParm1 = new DynamicParameters();
                        ObjParm1.Add("@id", "0");
                        ObjParm1.Add("@orderid", id);
                        ObjParm1.Add("@productid", Model.productid);
                        ObjParm1.Add("@company_name", Model.company_name);
                        ObjParm1.Add("@first_name", Model.first_name);
                        ObjParm1.Add("@last_name", Model.last_name);
                        ObjParm1.Add("@contactno1", Model.contactno1);
                        ObjParm1.Add("@contactno2", Model.contactno2);
                        ObjParm1.Add("@emailid", Model.emailid);
                        ObjParm1.Add("@designation", Model.designation);
                        ObjParm1.Add("@address", Model.address);
                        ObjParm1.Add("@qty", Model.qty);
                        ObjParm1.Add("@createdby", Convert.ToInt32(Request.Cookies["userid"].ToString()));

                        await _orderRepository.Addorder_visiting_detailsAsync(ObjParm1);
                    }
                    TempData["MsgType"] = "Success";

                    TempData["Msg"] = "Order Placed Successfully";

                }
                else
                {
                    TempData["MsgType"] = "Error";
                    TempData["Msg"] = "Something went wrong";
                }

                return RedirectToAction("AllOrder");
            }
            catch (Exception ex)
            {
                TempData["MsgType"] = "Error";
                TempData["Msg"] = ex.Message;
                return View();
            }
        }

        public List<VisitingCardModel> ParseExcelFile(Stream stream)
        {
            var employees = new List<VisitingCardModel>();
            //Create a workbook instance
            //Opens an existing workbook from a stream.
            using (var workbook = new XLWorkbook(stream))
            {
                //Lets assume the First Worksheet contains the data
                var worksheet = workbook.Worksheet(1);
                //Lets assume first row contains the header, so skip the first row
                var rows = worksheet.RangeUsed().RowsUsed().Skip(1);
                //Loop Through all the Rows except the first row which contains the header data
                foreach (var row in rows)
                {
                    //Create an Instance of Employee object and populate it with the Excel Data Row
                    var employee = new VisitingCardModel
                    {
                        company_name = row.Cell(1).GetValue<string>(),
                        first_name = row.Cell(2).GetValue<string>(),
                        last_name = row.Cell(3).GetValue<string>(),
                        contactno1 = row.Cell(4).GetValue<string>(),
                        contactno2 = row.Cell(5).GetValue<string>(),
                        emailid = row.Cell(6).GetValue<string>(),
                        designation = row.Cell(7).GetValue<string>(),
                        address = row.Cell(8).GetValue<string>(),
                        qty = row.Cell(9).GetValue<int>(),

                    };
                    //Add the Employee to the List of Employees
                    employees.Add(employee);
                }
            }
            //Finally return the List of Employees
            return employees;
        }

        [HttpPost]
        public async Task<ActionResult> saveorder_printing(PrintingModel Model,string productid, string qty)
        {

            try
            {
                CorpUserModel user = new CorpUserModel();
                user = await _corpRepository.getdataByIdsync(Convert.ToInt32(Request.Cookies["userid"].ToString()));
                productid = productid.Substring(0, productid.Length - 1);
                qty = qty.Substring(0, productid.Length - 1);
                var pid = productid.Split(',');
                var q = qty.Split(',');

                var newString = "";
                int srno = 0;
                var result = await _orderRepository.getsrnosync();
                foreach (var itm in result)
                {
                    if (itm.srno == 0)
                    {
                        newString = "1".PadLeft(6, '0');
                        srno = 1;
                    }
                    else
                    {
                        newString = (itm.srno + 1).ToString().PadLeft(6, '0');
                        srno = itm.srno + 1;
                    }


                }
                int totalqty = 0;
                decimal total_price = 0;


                //var newString = result.PadLeft(4, '0');
                DynamicParameters ObjParm = new DynamicParameters();
                ObjParm.Add("@id", Model.id);
                ObjParm.Add("@orderid", "O" + newString);
                ObjParm.Add("@clientid", Convert.ToInt32(Request.Cookies["clientid"].ToString()));
                ObjParm.Add("@userid", Convert.ToInt32(Request.Cookies["userid"].ToString()));
                ObjParm.Add("@category", "Printing");
                ObjParm.Add("@total_amount", "0");
                ObjParm.Add("@total_item", "0");
                ObjParm.Add("@status", "1");
                ObjParm.Add("@srno", srno);

                ObjParm.Add("@approval_for", user.approveby);
                ObjParm.Add("@createdby", Convert.ToInt32(Request.Cookies["userid"].ToString()));

                ObjParm.Add("@result", dbType: DbType.Int32, direction: ParameterDirection.Output, size: 5215585);

                await _orderRepository.AddAsync(ObjParm);
                var id = ObjParm.Get<dynamic>("@result");
                if (id > 0)
                {
                    //if (file != null && file.Length > 0)
                    //{
                    //    var productlist = ParseExcelFile(file.OpenReadStream());
                    //    foreach (var item in productlist)
                    //    {
                    //        DynamicParameters ObjParm1 = new DynamicParameters();
                    //        ObjParm1.Add("@id", "0");
                    //        ObjParm1.Add("@orderid", id);
                    //        ObjParm1.Add("@productid", Model.productid);
                    //        ObjParm1.Add("@company_name", item.company_name);
                    //        ObjParm1.Add("@first_name", item.first_name);
                    //        ObjParm1.Add("@last_name", item.last_name);
                    //        ObjParm1.Add("@contactno1", item.contactno1);
                    //        ObjParm1.Add("@contactno2", item.contactno2);
                    //        ObjParm1.Add("@emailid", item.emailid);
                    //        ObjParm1.Add("@designation", item.designation);
                    //        ObjParm1.Add("@address", item.address);
                    //        ObjParm1.Add("@qty", item.qty);
                    //        ObjParm1.Add("@createdby", Convert.ToInt32(Request.Cookies["userid"].ToString()));

                    //        await _orderRepository.Addorder_visiting_detailsAsync(ObjParm1);
                    //    }
                    //}
                    //else
                    //{
                    if (pid.Length > 0)
                    {
                        for (int i = 0; i <= pid.Length-1; i++)
                        {
                            DynamicParameters ObjParm1 = new DynamicParameters();
                            ObjParm1.Add("@id", "0");
                            ObjParm1.Add("@orderid", id);
                            ObjParm1.Add("@productid", pid[i]);
                            ObjParm1.Add("@location", Model.location);
                            ObjParm1.Add("@companyname", Model.companyname);
                            ObjParm1.Add("@address", Model.address);
                            ObjParm1.Add("@telno", Model.telno);
                            ObjParm1.Add("@mobileno", Model.mobileno);
                            ObjParm1.Add("@website", Model.website);
                            ObjParm1.Add("@qty", q[i]);
                            await _orderRepository.Addorder_printing_detailsAsync(ObjParm1);
                        }
                    }
                   // }
                    TempData["MsgType"] = "Success";

                    TempData["Msg"] = "Order Placed Successfully";

                }
                else
                {
                    TempData["MsgType"] = "Error";
                    TempData["Msg"] = "Something went wrong";
                }

                return RedirectToAction("AllOrder");
            }
            catch (Exception ex)
            {
                TempData["MsgType"] = "Error";
                TempData["Msg"] = ex.Message;
                return View();
            }
        }
        //====================Vendor Order=================
        public async Task<ActionResult> OrderList()
        {
            var result = await _orderRepository.getorderlistforvendor(6);
            return View(result);
        }
        public async Task<ActionResult> OrderDetails(int id)
        {
            var result = await _orderRepository.get_order_details(id);
            return View(result);
        }
        public async Task<ActionResult> ApprovedOrderlist()
        {
            var result = await _orderRepository.getorderlistforvendor(12);
            return View(result);
        }
        public async Task<ActionResult> RejectedOrderlist()
        {
            var result = await _orderRepository.getorderlistforvendor(13);
            return View(result);
        }
        public async Task<ActionResult> ReleasedOrderlist()
        {
            var result = await _orderRepository.getorderlistforvendor(16);
            return View(result);
        }

        public async Task<ActionResult> update_qty(int id,int qty,string rate,int orderid)
        {
            try
            {

                //var newString = result.PadLeft(4, '0');
                DynamicParameters ObjParm1 = new DynamicParameters();
                ObjParm1.Add("@id",id);
                ObjParm1.Add("@orderid","0");
                ObjParm1.Add("@productid", "0");
                ObjParm1.Add("@rate", "0");
                ObjParm1.Add("@qty", "0");
                ObjParm1.Add("@total_amount", "0");
                ObjParm1.Add("@user_type", "Vendor");
                ObjParm1.Add("@updated_qty", qty);
                var total = Convert.ToInt32(qty) * Convert.ToDecimal(rate);
                ObjParm1.Add("@updated_amount", Convert.ToDecimal(total));
                if (id == 0)
                {

                    ObjParm1.Add("@createdby", Convert.ToInt32(Request.Cookies["userid"].ToString()));
                }
                else
                {
                    ObjParm1.Add("@createdby", Convert.ToInt32(Request.Cookies["userid"].ToString()));
                }

                await _orderRepository.updateqty(ObjParm1);

                TempData["MsgType"] = "Success";
                    if (id == 0)
                    {
                        TempData["Msg"] = "Record Saved Successfully";
                    }
                    else
                    {
                        TempData["Msg"] = "Record Updated Successfully";
                    }
              

                return Redirect("../Order/OrderDetails?id="+orderid+"");
            }
            catch (Exception ex)
            {
                TempData["MsgType"] = "Error";
                TempData["Msg"] = ex.Message;
                return View();
            }
        }

        public async Task<ActionResult> reject_item(int id, int orderid)
        {
            try
            {

                //var newString = result.PadLeft(4, '0');
                DynamicParameters ObjParm1 = new DynamicParameters();
                ObjParm1.Add("@id", id);
                ObjParm1.Add("@orderid", "0");
                ObjParm1.Add("@productid", "0");
                ObjParm1.Add("@rate", "0");
                ObjParm1.Add("@qty", "0");
                ObjParm1.Add("@total_amount", "0");
                ObjParm1.Add("@user_type", "Vendor");
                ObjParm1.Add("@is_reject", "1");
                ObjParm1.Add("@reject_by", Convert.ToInt32(Request.Cookies["userid"].ToString()));
                if (id == 0)
                {

                    ObjParm1.Add("@createdby", Convert.ToInt32(Request.Cookies["userid"].ToString()));
                }
                else
                {
                    ObjParm1.Add("@createdby", Convert.ToInt32(Request.Cookies["userid"].ToString()));
                }

                await _orderRepository.updateqty(ObjParm1);

                TempData["MsgType"] = "Success";
                if (id == 0)
                {
                    TempData["Msg"] = "Item Rejected Successfully";
                }
                else
                {
                    TempData["Msg"] = "Item Rejected Successfully";
                }


                return Redirect("../Order/OrderDetails?id=" + orderid + "");
            }
            catch (Exception ex)
            {
                TempData["MsgType"] = "Error";
                TempData["Msg"] = ex.Message;
                return View();
            }
        }

        public async Task<string> update_order_status(int id,int status,int userid,string remark, string shipping_address, string billing_address,string shippingaddress2)
        {
            try
            {
                CorpUserModel user = new CorpUserModel();
                if(status == 12)
                {
                    var order = (await _orderRepository.getorderbyid(id)).FirstOrDefault();
                    user = await _corpRepository.getdataByIdsync(order.userid);
                    
                    //get user obect based on order created by
                    //_orderRepository.get_order_details
                } else
                {
                    user = await _corpRepository.getdataByIdsync(Convert.ToInt32(Request.Cookies["userid"].ToString()));
                }
                

                var Result = await _orderRepository.getorderlist(0, id);
                string ap1 = "0";
                string ap2 = "0";
                string ap3 = "0";
                string ap4 = "0";
                int is_true = 0;
                string orderid = "";
                string qty = "0";
                string amount = "0";
                
                foreach (var item in Result)
                {
                    orderid = item.orderid;
                    qty=item.qty;
                    amount = item.amount;
                    if (item.approvel_1 != null)
                    {
                        ap1 = item.approve_1.ToString();
                    }
                    else {
                        ap1 = Request.Cookies["userid"].ToString();
                        is_true = 1;
                    }
                    if (item.approvel_2 != null)
                    {
                        ap2 = item.approve_2.ToString();
                    }
                    else {
                        if (is_true == 0)
                        {
                            ap2 = Request.Cookies["userid"].ToString();
                            is_true = 1;
                        }
                    }
                    if (item.approvel_3 != null)
                    {
                        ap3 = item.approve_3.ToString();
                    }
                    else 
                        //if (is_true == 0)
                        {
                            ap3 = Request.Cookies["userid"].ToString();
                            is_true = 1;
                        }
                    
                }

                //var newString = result.PadLeft(4, '0');
                DynamicParameters ObjParm = new DynamicParameters();
                ObjParm.Add("@id", id);
                ObjParm.Add("@orderid", "0");
                ObjParm.Add("@clientid", Convert.ToInt32(Request.Cookies["userid"].ToString()));
                ObjParm.Add("@userid", userid);
                ObjParm.Add("@category", "0");
                ObjParm.Add("@total_amount", "0");
                ObjParm.Add("@total_item", "0");
                ObjParm.Add("@remark", remark);
                ObjParm.Add("@shipping_address", shipping_address);
                ObjParm.Add("@billing_address", billing_address);
                ObjParm.Add("@shippingaddress2", shippingaddress2);
                if (status == 1)
                {
                    ObjParm.Add("@status", 6);
                }
                else {
                    ObjParm.Add("@status", status);
                }
                ObjParm.Add("@app1",ap1);
                ObjParm.Add("@app2", ap2);
                ObjParm.Add("@app3", ap3);
                ObjParm.Add("@app4", ap4);
                if (user != null)
                {
                    ObjParm.Add("@approval_for", user.approveby);
                }
                else {
                    ObjParm.Add("@approval_for", 0);
                }
                ObjParm.Add("@srno", "0");
                if (id == 0)
                {

                    ObjParm.Add("@createdby", Convert.ToInt32(Request.Cookies["userid"].ToString()));
                }
                else
                {
                    ObjParm.Add("@createdby", Convert.ToInt32(Request.Cookies["userid"].ToString()));
                }
                ObjParm.Add("@result", dbType: DbType.Int32, direction: ParameterDirection.Output, size: 5215585);

                await _orderRepository.AddAsync(ObjParm);
                var i = ObjParm.Get<dynamic>("@result");
                if (i > 0)
                { 
                    TempData["MsgType"] = "Success";
                    if (id == 0)
                    {
                        TempData["Msg"] = "Order Update Successfully";
                    }
                    else
                    {
                        TempData["Msg"] = "Order Updated Successfully";
                    }
                    if (status == 12) // approval by vendor
                    {
                        var result2 = await _corpRepository.getdataByIdsync(Convert.ToInt32(user.approveby));

                        StreamReader sr = new StreamReader("Challan/order_summary.html");
                        string s = sr.ReadToEnd();
                        s = s.Replace("{adminname}", result2.firstname + " " + result2.lastname).Replace("{clientname}", user.client_name).Replace("{orderno}", orderid).Replace("{Username}", user.firstname + " " + user.lastname).Replace("{department}", user.department_name).Replace("{branch}", user.branch_name).Replace("{level}", user.approval_type.ToString()).Replace("{totalitem}", qty.ToString()).Replace("{totalprice}", amount.ToString()).Replace("{remark}", "").Replace("{url}", "http://erp.vijbin.com/Client/new_Order");
                        string tblitem = "";
                        var Result1 = await _orderRepository.get_order_details(id);
                        foreach (var item in Result1)
                        {
                            tblitem += "<tr><td><img src='https://erp.vijbin.com/uploads/" + item.ImageUrl + "'/></td><td>" + item.Description + "</td><td>" + item.qty + "</td><td>" + item.rate + "</td><td>" + (Convert.ToDecimal(item.qty) * Convert.ToDecimal(item.rate)) + "</td></tr>";
                        }
                        s = s.Replace("{item}", tblitem);

                      string  msg = s;
                        _sendMail.Send_Mail("rder@erp.vijbin.com.com", "Order Approval Required for Order No. " + orderid + "", msg);
                    }
                    else
                    {
                        if (user != null)
                        {
                            var result2 = await _corpRepository.getdataByIdsync(Convert.ToInt32(user.approveby));

                            string msg = "<h3>Hello Admin (" + result2.firstname + " " + result2.lastname + "),</h3><br/>";
                            msg += "<p>The order no. " + orderid + ", which is requested by your requester (" + user.firstname + " " + user.lastname + "), is waiting for your approval. Below is the summary of the order:</p>";

                            msg += "<p><b>Client Name:</b> " + user.client_name + "</p>";
                            msg += "<p><b>Order Number</b>: " + orderid + "</p>";
                            msg += "<p><b>Department:</b> " + user.department_name + "</p>";
                            msg += "<p><b>Branch:</b> " + user.branch_name + "</p>";
                            msg += "<p><b>Approval Level:</b> " + user.approval_type + "</p>";
                            msg += "<p><b>Total Items:</b> " + qty + "</p>";
                            msg += "<p><b>Total Price:</b> " + amount + "</p>";
                            msg += "<p>You can also approve the order just by clicking the below URL:</p>";
                            msg += "https://erp.vijbin.com/Client/new_Order";
                            _sendMail.Send_Mail(result2.emailid, "Order Approval Required for Order No. " + orderid + "", msg);
                        }
                    }
                }
                else
                {
                    TempData["MsgType"] = "Error";
                    TempData["Msg"] = "Something went wrong";
                }

                return "Order Details Update Successfully";
            }
            catch (Exception ex)
            {
                TempData["MsgType"] = "Error";
                TempData["Msg"] = ex.Message;
                return ex.Message;
            }
        }

        public async Task<string> update_user_order(int id,int qty,string rate,int o=0)
        {
            try
            {
                DynamicParameters ObjParm1 = new DynamicParameters();
                ObjParm1.Add("@id", id);
                ObjParm1.Add("@orderid", "0");
                ObjParm1.Add("@productid", "0");
                ObjParm1.Add("@rate", "0");
                ObjParm1.Add("@qty", "0");
                ObjParm1.Add("@total_amount", "0");
                ObjParm1.Add("@updated_qty", qty);
                ObjParm1.Add("@updated_amount", (qty * Convert.ToDecimal(rate)));

                ObjParm1.Add("@createdby", Convert.ToInt32(Request.Cookies["userid"].ToString()));

                await _orderRepository.Addorder_detailsAsync(ObjParm1);


                var Result = await _orderRepository.get_order_details(o);
                int totalqty = 0;
                if (Result.Count()>0)
                {
                    int orderid = 0;
                    foreach (var item in Result)
                    {
                        orderid = item.orderid;
                        if (item.updated_qty != "0")
                        {
                            totalqty = totalqty + Convert.ToInt32(item.updated_qty);
                        }
                        else {
                            totalqty = totalqty + Convert.ToInt32(item.qty);
                        }
                    }

                    DynamicParameters ObjParm = new DynamicParameters();
                    ObjParm.Add("@id", orderid);
                    ObjParm.Add("@qty", totalqty);
                    

                    await _orderRepository.updateorder_detailsAsync(ObjParm);
                }

                return "Order Details Update Successfully";
            }
            catch (Exception ex)
            {

                return ex.Message;
            }
        }

        public async Task<ActionResult> update_order_statusByEmail(int id, int status, int userid,int clientid)
        {
            try
            {
                CorpUserModel user = new CorpUserModel();
                user = await _corpRepository.getdataByIdsync(clientid);

                CorpUserModel user1 = new CorpUserModel();
                user1 = await _corpRepository.getdataByIdsync(user.approveby);

                var Result = await _orderRepository.getorderlist(0, id);
                string ap1 = "0";
                string ap2 = "0";
                string ap3 = "";
                int is_true = 0;
                string orderid = "";
                string qty = "0";
                string amount = "0";

                foreach (var item in Result)
                {
                    orderid = item.orderid;
                    qty = item.qty;
                    amount = item.amount;
                    if (item.approvel_1 != null)
                    {
                        ap1 = item.approve_1.ToString();
                    }
                    else
                    {
                        ap1 = clientid.ToString();
                        is_true = 1;
                    }
                    if (item.approvel_2 != null)
                    {
                        ap2 = item.approve_2.ToString();
                    }
                    else
                    {
                        if (is_true == 0)
                        {
                            ap2 = clientid.ToString();
                            is_true = 1;
                        }
                    }
                    if (item.approvel_3 != null)
                    {
                        ap3 = item.approve_3.ToString();
                    }
                    else
                    {
                        if (is_true == 0)
                        {
                            ap3 = clientid.ToString();
                            is_true = 1;
                        }
                    }
                }

                //var newString = result.PadLeft(4, '0');
                DynamicParameters ObjParm = new DynamicParameters();
                ObjParm.Add("@id", id);
                ObjParm.Add("@orderid", "0");
                ObjParm.Add("@clientid", clientid);
                ObjParm.Add("@userid", userid);
                ObjParm.Add("@category", "0");
                ObjParm.Add("@total_amount", "0");
                ObjParm.Add("@total_item", "0");
                if (status == 1)
                {
                    ObjParm.Add("@status", 6);
                }
                else
                {
                    ObjParm.Add("@status", status);
                }
                ObjParm.Add("@app1", ap1);
                ObjParm.Add("@app2", ap2);
                ObjParm.Add("@app3", ap3);
                if (user != null)
                {
                    ObjParm.Add("@approval_for", user.approveby);
                }
                else
                {
                    ObjParm.Add("@approval_for", 0);
                }
                ObjParm.Add("@srno", "0");
                if (id == 0)
                {

                    ObjParm.Add("@createdby", Convert.ToInt32(Request.Cookies["userid"].ToString()));
                }
                else
                {
                    ObjParm.Add("@createdby", Convert.ToInt32(Request.Cookies["userid"].ToString()));
                }
                ObjParm.Add("@result", dbType: DbType.Int32, direction: ParameterDirection.Output, size: 5215585);

                await _orderRepository.AddAsync(ObjParm);
                var i = ObjParm.Get<dynamic>("@result");
                if (i > 0)
                {
                    //TempData["MsgType"] = "Success";
                    //if (id == 0)
                    //{
                    //    TempData["Msg"] = "Order Update Successfully";
                    //}
                    //else
                    //{
                    //    TempData["Msg"] = "Order Updated Successfully";
                    //}
                    if (status == 1)
                    {
                        var result2 = await _corpRepository.getdataByIdsync(Convert.ToInt32(user.approveby));

                        StreamReader sr = new StreamReader("Challan/order_summary.html");
                        string s = sr.ReadToEnd();
                        s = s.Replace("{adminname}", result2.firstname + " " + result2.lastname).Replace("{clientname}", user.client_name).Replace("{orderno}", orderid).Replace("{Username}", user.firstname + " " + user.lastname).Replace("{department}", user.department_name).Replace("{branch}", user.branch_name).Replace("{level}", user.approval_type.ToString()).Replace("{totalitem}", qty.ToString()).Replace("{totalprice}", amount.ToString()).Replace("{remark}", "").Replace("{url}", "<a href='http://erp.vijbin.com/Order/update_order_statusByEmail?id=" + id + "&status="+ user1.approval_type + "&userid="+ Convert.ToInt32(Request.Cookies["userid"].ToString()) + "&clientid="+ user.approveby + " class='btn btn-success'>Accept</a> &nbsp;&nbsp;&nbsp;  <a href='' class='btn btn-danger'>Reject</a>");
                        string tblitem = "";
                        var Result1 = await _orderRepository.get_order_details(id);
                        foreach (var item in Result1)
                        {
                            tblitem += "<tr><td><img src='http://erp.vijbin.com/uploads/" + item.ImageUrl + "' style='width:100%'/></td><td>" + item.Description + "</td><td>" + item.qty + "</td><td>" + item.rate + "</td><td>" + (Convert.ToDecimal(item.qty) * Convert.ToDecimal(item.rate)) + "</td></tr>";
                        }
                        s = s.Replace("{item}", tblitem);

                        string msg = s;
                        _sendMail.Send_Mail(user1.emailid, "Order Approval Required for Order No. " + orderid + "", msg);

                        return Redirect("http://erp.vijbin.com/Order/msgpage?i=" + orderid);
                    }
                    else
                    {
                        if (user != null)
                        {
                            var result2 = await _corpRepository.getdataByIdsync(Convert.ToInt32(user.approveby));

                            StreamReader sr = new StreamReader("Challan/order_summary.html");
                            string s = sr.ReadToEnd();
                            s = s.Replace("{adminname}", result2.firstname + " " + result2.lastname).Replace("{clientname}", user.client_name).Replace("{orderno}", orderid).Replace("{Username}", user.firstname + " " + user.lastname).Replace("{department}", user.department_name).Replace("{branch}", user.branch_name).Replace("{level}", user.approval_type.ToString()).Replace("{totalitem}", qty.ToString()).Replace("{totalprice}", amount.ToString()).Replace("{remark}", "").Replace("{url}", "<a href='http://erp.vijbin.com/Order/update_order_statusByEmail?id=" + id + "&status=" + user1.approval_type + "&userid=" + Convert.ToInt32(Request.Cookies["userid"].ToString()) + "&clientid=" + user.approveby + " class='btn btn-success'>Accept</a> &nbsp;&nbsp;&nbsp;  <a href='' class='btn btn-danger'>Reject</a>");
                            string tblitem = "";
                            var Result1 = await _orderRepository.get_order_details(id);
                            foreach (var item in Result1)
                            {
                                tblitem += "<tr><td><img src='http://erp.vijbin.com/uploads/" + item.ImageUrl + "' style='width:100%'/></td><td>" + item.Description + "</td><td>" + item.qty + "</td><td>" + item.rate + "</td><td>" + (Convert.ToDecimal(item.qty) * Convert.ToDecimal(item.rate)) + "</td></tr>";
                            }
                            s = s.Replace("{item}", tblitem);

                            string msg = s;
                            _sendMail.Send_Mail(user1.emailid, "Order Approval Required for Order No. " + orderid + "", msg);

                            return Redirect("http://erp.vijbin.com/Order/msgpage?i=" + orderid);
                        }
                    }
                }
                else
                {
                    TempData["MsgType"] = "Error";
                    TempData["Msg"] = "Something went wrong";
                }

               // return "Order Details Update Successfully";
            }
            catch (Exception ex)
            {
                TempData["MsgType"] = "Error";
                TempData["Msg"] = ex.Message;
                return Redirect("http://erp.vijbin.com/Order/msgpage?i=");
            }
            return Redirect("http://erp.vijbin.com/Order/msgpage?i=");
        }
        //========================End======================

        public ActionResult msgpage(string i)
        {
            ViewBag.msg = i;
            return View();
        }

        public string Delete_StationaryOrder(int id)
        {
            try
            {
                _orderRepository.Delete_stationaryAsync(id);

                return "Record Deleted Successfully";
            }
            catch (Exception ex)
            {


                return ex.Message;
            }


        }

        public string Delete_Details_StationaryOrder(int id)
        {
            try
            {
                _orderRepository.Delete_Orderdetails_stationaryAsync(id);

                return "Record Deleted Successfully";
            }
            catch (Exception ex)
            {


                return ex.Message;
            }


        }
    }
}
