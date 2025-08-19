using ClosedXML.Excel;
using Dapper;
using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using OTS.Data.Models;
using OTS.Data.Repository;
using System.Data;
using System.IO;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography.Xml;
using System.Text;
using Document = iTextSharp.text.Document;
using PageSize = iTextSharp.text.PageSize;

namespace OTS.UI.Controllers
{
    public class UserController : Controller
    {
        private readonly IVendorRepository _vendorRepository;
        private readonly ICostRepository _costRepository;
        private readonly ICorpRepository _corpRepository;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly Iproductrepository _productrepository;
        private readonly IcartRepository _cartrepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IClientRepository _clientRepository;
        private readonly ISendMailRepositery _sendMail;

        public UserController(IVendorRepository vendorRepository,ICostRepository costRepository,ICorpRepository corpRepository,IDepartmentRepository departmentRepository, Iproductrepository productrepository,IcartRepository cartrepository,IOrderRepository orderRepository,IClientRepository clientRepository, ISendMailRepositery sendMail)
        {
            _vendorRepository = vendorRepository;
            _costRepository = costRepository;
            _corpRepository = corpRepository;
            _departmentRepository = departmentRepository;
            _productrepository = productrepository;
            _cartrepository = cartrepository;
            _orderRepository = orderRepository;
            _clientRepository = clientRepository;
            _sendMail = sendMail;
        }

        public async Task<ActionResult> getnotification()
        {
            List<NotificationModel> Modellist = new List<NotificationModel>();
            var result = await _clientRepository.getnotoficationsync(Convert.ToInt32(Request.Cookies["userid"].ToString()), "User");
            foreach (var item in result)
            {
                NotificationModel Model = new NotificationModel();
                Model.url = item.url;
                Model.msg = item.msg;
                Modellist.Add(Model);
            }
            return Ok(Modellist);
        }
        public ActionResult Add()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> Add(VendorModel Model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    DynamicParameters ObjParm = new DynamicParameters();
                    ObjParm.Add("@id", Model.Id);
                    ObjParm.Add("@name", Model.name);
                    ObjParm.Add("@emailid", Model.emailid);
                    ObjParm.Add("@phoneno ", Model.phoneno);
                    ObjParm.Add("@state", Model.state);
                    ObjParm.Add("@gstno", Model.gstno);
                    ObjParm.Add("@location", Model.location);
                    ObjParm.Add("@servicecategory", Model.servicecategory);
                    ObjParm.Add("@username", Model.username);
                    ObjParm.Add("@password", Model.password);
                    ObjParm.Add("@region", Model.region);
                    ObjParm.Add("@clientid", Convert.ToInt32(Request.Cookies["clientid"].ToString()));
                    if (Model.Id == 0)
                    {

                        Model.createdby = Convert.ToInt32(Request.Cookies["userid"].ToString());
                        ObjParm.Add("@createdby", Model.createdby);
                    }
                    else
                    {
                        Model.updatedby = Convert.ToInt32(Request.Cookies["userid"].ToString());
                        ObjParm.Add("@updatedby", Model.updatedby);
                    }
                    ObjParm.Add("@result", dbType: DbType.Int32, direction: ParameterDirection.Output, size: 5215585);

                    await _vendorRepository.AddAsync(ObjParm);
                    var id = ObjParm.Get<dynamic>("@result");
                    if (id > 0)
                    {
                        TempData["MsgType"] = "Success";
                        if (Model.Id == 0)
                        {
                            TempData["Msg"] = "Record Saved Successfully";
                        }
                        else
                        {
                            TempData["Msg"] = "Record Updated Successfully";
                        }
                    }
                    else
                    {
                        TempData["MsgType"] = "Error";
                        TempData["Msg"] = "Something went wrong";
                    }
                }
                return View();
            }
            catch (Exception ex)
            {
                TempData["MsgType"] = "Error";
                TempData["Msg"] = ex.Message;
                return View();
            }
        }

        public async Task<ActionResult> Edit_user(int id)
        {
            var result = await _vendorRepository.getdataByIdsync(id);

            return View(result);
        }
        public async Task<ActionResult> List()
        {
            var result = await _vendorRepository.getdatasync();
            return View(result);
        }

        public string Delete_vendor(int id)
        {
            try
            {
                _vendorRepository.DeleteAsync(id);

                return "Record Deleted Successfully";
            }
            catch (Exception ex)
            {


                return ex.Message;
            }


        }

        //=============== Corporate User===============//
        public async Task<ActionResult> Add_corporateuser()
        {
            combomodel combolist = new combomodel();
            List<VendorModel> list = new List<VendorModel>();
            var result = await _vendorRepository.getdatasync();
            foreach (var item in result)
            {
                VendorModel model = new VendorModel();
                model.Id=item.Id;
                model.name = item.name;
                list.Add(model);
            }
           

            List<CostCenter> costlist = new List<CostCenter>();
            var result1 = await _costRepository.getdatasync();
            foreach (var item in result1)
            {
                CostCenter model = new CostCenter();
                model.Id = item.Id;
                model.Name = item.Name;
                costlist.Add(model);
            }

            List<CorpUserModel> corptlist = new List<CorpUserModel>();
            var result2 = await _corpRepository.getdatasync();
            foreach (var item in result2)
            {
                CorpUserModel model = new CorpUserModel();
                model.Id = item.Id;
                model.firstname = item.firstname;
                model.lastname = item.lastname;
                corptlist.Add(model);
            }

            List<UserTypeModel> usertypetlist = new List<UserTypeModel>();
            var result3 = await _corpRepository.getusertype();
            foreach (var item in result3)
            {
                UserTypeModel model = new UserTypeModel();
                model.id = item.id;
                model.usertype = item.usertype;

                usertypetlist.Add(model);
            }

            List<DepartmentModel> departmentlist = new List<DepartmentModel>();
            var result4 = await _departmentRepository.getdatasync();
            foreach (var item in result4)
            {
                DepartmentModel model = new DepartmentModel();
                model.Id = item.Id;
                model.Name = item.Name;

                departmentlist.Add(model);
            }
            
            combolist.VendorModels = list;
            combolist.costCenters = costlist;
            combolist.CorpUserModels = corptlist;
            combolist.UserTypeModels = usertypetlist;
            combolist.departmentModels = departmentlist;
            return View(combolist);
        }

        public async Task<ActionResult> Edit_corporateuser(int id)
        {
            combomodel combolist = new combomodel();
            List<VendorModel> list = new List<VendorModel>();
            var result = await _vendorRepository.getdatasync();
            foreach (var item in result)
            {
                VendorModel model = new VendorModel();
                model.Id = item.Id;
                model.name = item.name;
                list.Add(model);
            }


            List<CostCenter> costlist = new List<CostCenter>();
            var result1 = await _costRepository.getdatasync();
            foreach (var item in result1)
            {
                CostCenter model = new CostCenter();
                model.Id = item.Id;
                model.Name = item.Name;
                costlist.Add(model);
            }

            List<CorpUserModel> corptlist = new List<CorpUserModel>();
            var result2 = await _corpRepository.getdatasync();
            foreach (var item in result2)
            {
                CorpUserModel model = new CorpUserModel();
                model.Id = item.Id;
                model.firstname = item.firstname;
                model.lastname = item.lastname;
                corptlist.Add(model);
            }

            List<UserTypeModel> usertypetlist = new List<UserTypeModel>();
            var result3 = await _corpRepository.getusertype();
            foreach (var item in result3)
            {
                UserTypeModel model = new UserTypeModel();
                model.id = item.id;
                model.usertype = item.usertype;

                usertypetlist.Add(model);
            }

            List<DepartmentModel> departmentlist = new List<DepartmentModel>();
            var result4 = await _departmentRepository.getdatasync();
            foreach (var item in result4)
            {
                DepartmentModel model = new DepartmentModel();
                model.Id = item.Id;
                model.Name = item.Name;

                departmentlist.Add(model);
            }
            CorpUserModel corpmodel = new CorpUserModel();
            corpmodel = await _corpRepository.getdataByIdsync(id);
            

            combolist.VendorModels = list;
            combolist.costCenters = costlist;
            combolist.CorpUserModels = corptlist;
            combolist.UserTypeModels = usertypetlist;
            combolist.departmentModels = departmentlist;
            combolist.CorpUser_Models = corpmodel;
            return View(combolist);
        }

        [HttpPost]
        public async Task<ActionResult> Add_corporateuser(CorpUserModel Model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    DynamicParameters ObjParm = new DynamicParameters();
                    ObjParm.Add("@id", Model.Id);
                    ObjParm.Add("@vendorid", Model.vendorid);
                    ObjParm.Add("@firstname", Model.firstname);
                    ObjParm.Add("@lastname", Model.lastname);
                    ObjParm.Add("@contactno", Model.contactno);
                    ObjParm.Add("@username", Model.username);
                    ObjParm.Add("@password", Model.password);
                    ObjParm.Add("@emailid", Model.emailid);
                    ObjParm.Add("@regions", Model.regions);
                    ObjParm.Add("@usertype", Model.usertype);
                    ObjParm.Add("@department", Model.department);

                    ObjParm.Add("@approval_type", Model.approval_type);
                    ObjParm.Add("@approveby", Model.approveby);
                    ObjParm.Add("@branchid", Model.branchid);
                    ObjParm.Add("@tat", Model.tat);
                    ObjParm.Add("@limitamount", Model.limitamount);
                    ObjParm.Add("@billingaddress", Model.billingaddress);
                    ObjParm.Add("@shippingaddress1", Model.shippingaddress1);
                    ObjParm.Add("@shippingaddress2", Model.shippingaddress2);
                    ObjParm.Add("@clientid", Convert.ToInt32(Request.Cookies["clientid"].ToString()));
                    if (Model.Id == 0)
                    {

                        Model.createdby = Convert.ToInt32(Request.Cookies["userid"].ToString());
                        ObjParm.Add("@createdby", Model.createdby);
                    }
                    else
                    {
                        Model.updatedby = Convert.ToInt32(Request.Cookies["userid"].ToString());
                        ObjParm.Add("@updatedby", Model.updatedby);
                    }
                    ObjParm.Add("@result", dbType: DbType.Int32, direction: ParameterDirection.Output, size: 5215585);

                    await _corpRepository.AddAsync(ObjParm);
                    var id = ObjParm.Get<dynamic>("@result");
                    if (id > 0)
                    {
                        TempData["MsgType"] = "Success";
                        if (Model.Id == 0)
                        {
                            string html = "<p>Hello "+ Model.firstname+" "+ Model.lastname+",<p>";

                            html += "<p>Welcome to the OTS Portal!</p>";
                            html += "<p>We have created your login credentials for placing orders and making your life hassle-free. You can change the password upon your first login.</p>";
                            html += "<p><b>Username: </b>"+Model.username +"</p>";
                            html += "<p><b>Password: </b>"+Model.password + "</p>";
                            html += "<p><b>URL to Login: </b> http://order.shree-harienterprises.com/ </p>";

                            html += "<p>Enjoy your day!</p>";

                            _sendMail.Send_Mail(Model.emailid, "Welcome to OTS Portal - Your Account Details", html);
                            TempData["Msg"] = "Record Saved Successfully";
                        }
                        else
                        {
                            TempData["Msg"] = "Record Updated Successfully";
                        }
                    }
                    else
                    {
                        TempData["MsgType"] = "Error";
                        TempData["Msg"] = "Something went wrong";
                    }
                }
                var userRoleType = Request.Cookies["UserRoleType"]?.ToString();
                if (userRoleType == "1")
                    return Redirect("../Client/ViewUser");
                else
                    return Redirect("../supplier/userlist");
            }
            catch (Exception ex)
            {
                TempData["MsgType"] = "Error";
                TempData["Msg"] = ex.Message;
                return Redirect("../supplier/user");
            }
        }

        public async Task<ActionResult> corporateuserList()
        {
            var result = await _corpRepository.getdatasync();
            return View(result);
        }

        public string Delete_CorpUser(int id)
        {
            try
            {
                _corpRepository.DeleteAsync(id);

                return "Record Deleted Successfully";
            }
            catch (Exception ex)
            {


                return ex.Message;
            }


        }
        //=======================End==================//

        //==================User Dashboard===================
        public async Task<ActionResult> index()
        {
            combomodel model=new combomodel();
            List<ProductModel> products=new List<ProductModel>();
            List<CartModel> cart=new List<CartModel>();
            List<OrderModel> OrderModel = new List<OrderModel>();
            var result = await _productrepository.getdataBycategorysync("Stationery", 0);
            foreach (var item in result)
            {
                ProductModel product = new ProductModel();
                product.id = item.id;
                product.Description = item.Description;
                product.Rate = item.Rate;
                product.ImageUrl = item.ImageUrl;
                product.stock = item.stock;
                products.Add(product);
            }

            var result2 = await _productrepository.getdataBycategorysync("Houskeeping", 0);
            foreach (var item in result2)
            {
                ProductModel product = new ProductModel();
                product.id = item.id;
                product.Description = item.Description;
                product.Rate = item.Rate;
                product.ImageUrl = item.ImageUrl;
                product.stock = item.stock;
                products.Add(product);
            }

            var result1 = await _cartrepository.getdatasync(Convert.ToInt32(Request.Cookies["userid"].ToString()), Convert.ToInt32(Request.Cookies["clientid"].ToString()));

            foreach (var item in result1)
            {
                CartModel cartModel = new CartModel();
                cartModel.Id = item.Id;
                cartModel.Description = item.Description;
                cartModel.rate = item.rate;
                cartModel.ImageUrl = item.ImageUrl;
                cartModel.qty = item.qty;
                cart.Add(cartModel);
            }

            var Result2 = await _orderRepository.get_User_order_list(Convert.ToInt32(Request.Cookies["userid"].ToString()), Convert.ToInt32(Request.Cookies["clientid"].ToString()), "All");
            decimal totalamount=0;
            foreach (var item in Result2)
            {
                OrderModel cartModel = new OrderModel();
                cartModel.orderid = item.orderid;
                cartModel.id = Convert.ToInt32(item.id);
                cartModel.category = item.Description;
                cartModel.qty = item.qty;
                cartModel.orderdate = item.orderdate;
                cartModel.total_amount = item.total_amount;
                totalamount = totalamount + Convert.ToDecimal(item.total_amount);
                
                OrderModel.Add(cartModel);
            }
            decimal limitamount = Convert.ToDecimal(Request.Cookies["limit"].ToString());
            ViewBag.pending = limitamount - totalamount;
            model.product_Model = products;
            model.cart_Model= cart;
            model.OrderModel = OrderModel;
            return View(model);
        }

        public async Task<ActionResult> Housekeeping()
        {
            combomodel model = new combomodel();
            List<ProductModel> products = new List<ProductModel>();
            List<CartModel> cart = new List<CartModel>();
            List<OrderModel> OrderModel = new List<OrderModel>();
            var result = await _productrepository.getdataBycategorysync("Houskeeping", 0);
            foreach (var item in result)
            {
                ProductModel product = new ProductModel();
                product.id = item.id;
                product.Description = item.Description;
                product.Rate = item.Rate;
                product.ImageUrl = item.ImageUrl;
                product.stock = item.stock;
                products.Add(product);
            }

            var result1 = await _cartrepository.getdatasync(Convert.ToInt32(Request.Cookies["userid"].ToString()), Convert.ToInt32(Request.Cookies["clientid"].ToString()));

            foreach (var item in result1)
            {
                CartModel cartModel = new CartModel();
                cartModel.Id = item.Id;
                cartModel.Description = item.Description;
                cartModel.rate = item.rate;
                cartModel.ImageUrl = item.ImageUrl;
                cartModel.qty = item.qty;
                cart.Add(cartModel);
            }

            var Result2 = await _orderRepository.get_User_order_list(Convert.ToInt32(Request.Cookies["userid"].ToString()), Convert.ToInt32(Request.Cookies["clientid"].ToString()), "All");

            foreach (var item in Result2)
            {
                OrderModel cartModel = new OrderModel();
                cartModel.orderid = item.orderid;
                cartModel.category = item.Description;
                cartModel.qty = item.qty;
                cartModel.orderdate = item.orderdate;
                cartModel.total_amount = item.total_amount;
                OrderModel.Add(cartModel);
            }

            model.product_Model = products;
            model.cart_Model = cart;
            model.OrderModel = OrderModel;
            return View(model);
        }

        public async Task<ActionResult> add_tocart(int id, int productid, int qty, string rate)
        {
            DynamicParameters ObjParm = new DynamicParameters();
            ObjParm.Add("@id", id);
            ObjParm.Add("@userid", Convert.ToInt32(Request.Cookies["userid"].ToString()));
            ObjParm.Add("@clientid", Convert.ToInt32(Request.Cookies["clientid"].ToString()));
            ObjParm.Add("@productid", productid);
            ObjParm.Add("@qty", qty);
            ObjParm.Add("@rate", rate);
            ObjParm.Add("@createdby", Convert.ToInt32(Request.Cookies["userid"].ToString()));
            
            ObjParm.Add("@result", dbType: DbType.Int32, direction: ParameterDirection.Output, size: 5215585);
            _cartrepository.Add_cart(ObjParm);
            var i = ObjParm.Get<dynamic>("@result");
            var result = await _cartrepository.getdatasync(Convert.ToInt32(Request.Cookies["userid"].ToString()), Convert.ToInt32(Request.Cookies["clientid"].ToString()));

           return Ok(result);
        }

        public async Task<ActionResult> wishlist_tocart(int id, int productid, int qty, string rate)
        {
            DynamicParameters ObjParm = new DynamicParameters();
            ObjParm.Add("@id", "0");
            ObjParm.Add("@userid", Convert.ToInt32(Request.Cookies["userid"].ToString()));
            ObjParm.Add("@clientid", Convert.ToInt32(Request.Cookies["clientid"].ToString()));
            ObjParm.Add("@productid", productid);
            ObjParm.Add("@qty", qty);
            ObjParm.Add("@rate", rate);
            ObjParm.Add("@createdby", Convert.ToInt32(Request.Cookies["userid"].ToString()));

            ObjParm.Add("@result", dbType: DbType.Int32, direction: ParameterDirection.Output, size: 5215585);
            _cartrepository.Add_cart(ObjParm);
            var i = ObjParm.Get<dynamic>("@result");
            _cartrepository.Deletewishlistc(id);

            return Ok("Success");
        }
        public string delete_tocart(int id)
        {
            _cartrepository.DeleteAsync(id);
            return "Record Deleted Successfully";
        }

        public async Task<ActionResult> proceedtocart()
        {
            combomodel model = new combomodel();
            CorpUserModel user = new CorpUserModel();
            List<CartModel> cart = new List<CartModel>();
            user = await _corpRepository.getdataByIdsync(Convert.ToInt32(Request.Cookies["userid"].ToString()));
            
            var result1 = await _cartrepository.getdatasync(Convert.ToInt32(Request.Cookies["userid"].ToString()), Convert.ToInt32(Request.Cookies["clientid"].ToString()));

            foreach (var item in result1)
            {
                CartModel cartModel = new CartModel();
                cartModel.Id = item.Id;
                cartModel.Description = item.Description;
                cartModel.rate = item.rate;
                cartModel.ImageUrl = item.ImageUrl;
                cartModel.qty = item.qty;
                cartModel.Make = item.Make;
                cartModel.Category = item.Category;
                cartModel.HSN = item.HSN    ;
                cartModel.gst = item.gst;
                decimal gst = Convert.ToDecimal(item.rate) * Convert.ToDecimal(item.gst) / 100;
                cartModel.totalamount = Convert.ToDecimal(Convert.ToDecimal((Convert.ToDecimal(item.rate) + gst) * item.qty).ToString("0.00"));
                cart.Add(cartModel);
            }
           
            model.cart_Model = cart;
            model.CorpUser_Models = user;
            return View(model);
        }

        public async Task<ActionResult> addneworder()
        {
            combomodel model = new combomodel();
            List<ProductModel> products = new List<ProductModel>();
            List<CartModel> cart = new List<CartModel>();
            var result = await _productrepository.getdataBycategorysync("Stationery", 0, Convert.ToInt32(Request.Cookies["userid"].ToString()), Convert.ToInt32(Request.Cookies["clientid"].ToString()));
            foreach (var item in result)
            {
                ProductModel product = new ProductModel();
                product.id = item.id;
                product.wishlistid = item.wishlistid;
                product.Description = item.Description;
                product.Rate = item.Rate;
                product.ImageUrl = item.ImageUrl;
                product.Category = item.Category;
                product.Make = item.Make;
                product.stock = item.stock;
                product.HSN = item.HSN;
                products.Add(product);
            }

            model.product_Model = products;
            
            return View(model);
        }

        public async Task<ActionResult> add_towishlist(int id, int productid)
        {
            DynamicParameters ObjParm = new DynamicParameters();
            ObjParm.Add("@id", id);
            ObjParm.Add("@userid", Convert.ToInt32(Request.Cookies["userid"].ToString()));
            ObjParm.Add("@clientid", Convert.ToInt32(Request.Cookies["clientid"].ToString()));
            ObjParm.Add("@productid", productid);
            ObjParm.Add("@createdby", Convert.ToInt32(Request.Cookies["userid"].ToString()));

            ObjParm.Add("@result", dbType: DbType.Int32, direction: ParameterDirection.Output, size: 5215585);
            _cartrepository.Add_wishlist(ObjParm);
            

            return Ok("Success");
        }

        public async Task<ActionResult> wishlist()
        {
            var result = await _cartrepository.getwishlist(Convert.ToInt32(Request.Cookies["userid"].ToString()), Convert.ToInt32(Request.Cookies["clientid"].ToString()));
            return View(result);
        }

        public string delete_wishlist(int id)
        {
            _cartrepository.Deletewishlistc(id);
            return "Record Deleted Successfully";
        }

        public async Task<ActionResult> AllOrder()
        {
            var Result = await _orderRepository.get_User_order_list(Convert.ToInt32(Request.Cookies["userid"].ToString()), Convert.ToInt32(Request.Cookies["clientid"].ToString()), "All");
            List<OrderModel> list = new List<OrderModel>();
            foreach (var item in Result)
            {
                OrderModel model = new OrderModel();
                var Result1 = await _orderRepository.get_order_details(item.id);
                int qty = 0;
                decimal amount = 0;
                foreach (var item1 in Result1)
                {
                    
                    if (item1.updated_qty != "0")
                    {
                        qty = Convert.ToInt32(item1.updated_qty);
                    }
                    else
                    {
                        qty = Convert.ToInt32(item1.qty);
                    }
                    amount = amount+(Convert.ToDecimal(item1.rate) * qty) + Convert.ToDecimal(item1.gst_amt);
                    
                }
                model.total_amount = amount.ToString();
                model.orderid = item.orderid;
                model.category = item.category;
                model.approvel_1 = item.approvel_1;
                model.approvel_2 = item.approvel_2;
                model.approvel_3 = item.approvel_3;
                model.status = item.status;
                model.rate= item.rate;
                model.total_item = item.total_item;
                model.id = item.id;
                model.orderdate=item.orderdate;
                model.status_name = item.status_name;
                list.Add(model);
            }

            return View(list);
        }

        public async Task<ActionResult> pendingOrder()
        {
            var Result = await _orderRepository.get_User_order_list(Convert.ToInt32(Request.Cookies["userid"].ToString()), Convert.ToInt32(Request.Cookies["clientid"].ToString()), "1");
            return View(Result);
        }

        public async Task<ActionResult> ApprovedOrder()
        {
            var Result = await _orderRepository.get_User_order_list(Convert.ToInt32(Request.Cookies["userid"].ToString()), Convert.ToInt32(Request.Cookies["clientid"].ToString()), "6");
            return View(Result);
        }
        public async Task<ActionResult> edit_order(int id)
        {
            var Result = await _orderRepository.get_order_details(id);
            return View(Result);
        }

        public async Task<ActionResult> Edit_visiting_order(int id)
        {
            var Result = await _orderRepository.get_User_Visitingorder_list(id);
            return View(Result);
        }

        public async Task<ActionResult> ViewOrder(int id)
        {
            order_combo order_Combo = new order_combo();
            order_list_model order_List_Model = new order_list_model();
            List<order_details> order_Details = new List<order_details>();
            var Result = await _orderRepository.getorderlist(0,id);
            var Result1 = await _orderRepository.get_order_details(id);
            foreach (var item in Result)
            {
                order_List_Model.orderid = item.orderid;
                order_List_Model.orderdate = item.orderdate;
                order_List_Model.qty = item.qty;
                order_List_Model.amount = item.amount;
                order_List_Model.billing_address = item.billing_address;
                order_List_Model.shipping_address = item.shipping_address;
                order_List_Model.remark = item.remark;
                order_List_Model.category = item.category;

            }

            foreach (var item in Result1)
            {
                order_details model = new order_details();
                model.ImageUrl = item.ImageUrl;
                if (item.updated_qty != "0")
                { model.qty = item.updated_qty; }
                else { model.qty = item.qty; }
               
                model.rate=item.rate;
                model.hsncode=item.hsncode;
                model.Description = item.Description;
                model.gst_amt = item.gst_amt;
                model.gst_per = item.gst_per;
                decimal gst_amt = Convert.ToDecimal(Convert.ToDecimal(item.rate) * Convert.ToDecimal(item.gst_per) / 100);
                model.total_amount = ((Convert.ToDecimal(item.rate) + Convert.ToDecimal(gst_amt)) * Convert.ToInt32(model.qty)).ToString("0.00");
                order_Details.Add(model);
            }
            order_Combo.order_List_Model = order_List_Model;
            order_Combo.order_list = order_Details;
            return View(order_Combo);
        }

        public async Task<ActionResult> OrderReport()
        {
            var Result = await _orderRepository.get_User_order_list(Convert.ToInt32(Request.Cookies["userid"].ToString()), Convert.ToInt32(Request.Cookies["clientid"].ToString()), "All");
            return View(Result);
        }
        public async Task<ActionResult> OrderWiseReport()
        {
            var Result = await _orderRepository.get_User_order_list(Convert.ToInt32(Request.Cookies["userid"].ToString()), Convert.ToInt32(Request.Cookies["clientid"].ToString()), "All");
            return View(Result);
        }

        public async Task<ActionResult> productWiseReport()
        {
            var Result = await _orderRepository.getorderbyproduct(Convert.ToInt32(Request.Cookies["userid"].ToString()));
            return View(Result);
        }
        public async Task<ActionResult> gstwiseReport()
        {
            var Result = await _orderRepository.getorderbyproduct(Convert.ToInt32(Request.Cookies["userid"].ToString()));
            return View(Result);
        }
        public async Task<ActionResult> stationary()
        {
            combomodel model = new combomodel();
            List<ProductModel> products = new List<ProductModel>();
            List<CartModel> cart = new List<CartModel>();
            var result = await _productrepository.getdataBycategorysync("Stationery", 0);
            foreach (var item in result)
            {
                ProductModel product = new ProductModel();
                product.id = item.id;
                product.Description = item.Description;
                product.Rate = item.Rate;
                product.ImageUrl = item.ImageUrl;
                product.stock = item.stock;
                products.Add(product);
            }

            var result1 = await _cartrepository.getdatasync(Convert.ToInt32(Request.Cookies["userid"].ToString()), Convert.ToInt32(Request.Cookies["clientid"].ToString()));

            foreach (var item in result1)
            {
                CartModel cartModel = new CartModel();
                cartModel.Id = item.Id;
                cartModel.Description = item.Description;
                cartModel.rate = item.rate;
                cartModel.ImageUrl = item.ImageUrl;
                cartModel.qty = item.qty;

                cart.Add(cartModel);
            }
            model.product_Model = products;
            model.cart_Model = cart;
            return View(model);
        }

        public ActionResult FeedBack()
        { 
        return View();
        }

        [HttpPost]
        public async Task<ActionResult> FeedBack(feedbackModel model)
        {
            DynamicParameters ObjParm = new DynamicParameters();
            
            ObjParm.Add("@userid", Convert.ToInt32(Request.Cookies["userid"].ToString()));
            ObjParm.Add("@clientid", Convert.ToInt32(Request.Cookies["clientid"].ToString()));
            ObjParm.Add("@msg", model.msg);
            ObjParm.Add("@rate", model.rate);

            ObjParm.Add("@result", dbType: DbType.Int32, direction: ParameterDirection.Output, size: 5215585);
            await _corpRepository.Addfeedback(ObjParm);
            var id = ObjParm.Get<dynamic>("@result");
            if (id > 0)
            {
                TempData["MsgType"] = "Success";
                TempData["Msg"] = "Thanks For Your Valuable Feedback";
                
            }
            else
            {
                TempData["MsgType"] = "Error";
                TempData["Msg"] = "Something went wrong";
            }
            return Redirect("../User/feedback");
        }

        public async Task<ActionResult> Challan() {
            var result = await _orderRepository.getorderlistforvendor(14,"User", Convert.ToInt32(Request.Cookies["userid"].ToString()));
            return View(result);
        }

        public async Task<ActionResult> PrintChallan(int id)
        {
            StreamReader sr = new StreamReader("Challan/challan.html");
            string s = sr.ReadToEnd();
            order_combo order_Combo = new order_combo();
            order_list_model order_List_Model = new order_list_model();
            List<order_details> order_Details = new List<order_details>();
            var Result = await _orderRepository.getorderlist(0, id);
            var Result1 = await _orderRepository.get_order_details(id);
            foreach (var item in Result)
            {
                order_List_Model.orderid = item.orderid;
                order_List_Model.orderdate = item.orderdate;
                order_List_Model.qty = item.qty;
                order_List_Model.amount = item.amount;
                order_List_Model.billing_address = item.billing_address;
                order_List_Model.shipping_address = item.shipping_address;
                order_List_Model.remark = item.remark;
                order_List_Model.category = item.category;
                order_List_Model.clientname = item.clientname;
                order_List_Model.gst = item.gst;

            }
            string item1 = "";
            int g = 1;
            foreach (var item in Result1)
            {
                order_details model = new order_details();
                model.ImageUrl = item.ImageUrl;
                model.qty = item.qty;
                model.updated_qty = item.updated_qty;
                model.rate = item.rate;
                model.hsncode = item.hsncode;
                model.Description = item.Description;
                model.category = item.category;
                order_Details.Add(model);
                if (Convert.ToInt32(item.updated_qty) > 0)
                {
                    item1 += "<tr><td>" + g + "</td><td>" + item.Description + "</td><td>" + item.updated_qty + "</td><td>PCS</td><td>" + item.rate + "</td></tr>";
                }
                else
                {
                    item1 += "<tr><td>" + g + "</td><td>" + item.Description + "</td><td>" + item.qty + "</td><td>PCS</td><td>" + item.rate + "</td></tr>";
                }

                g = g + 1;
            }
            order_Combo.order_List_Model = order_List_Model;
            order_Combo.order_list = order_Details;

            s = s.Replace("{challanno}", order_List_Model.orderid).Replace("{date}", order_List_Model.orderdate).Replace("{buyername}", order_List_Model.clientname).Replace("{addressline1}", order_List_Model.billing_address).Replace("{consigneename}", order_List_Model.clientname).Replace("{addressline2}", order_List_Model.shipping_address).Replace("{item}", item1).Replace("{gst}", order_List_Model.gst);
            TempData["html"] = s;


            return View();
        }

        public async Task<ActionResult> viewchallan(int id)
        {
            order_combo order_Combo = new order_combo();
            order_list_model order_List_Model = new order_list_model();
            List<order_details> order_Details = new List<order_details>();
           
            var Result = await _orderRepository.getorderlist(0, id);
            var Result1 = await _orderRepository.get_order_details(id);
            var Result2 = await _corpRepository.getShipping(id);
           
            foreach (var item in Result)
            {
                order_List_Model.id = item.id;
                order_List_Model.orderid = item.orderid;
                order_List_Model.orderdate = item.orderdate;
                order_List_Model.qty = item.qty;
                order_List_Model.amount = item.amount;
                order_List_Model.billing_address = item.billing_address;
                order_List_Model.shipping_address = item.shipping_address;
                order_List_Model.remark = item.remark;
                order_List_Model.category = item.category;

            }

            foreach (var item in Result1)
            {
                order_details model = new order_details();
                model.id=item.id;
                model.ImageUrl = item.ImageUrl;
                model.qty = item.qty;
                model.updated_qty = item.updated_qty;
                model.rate = item.rate;
                model.hsncode = item.hsncode;
                model.Description = item.Description;
                model.category = item.category;
                model.is_reject = item.is_reject;
                model.remark = item.remark;

                order_Details.Add(model);
            }
            order_Combo.order_List_Model = order_List_Model;
            order_Combo.order_list = order_Details;
            order_Combo.shipping = Result2;
            return View(order_Combo);
        }


        [HttpGet]
        public async Task<ActionResult> single_reject_challan(int id, int status, string remark)
        {

            DynamicParameters ObjParm = new DynamicParameters();

            ObjParm.Add("@userid", Convert.ToInt32(Request.Cookies["userid"].ToString()));
            ObjParm.Add("@status", status);
            ObjParm.Add("@remark", remark);
            ObjParm.Add("@id", id);
            await _corpRepository.single_reject_challan(ObjParm);

            return Ok("Success");
        }

        [HttpGet]
        public async Task<ActionResult> get_challan_count(int id)
        {
            challan_count order_List_Model = new challan_count();
            var Result = await _corpRepository.get_challan_count(id);
            foreach (var item in Result)
            {
                order_List_Model.reject_count = item.reject_count;
                order_List_Model.sucess_count = item.sucess_count;
                order_List_Model.total_count = item.total_count;
            }
            return Ok(order_List_Model);
        }

        [HttpGet]
        public async Task<ActionResult> All_challan_update(int id,int status, string remark)
        {
            DynamicParameters ObjParm = new DynamicParameters();

            ObjParm.Add("@userid", Convert.ToInt32(Request.Cookies["userid"].ToString()));
            ObjParm.Add("@status", status);
            ObjParm.Add("@remark", remark);
            ObjParm.Add("@id", id);
            await _corpRepository.All_reject_Accept_challan(ObjParm);

            return Ok("Success");
        }
        //========================End========================

        //========Order Wise====================

        [HttpGet]
        public async Task<ActionResult> ExportToExcel_order(int id = 0, string barcode = "", int supplierid = 0, int qty = 0, int type = 0, int catgeoryid = 0)
        {
            var Result = await _orderRepository.get_User_order_list(Convert.ToInt32(Request.Cookies["userid"].ToString()), Convert.ToInt32(Request.Cookies["clientid"].ToString()), "All");
            //exportfile(result.ToList());
            DataTable dt = new DataTable();
            dt.Columns.Add("Order Id", typeof(string));
            dt.Columns.Add("Order Date", typeof(string));
            dt.Columns.Add("Product", typeof(string));
            dt.Columns.Add("Category", typeof(string));
            dt.Columns.Add("Qty", typeof(string));
            foreach (var item in Result)
            {

                dt.Rows.Add(item.orderid, item.orderdate, item.Description,item.category,item.qty);


            }
            string base64String;

            using (var wb = new XLWorkbook())
            {
                var sheet = wb.AddWorksheet(dt, "Order Wise Report");

                // Apply font color to columns 1 to 5
                sheet.Columns(1, 8).Style.Font.FontColor = XLColor.Black;

                using (var ms = new MemoryStream())
                {
                    wb.SaveAs(ms);

                    // Convert the Excel workbook to a base64-encoded string
                    base64String = Convert.ToBase64String(ms.ToArray());
                }
            }

            // Return a CreatedResult with the base64-encoded Excel data
            return new CreatedResult(string.Empty, new
            {
                Code = 200,
                Status = true,
                Message = "",
                Data = base64String
            });

        }

        public void exportfile_order(List<OrderModel> model)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("OrderWise Report");
                var currentRow = 1;
                worksheet.Cell(currentRow, 1).Value = "Order Id";
                worksheet.Cell(currentRow, 2).Value = "Order Date";
                worksheet.Cell(currentRow, 3).Value = "Product";
                worksheet.Cell(currentRow, 4).Value = "Category";
                worksheet.Cell(currentRow, 5).Value = "QTY";

                foreach (var item in model)
                {
                    {
                        currentRow++;
                        worksheet.Cell(currentRow, 1).Value = item.orderid;
                        worksheet.Cell(currentRow, 2).Value = item.orderdate;
                        worksheet.Cell(currentRow, 3).Value = item.Description;
                        worksheet.Cell(currentRow, 4).Value = item.category;
                        worksheet.Cell(currentRow, 5).Value = item.qty;



                    }
                }
                using var stream = new MemoryStream();
                workbook.SaveAs(stream);
                var content = stream.ToArray();
                Response.Clear();
                Response.Headers.Add("content-disposition", "attachment;filename=OrderWiseReport.xls");
                Response.ContentType = "application/xls";
                Response.Body.WriteAsync(content);
                //Response.Body.Flush();
                // return Redirect("../Report/StockReport");
            }




        }

        //==============End========================


        //========Product Wise====================

        [HttpGet]
        public async Task<ActionResult> ExportToExcel(int id = 0, string barcode = "", int supplierid = 0, int qty = 0, int type = 0, int catgeoryid = 0)
        {
            var Result = await _orderRepository.getorderbyproduct(Convert.ToInt32(Request.Cookies["userid"].ToString()));
            //exportfile(result.ToList());
            DataTable dt = new DataTable();
            dt.Columns.Add("Order Id", typeof(string));
            dt.Columns.Add("Product Name", typeof(string));
            dt.Columns.Add("Product Category", typeof(string));
            dt.Columns.Add("Qty", typeof(string));
            dt.Columns.Add("Date", typeof(string));
            foreach (var item in Result)
            {

                dt.Rows.Add(item.orderid, item.name, item.category,item.qty,item.orderdate);


            }
            string base64String;

            using (var wb = new XLWorkbook())
            {
                var sheet = wb.AddWorksheet(dt, "Product Wise Report");

                // Apply font color to columns 1 to 5
                sheet.Columns(1, 8).Style.Font.FontColor = XLColor.Black;

                using (var ms = new MemoryStream())
                {
                    wb.SaveAs(ms);

                    // Convert the Excel workbook to a base64-encoded string
                    base64String = Convert.ToBase64String(ms.ToArray());
                }
            }

            // Return a CreatedResult with the base64-encoded Excel data
            return new CreatedResult(string.Empty, new
            {
                Code = 200,
                Status = true,
                Message = "",
                Data = base64String
            });

        }

        public void exportfile(List<OrderModel> model)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("ProductWise Report");
                var currentRow = 1;
                worksheet.Cell(currentRow, 1).Value = "Order Id";
                worksheet.Cell(currentRow, 2).Value = "Product Name";
                worksheet.Cell(currentRow, 3).Value = "Product Category";
                worksheet.Cell(currentRow, 4).Value = "Qty";
                worksheet.Cell(currentRow, 5).Value = "Date";

                foreach (var item in model)
                {
                    {
                        currentRow++;
                        worksheet.Cell(currentRow, 1).Value = item.orderid;
                        worksheet.Cell(currentRow, 2).Value = item.name;
                        worksheet.Cell(currentRow, 3).Value = item.category;
                        worksheet.Cell(currentRow, 4).Value = item.qty;
                        worksheet.Cell(currentRow, 5).Value = item.orderdate;


                    }
                }
                using var stream = new MemoryStream();
                workbook.SaveAs(stream);
                var content = stream.ToArray();
                Response.Clear();
                Response.Headers.Add("content-disposition", "attachment;filename=ProductWiseReport.xls");
                Response.ContentType = "application/xls";
                Response.Body.WriteAsync(content);
                //Response.Body.Flush();
                // return Redirect("../Report/StockReport");
            }




        }

        //==============End========================

        //============Html to PDF====================

        public async Task<ActionResult> ExportPdf_product()
        {
            StreamReader sr = new StreamReader("challan/productwisereport.html");
            string s = sr.ReadToEnd();
            var Result = await _orderRepository.getorderbyproduct(Convert.ToInt32(Request.Cookies["userid"].ToString()));
            string html = "";
            string items = "";
            int i = 1;
            foreach (var item in Result)
            {
                items += "<tr>" +
                    "<td style=\"border: 1px solid #dddddd; text-align: left; padding: 8px;\">" + i+"</td>"+
                    "<td style=\"border: 1px solid #dddddd; text-align: left; padding: 8px;\">" + item.orderid + "</td>" +
                    "<td style=\"border: 1px solid #dddddd; text-align: left; padding: 8px;\">" + item.name + "</td>" +
                     "<td style=\"border: 1px solid #dddddd; text-align: left; padding: 8px;\">" + item.category + "</td>" +
                     "<td style=\"border: 1px solid #dddddd; text-align: left; padding: 8px;\">" + item.qty + "</td>" +
                      "<td style=\"border: 1px solid #dddddd; text-align: left; padding: 8px;\">" + item.orderdate + "</td>" +
                    "</tr>";
             

                i = i + 1;
            }
            html += s.Replace("{item}", items);
            StringBuilder sb = new StringBuilder();
            sb.Append(html);
            StringReader sr1 = new StringReader(sb.ToString());
            //Rectangle envelope = new Rectangle(141, 107);
            Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 10f, 10f);
            HTMLWorker htmlparser = new HTMLWorker(pdfDoc);
            string base64String;
            using (MemoryStream memoryStream = new MemoryStream())
            {
               // var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/pdf", "producwisereport.pdf");
                 PdfWriter writer = PdfWriter.GetInstance(pdfDoc, memoryStream);
                //PdfWriter.GetInstance(pdfDoc, new FileStream(filePath, FileMode.Create));
                pdfDoc.Open();

                htmlparser.Parse(sr1);
                pdfDoc.Close();

                byte[] bytes = memoryStream.ToArray();
                base64String = Convert.ToBase64String(memoryStream.ToArray());
                return new CreatedResult(string.Empty, new
                {
                    Code = 200,
                    Status = true,
                    Message = "",
                    Data = base64String
                });
            }
           
        }

        public async Task<ActionResult> ExportPdf_order()
        {
            try
            {

                StreamReader sr = new StreamReader("Challan/OrderReport.html");
                string s = sr.ReadToEnd();
                var Result = await _orderRepository.get_User_order_list(Convert.ToInt32(Request.Cookies["userid"].ToString()), Convert.ToInt32(Request.Cookies["clientid"].ToString()), "All");
                string html = "";
                string items = "";
                int i = 1;
                foreach (var item in Result)
                {
                    items += "<tr>" +
                        "<td style=\"border: 1px solid #dddddd; text-align: left; padding: 8px;\">" + i + "</td>" +
                        "<td style=\"border: 1px solid #dddddd; text-align: left; padding: 8px;\">" + item.orderid + "</td>" +
                        "<td style=\"border: 1px solid #dddddd; text-align: left; padding: 8px;\">" + item.name + "</td>" +
                         "<td style=\"border: 1px solid #dddddd; text-align: left; padding: 8px;\">" + item.category + "</td>" +
                         "<td style=\"border: 1px solid #dddddd; text-align: left; padding: 8px;\">" + item.qty + "</td>" +
                          "<td style=\"border: 1px solid #dddddd; text-align: left; padding: 8px;\">" + item.orderdate + "</td>" +
                        "</tr>";


                    i = i + 1;
                }
                html += s.Replace("{item}", items);
                StringBuilder sb = new StringBuilder();
                sb.Append(html);
                StringReader sr1 = new StringReader(sb.ToString());
                //Rectangle envelope = new Rectangle(141, 107);
                Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 10f, 10f);
                HTMLWorker htmlparser = new HTMLWorker(pdfDoc);
                string base64String;
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    //var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/pdf", "producwisereport.pdf");
                    PdfWriter writer = PdfWriter.GetInstance(pdfDoc, memoryStream);
                    //PdfWriter.GetInstance(pdfDoc, new FileStream(filePath, FileMode.Create));
                    pdfDoc.Open();

                    htmlparser.Parse(sr1);
                    pdfDoc.Close();

                    byte[] bytes = memoryStream.ToArray();
                    base64String = Convert.ToBase64String(memoryStream.ToArray());
                    return new CreatedResult(string.Empty, new
                    {
                        Code = 200,
                        Status = true,
                        Message = "",
                        Data = base64String
                    });
                }
            }
            catch (Exception ex)
            {
                return new CreatedResult(string.Empty, new
                {
                    Code = 200,
                    Status = true,
                    Message =ex.Message,
                    Data = ""
                });
               
            }

        }
        //===================End=====================
    }
}
