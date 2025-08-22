using ClosedXML.Excel;
using Dapper;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Mvc;
using OTS.Data.Models;
using OTS.Data.Repository;
using System.Data;
using System.Text;

namespace OTS.UI.Controllers
{
    public class ClientController : Controller
    {
        private readonly Iproductrepository _productrepository;
        private readonly IcartRepository _cartrepository;
        private readonly IClientRepository _clientRepository;
        private readonly ICostRepository _costRepository;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IBranchRepository _branchRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IVendorRepository _vendorRepository;
        private readonly ICorpRepository _corpRepository;

        public ClientController(Iproductrepository productrepository, ICorpRepository corpRepository, IcartRepository cartrepository, IClientRepository clientRepository, ICostRepository costRepository, IDepartmentRepository departmentRepository, IBranchRepository branchRepository,IOrderRepository orderRepository,IVendorRepository vendorRepository)
        {
            _productrepository = productrepository;
            _corpRepository = corpRepository;
            _cartrepository = cartrepository;
            _clientRepository = clientRepository;
            _costRepository = costRepository;
            _departmentRepository = departmentRepository;
            _branchRepository = branchRepository;
            _orderRepository = orderRepository;
            _vendorRepository = vendorRepository;
        }
        public async Task<ActionResult> Index()
        {
            if (Request.Cookies["Userid"] != null)
            {
                int userid = Convert.ToInt32(Request.Cookies["Userid"].ToString());
                int clientid = Convert.ToInt32(Request.Cookies["clientid"].ToString());
                string u = Request.Cookies["Usertype"].ToString();
                CorpUserModel user = new CorpUserModel();
                user = await _corpRepository.getdataByIdsync(Convert.ToInt32(Request.Cookies["userid"].ToString()));
                if (u == "C")
                {
                    if (user.approval_type == 1)
                    {
                        var Result = await _orderRepository.getorderlist(2, 0, "", "Client", userid);
                        ViewBag.approval_type = user.approval_type;
                        return View(Result);
                    }
                    else
                    {
                        var Result = await _orderRepository.getorderlist(2, 0, "", "Client", userid);
                        ViewBag.approval_type = user.approval_type;
                        return View(Result);
                    }

                }
                else
                {
                    var Result = await _orderRepository.getorderlist(1, 0, "", "User", userid);
                    ViewBag.approval_type = user.approval_type;
                    return View(Result);
                }
            }
            else
            {
                return Redirect("../Account/Login");
            }
            /*combomodel model = new combomodel();
            var result = await _vendorRepository.getclientdash(Convert.ToInt32(Request.Cookies["clientid"].ToString()));
            foreach (var item in result)
            {
                ViewBag.product_uploaded = item.total_product.ToString();
                ViewBag.complete_order = item.complete_order.ToString();
                ViewBag.icompelete_order = item.incomplete_order.ToString();
            }

            return View();*/
        }
        public ActionResult about()
        {
            return View();
        }
        public ActionResult add_vendor()
        {
            return View();
        }

        public string delete_tocart(int id)
        {
            _cartrepository.DeleteAsync(id);
            return "Record Deleted Successfully";
        }

        public async Task<ActionResult> ViewOrder(int id)
        {
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

            }

            foreach (var item in Result1)
            {
                order_details model = new order_details();
                model.ImageUrl = item.ImageUrl;
                if (item.updated_qty!="0")
                {
                    model.qty = item.updated_qty;
                }
                else {
                    model.qty = item.qty;
                }
                
                model.rate = item.rate;
                model.gst_amt = item.gst_amt;
                model.gst_per = item.gst_per;
                model.hsncode = item.hsncode;
                model.Description = item.Description;
                model.category = item.category;

                order_Details.Add(model);
            }
            order_Combo.order_List_Model = order_List_Model;
            order_Combo.order_list = order_Details;
            return View(order_Combo);
        }
        public async Task<ActionResult> All_Order(string f="",string t="")
        {
            if (f == "")
            {
                f = DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd");
            }
            if (t == "")
            {
                t = DateTime.Now.ToString("yyyy-MM-dd");
            }
            int userid = Convert.ToInt32(Request.Cookies["Userid"].ToString());
            int clientid = Convert.ToInt32(Request.Cookies["clientid"].ToString());
            var Result = await _orderRepository.getorderlist(userid, clientid, "All","Client",0,f,t);
            return View(Result);
        }

        [HttpGet]
        public async Task<ActionResult> ExportToExcel_AllOrder(string f = "", string t = "")
        {
            int userid = Convert.ToInt32(Request.Cookies["Userid"].ToString());
            int clientid = Convert.ToInt32(Request.Cookies["clientid"].ToString());
            var result = await _orderRepository.getorderlist(userid, clientid, "All", "Client", 0, f, t);
            //exportfile(result.ToList());
            DataTable dt = new DataTable();
            dt.Columns.Add("Order Id", typeof(string));
            dt.Columns.Add("Qty", typeof(string));
            dt.Columns.Add("Department", typeof(string));
            dt.Columns.Add("Order Status", typeof(string));
            dt.Columns.Add("Amount", typeof(string));
            dt.Columns.Add("Date", typeof(string));

            foreach (var item in result)
            {

                dt.Rows.Add(item.orderid, item.qty, item.department, item.status, item.amount, item.orderdate);


            }
            string base64String;

            using (var wb = new XLWorkbook())
            {
                var sheet = wb.AddWorksheet(dt, "AllOrder");

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

        public async Task<ActionResult> ExportPdf_Allorder(string f = "", string t = "")
        {
            try
            {
                int userid = Convert.ToInt32(Request.Cookies["Userid"].ToString());
                int clientid = Convert.ToInt32(Request.Cookies["clientid"].ToString());
                if (f == "")
                {
                    f = DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd");
                }
                if (t == "")
                {
                    t = DateTime.Now.ToString("yyyy-MM-dd");
                }
                StreamReader sr = new StreamReader("Challan/htmlpage.html");
                string s = sr.ReadToEnd();
                var result = await _orderRepository.getorderlist(userid, clientid, "All", "Client", 0, f, t);
                string html = "";
                string items = "";
                int i = 1;
                foreach (var item in result)
                {
                    items += "<tr>" +
                        "<td style=\"border: 1px solid #dddddd; text-align: left; padding: 8px;\">" + i + "</td>" +
                        "<td style=\"border: 1px solid #dddddd; text-align: left; padding: 8px;\">" + item.orderid + "</td>" +
                        "<td style=\"border: 1px solid #dddddd; text-align: left; padding: 8px;\">" + item.qty + "</td>" +
                         "<td style=\"border: 1px solid #dddddd; text-align: left; padding: 8px;\">" + item.department + "</td>" +
                         "<td style=\"border: 1px solid #dddddd; text-align: left; padding: 8px;\">" + item.status + "</td>" +
                          "<td style=\"border: 1px solid #dddddd; text-align: left; padding: 8px;\">" + item.amount + "</td>" +
                           "<td style=\"border: 1px solid #dddddd; text-align: left; padding: 8px;\">" + item.orderdate + "</td>" +
                        "</tr>";


                    i = i + 1;
                }
                s = s.Replace("{report}", "All Order");
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
                    Message = ex.Message,
                    Data = ""
                });

            }

        }

        public async Task<ActionResult> New_Order()
        {
            if (Request.Cookies["Userid"] != null)
            {
                int userid = Convert.ToInt32(Request.Cookies["Userid"].ToString());
                int clientid = Convert.ToInt32(Request.Cookies["clientid"].ToString());
                string u = Request.Cookies["Usertype"].ToString();
                CorpUserModel user = new CorpUserModel();
                user = await _corpRepository.getdataByIdsync(Convert.ToInt32(Request.Cookies["userid"].ToString()));
                if (u == "C")
                {
                    if (user.approval_type == 1)
                    {
                        var Result = await _orderRepository.getorderlist(4, 0, "", "Client", userid);
                        ViewBag.approval_type = user.approval_type;
                        return View(Result);
                    }
                    else
                    {
                        var Result = await _orderRepository.getorderlist(3, 0, "", "Client", userid);
                        ViewBag.approval_type = user.approval_type;
                        return View(Result);
                    }
                }
                else
                {
                    /*var Result = await _orderRepository.getorderlist(1, 0, "", "User", userid);
                    ViewBag.approval_type = user.approval_type;
                    return View(Result);*/
                    if (user.approval_type == 3)
                    {
                        var Result = await _orderRepository.getorderlist(2, 0, "", "Client", userid);
                        ViewBag.approval_type = user.approval_type;
                        return View(Result);
                    }
<<<<<<< HEAD

                }
                else
                {
                    /*var Result = await _orderRepository.getorderlist(1, 0, "", "User", userid);
                    ViewBag.approval_type = user.approval_type;
                    return View(Result);*/
                    if (user.approval_type == 3)
=======
                    else if(user.approval_type == 4)
>>>>>>> 44278b0ea5548a6d38c04ab8ea96302cff6ac0b4
                    {
                        var Result = await _orderRepository.getorderlist(1, 0, "", "Client", userid);
                        ViewBag.approval_type = user.approval_type;
                        return View(Result);
                    }
                    else
                    {
<<<<<<< HEAD
                        var Result = await _orderRepository.getorderlist(2, 0, "", "Client", userid);
=======
                        var Result = await _orderRepository.getorderlist(3, 0, "", "Client", userid);
>>>>>>> 44278b0ea5548a6d38c04ab8ea96302cff6ac0b4
                        ViewBag.approval_type = user.approval_type;
                        return View(Result);
                    }
                }
            }
            else 
            {
                return Redirect("../Account/Login");
            }
        }

        public async Task<ActionResult> Edit_orer(int id)
        {
            var Result = await _orderRepository.get_order_details(id);
            return View(Result);
        }

        public async Task<ActionResult> Approved_Order()
        {
            string f = DateTime.Now.AddDays(-30).ToString("dd-MM-yyyy");
            string t = DateTime.Now.ToString("dd-MM-yyyy");
            int userid = Convert.ToInt32(Request.Cookies["Userid"].ToString());
            int clientid = Convert.ToInt32(Request.Cookies["clientid"].ToString());
            string u = Request.Cookies["Usertype"].ToString();
            if (u == "C")
            {
                var Result = await _orderRepository.getorderlist(6, 0, "","Client",0);
                return View(Result);
            }
            else {
                var Result = await _orderRepository.getorderlist(6, 0, "","User",0,f,t);
                return View(Result);
            }
        }

        [HttpGet]
        public async Task<ActionResult> ExportToExcel_ApprovedOrder(string f = "", string t = "")
        {
            int userid = Convert.ToInt32(Request.Cookies["Userid"].ToString());
            int clientid = Convert.ToInt32(Request.Cookies["clientid"].ToString());
            var result = await _orderRepository.getorderlist(6, 0, "", "User", 0, f, t);
            //exportfile(result.ToList());
            DataTable dt = new DataTable();
            dt.Columns.Add("Order Id", typeof(string));
            dt.Columns.Add("Qty", typeof(string));
            dt.Columns.Add("Department", typeof(string));
            dt.Columns.Add("Order Status", typeof(string));
            dt.Columns.Add("Amount", typeof(string));
            dt.Columns.Add("Date", typeof(string));

            foreach (var item in result)
            {

                dt.Rows.Add(item.orderid, item.qty, item.department, item.status, item.amount, item.orderdate);


            }
            string base64String;

            using (var wb = new XLWorkbook())
            {
                var sheet = wb.AddWorksheet(dt, "ApprovedOrder");

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

        public async Task<ActionResult> ExportPdf_ApprovedOrder(string f = "", string t = "")
        {
            try
            {
                int userid = Convert.ToInt32(Request.Cookies["Userid"].ToString());
                int clientid = Convert.ToInt32(Request.Cookies["clientid"].ToString());
                if (f == "")
                {
                    f = DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd");
                }
                if (t == "")
                {
                    t = DateTime.Now.ToString("yyyy-MM-dd");
                }
                StreamReader sr = new StreamReader("Challan/htmlpage.html");
                string s = sr.ReadToEnd();
                var result = await _orderRepository.getorderlist(6, 0, "", "User", 0, f, t);
                string html = "";
                string items = "";
                int i = 1;
                foreach (var item in result)
                {
                    items += "<tr>" +
                        "<td style=\"border: 1px solid #dddddd; text-align: left; padding: 8px;\">" + i + "</td>" +
                        "<td style=\"border: 1px solid #dddddd; text-align: left; padding: 8px;\">" + item.orderid + "</td>" +
                        "<td style=\"border: 1px solid #dddddd; text-align: left; padding: 8px;\">" + item.qty + "</td>" +
                         "<td style=\"border: 1px solid #dddddd; text-align: left; padding: 8px;\">" + item.department + "</td>" +
                         "<td style=\"border: 1px solid #dddddd; text-align: left; padding: 8px;\">" + item.status + "</td>" +
                          "<td style=\"border: 1px solid #dddddd; text-align: left; padding: 8px;\">" + item.amount + "</td>" +
                           "<td style=\"border: 1px solid #dddddd; text-align: left; padding: 8px;\">" + item.orderdate + "</td>" +
                        "</tr>";


                    i = i + 1;
                }
                s = s.Replace("{report}", "Approved Order");
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
                    Message = ex.Message,
                    Data = ""
                });

            }

        }
        public async Task<ActionResult> User(int id=0)
        {
            if (id == 0)
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

                //List<ClientModel> list = new List<ClientModel>();

                //var result = await _clientRepository.getdatasync(Convert.ToInt32(Request.Cookies["userid"].ToString()));
                //foreach (var item in result)
                //{
                //    ClientModel model = new ClientModel();
                //    model.Id = item.Id;
                //    model.Name = item.Name;
                //    list.Add(model);
                //}


                List<CostCenter> costlist = new List<CostCenter>();
                var result1 = await _costRepository.getdatasync();
                foreach (var item in result1)
                {
                    CostCenter model = new CostCenter();
                    model.Id = item.Id;
                    model.Name = item.Name;
                    costlist.Add(model);
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

                List<BranchModel> branchlist = new List<BranchModel>();
                var result5 = await _branchRepository.getdatasync();
                foreach (var item in result4)
                {
                    BranchModel model = new BranchModel();
                    model.Id = item.Id;
                    model.Name = item.Name;

                    branchlist.Add(model);
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
                CorpUserModel corpmodel = new CorpUserModel();
                combolist.CorpUser_Models = corpmodel;

                combolist.VendorModels = list;
                combolist.costCenters = costlist;
                combolist.departmentModels = departmentlist;
                combolist.branchModels = branchlist;
                combolist.CorpUserModels = corptlist;
                combolist.UserTypeModels = usertypetlist;
                return View(combolist);
            }
            else
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


                //List<ClientModel> list = new List<ClientModel>();

                //var result = await _clientRepository.getdatasync();
                //foreach (var item in result)
                //{
                //    ClientModel model = new ClientModel();
                //    model.Id = item.Id;
                //    model.Name = item.Name;
                //    list.Add(model);
                //}


                List<CostCenter> costlist = new List<CostCenter>();
                var result1 = await _costRepository.getdatasync();
                foreach (var item in result1)
                {
                    CostCenter model = new CostCenter();
                    model.Id = item.Id;
                    model.Name = item.Name;
                    costlist.Add(model);
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

                List<BranchModel> branchlist = new List<BranchModel>();
                var result5 = await _branchRepository.getdatasync();
                foreach (var item in result4)
                {
                    BranchModel model = new BranchModel();
                    model.Id = item.Id;
                    model.Name = item.Name;

                    branchlist.Add(model);
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

                combolist.VendorModels = list;
                combolist.costCenters = costlist;
                combolist.departmentModels = departmentlist;
                combolist.branchModels = branchlist;
                combolist.CorpUserModels = corptlist;
                combolist.UserTypeModels = usertypetlist;
                CorpUserModel corpmodel = new CorpUserModel();
                corpmodel = await _corpRepository.getdataByIdsync(id);
                combolist.CorpUser_Models = corpmodel;
                return View(combolist);
            }
        }
        public async Task<ActionResult> ViewUser()
        {
            var result = await _corpRepository.getdatasync();
            return View(result);
        }
        public async Task<ActionResult> Stationary()
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

        public async Task<ActionResult> Housekeeping()
        {
            combomodel model = new combomodel();
            List<ProductModel> products = new List<ProductModel>();
            List<CartModel> cart = new List<CartModel>();
            var result = await _productrepository.getdataBycategorysync("Houskeeping", 0);
            foreach (var item in result)
            {
                ProductModel product = new ProductModel();
                product.id = item.id;
                product.Description = item.Description;
                product.Rate = item.Rate;
                product.ImageUrl = item.ImageUrl;
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
        public async Task<ActionResult> MisReportBranchWise()
        {
            var Result = await _orderRepository.getorderbybranch();
            return View(Result);
        }
        public async Task<ActionResult> MisReportCostCenterWise()
        {
           return View();
        }
        public async Task<ActionResult> MisReportDepartmentrWise(string f="",string t="")
        {
            if (f == "")
            {
                f = DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd");
            }
            if (t == "")
            {
                t = DateTime.Now.ToString("yyyy-MM-dd");
            }
            var Result = await _orderRepository.getorderbydepartment(f,t);
            return View(Result);
        }
        public async Task<ActionResult> MisReportOrderWise(string f="",string t="",string o="")
        {
            if (f == "")
            {
                f = DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd");
            }
            if (t == "")
            {
                t = DateTime.Now.ToString("yyyy-MM-dd");
            }
            var Result = await _orderRepository.getorderlist(0, 0, "All","",0,f,t);
            return View(Result);
        }
        public async Task<ActionResult> MisReportProductWise(string f = "", string t = "", string o = "")
        {
            if (f == "")
            {
                f = DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd");
            }
            if (t == "")
            {
                t = DateTime.Now.ToString("yyyy-MM-dd");
            }
            var Result = await _orderRepository.getorderbyproduct(0,f,t);
            return View(Result);
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
                cartModel.HSN = item.HSN;
                cart.Add(cartModel);
            }

            model.cart_Model = cart;
            model.CorpUser_Models = user;
            return View(model);
        }
        public ActionResult RunTimeOrder()
        {
            return View();
        }

        public async Task<ActionResult> ViewAgreement()
        {
            var result = await _vendorRepository.getagreement(Convert.ToInt32(Request.Cookies["userid"].ToString()));
            return View(result);
        }

        public async Task<ActionResult> Master(int id = 0, string t = "")
        {
            combomodel combomodel = new combomodel();
            List<CostCenter> costlist = new List<CostCenter>();
            List<DepartmentModel> DepartmentList = new List<DepartmentModel>();
            List<BranchModel> branchlist = new List<BranchModel>();
            CostCenter costCenter = new CostCenter();
            DepartmentModel Department = new DepartmentModel();
            BranchModel branch = new BranchModel();
            if (id == 0)
            {
                var cost = await _costRepository.getdatasync();
                if (cost.Count() > 0)
                {
                    foreach (var item in cost)
                    {
                        CostCenter costModel = new CostCenter();
                        costModel.Name = item.Name;
                        costModel.Id = item.Id;
                        costModel.createdon = item.createdon;
                        costModel.createdby = item.createdby;
                        costlist.Add(costModel);
                    }


                }
                combomodel.costCenters = costlist;
                combomodel.costCenterModel = costCenter;

                var departments = await _departmentRepository.getdatasync();
                if (departments.Count() > 0)
                {
                    foreach (var item in departments)
                    {
                        DepartmentModel departmentModel = new DepartmentModel();
                        departmentModel.Name = item.Name;
                        departmentModel.Id = item.Id;
                        departmentModel.createdon = item.createdon;
                        departmentModel.createdby = item.createdby;
                        DepartmentList.Add(departmentModel);
                    }


                }

                combomodel.departmentModel = Department;
                combomodel.departmentModels = DepartmentList;

                var branchdata = await _branchRepository.getdatasync();
                if (branchdata.Count() > 0)
                {
                    foreach (var item in branchdata)
                    {
                        BranchModel branchModel = new BranchModel();
                        branchModel.Name = item.Name;
                        branchModel.Id = item.Id;
                        branchModel.createdon = item.createdon;
                        branchModel.createdby = item.createdby;
                        branchlist.Add(branchModel);
                    }


                }

                combomodel.branchModel = branch;
                combomodel.branchModels = branchlist;
            }
            else
            {
                var cost = await _costRepository.getdatasync();
                if (cost.Count() > 0)
                {
                    foreach (var item in cost)
                    {
                        CostCenter costModel = new CostCenter();
                        costModel.Name = item.Name;
                        costModel.Id = item.Id;
                        costModel.createdon = item.createdon;
                        costModel.createdby = item.createdby;
                        costlist.Add(costModel);
                    }


                }
                combomodel.costCenters = costlist;

                var departments = await _departmentRepository.getdatasync();
                if (departments.Count() > 0)
                {
                    foreach (var item in departments)
                    {
                        DepartmentModel departmentModel = new DepartmentModel();
                        departmentModel.Name = item.Name;
                        departmentModel.Id = item.Id;
                        departmentModel.createdon = item.createdon;
                        departmentModel.createdby = item.createdby;
                        DepartmentList.Add(departmentModel);
                    }


                }

                combomodel.departmentModel = Department;
                combomodel.departmentModels = DepartmentList;


                var branchdata = await _branchRepository.getdatasync();
                if (branchdata.Count() > 0)
                {
                    foreach (var item in branchdata)
                    {
                        BranchModel branchModel = new BranchModel();
                        branchModel.Name = item.Name;
                        branchModel.Id = item.Id;
                        branchModel.createdon = item.createdon;
                        branchModel.createdby = item.createdby;
                        branchlist.Add(branchModel);
                    }


                }


                combomodel.branchModels = branchlist;

                if (t == "C")
                {
                    var result = await _costRepository.getdataByIdsync(id);
                    combomodel.costCenterModel = result;
                    combomodel.departmentModel = Department;
                    combomodel.branchModel = branch;
                }


                if (t == "D")
                {
                    var result = await _departmentRepository.getdataByIdsync(id);
                    combomodel.departmentModel = result;
                    combomodel.costCenterModel = costCenter;
                    combomodel.branchModel = branch;
                }

                if (t == "B")
                {
                    var result = await _branchRepository.getdataByIdsync(id);
                    combomodel.branchModel = result;
                    combomodel.costCenterModel = costCenter;
                    combomodel.departmentModel = Department;
                }
            }
            return View(combomodel);
        }

        public ActionResult Approve_challan()
        {
            return View();
        }

        public async Task<ActionResult> Visitingcard()
        {
            var result = await _productrepository.getdataBycategorysync("Visiting Card", 0);
            return View(result);
        }

        public async Task<ActionResult> Visiting_form(int id)
        {
            TempData["productid"] = id;
            return View();
        }

        public async Task<ActionResult> Printing()
        {
            return View();
        }

        
        public async Task<ActionResult> UpdateStatus(int oid,int status)
        {
            try
            {
                
                    DynamicParameters ObjParm = new DynamicParameters();
                    ObjParm.Add("@id", oid);
                    ObjParm.Add("@status", status);


                    await _orderRepository.updatestatus(ObjParm);
                    
                   
                        TempData["MsgType"] = "Success";
                        
                        TempData["Msg"] = "Record Saved Successfully";
                    
                return Redirect("../Client/New_order");
            }
            catch (Exception ex)
            {
                TempData["MsgType"] = "Error";
                TempData["Msg"] = ex.Message;
                return Redirect("../Client/New_order");
            }
        }

        //========Product Wise====================

        [HttpGet]
        public async Task<ActionResult> ExportToExcel(int id = 0, string barcode = "", int supplierid = 0, int qty = 0, int type = 0, int catgeoryid = 0, string f = "", string t = "")
        {
            if (f == "")
            {
                f = DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd");
            }
            if (t == "")
            {
                t = DateTime.Now.ToString("yyyy-MM-dd");
            }
            var result = await _orderRepository.getorderbyproduct(0, f, t);
            //exportfile(result.ToList());
            DataTable dt = new DataTable();
            dt.Columns.Add("Order Id", typeof(string));
            dt.Columns.Add("Product Name", typeof(string));
            dt.Columns.Add("Name", typeof(string));
            dt.Columns.Add("Product Category", typeof(string));
            dt.Columns.Add("GST (%)", typeof(string));
            dt.Columns.Add("HSN Code", typeof(string));
            dt.Columns.Add("Qty", typeof(string));
            dt.Columns.Add("MRP", typeof(string));
            dt.Columns.Add("Date", typeof(string));
            foreach (var item in result)
            {

                dt.Rows.Add(item.orderid, item.Description,item.name, item.category, item.gst, item.hsn, item.qty, item.rate, item.orderdate);


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
                worksheet.Cell(currentRow, 4).Value = "GST (%)";
                worksheet.Cell(currentRow, 5).Value = "HSN Code";
                worksheet.Cell(currentRow, 6).Value = "Qty";
                worksheet.Cell(currentRow, 7).Value = "MRP";
                worksheet.Cell(currentRow, 8).Value = "Date";

                foreach (var item in model)
                {
                    {
                        currentRow++;
                        worksheet.Cell(currentRow, 1).Value = item.orderid;
                        worksheet.Cell(currentRow, 2).Value = item.name;
                        worksheet.Cell(currentRow, 3).Value = item.category;
                        worksheet.Cell(currentRow, 4).Value = item.gst;
                        worksheet.Cell(currentRow, 5).Value = item.hsn;
                        worksheet.Cell(currentRow, 6).Value = item.qty;
                        worksheet.Cell(currentRow, 7).Value = item.rate;
                        worksheet.Cell(currentRow, 8).Value = item.orderdate;


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

        public async Task<ActionResult> ExportPdf_product(string f = "", string t = "")
        {
            try
            {
                if (f == "")
                {
                    f = DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd");
                }
                if (t == "")
                {
                    t = DateTime.Now.ToString("yyyy-MM-dd");
                }
                StreamReader sr = new StreamReader("Challan/OrderReport.html");
                string s = sr.ReadToEnd();
                var result = await _orderRepository.getorderbyproduct(0, f, t);
                string html = "";
                string items = "";
                int i = 1;
                foreach (var item in result)
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
                s = s.Replace("{report}", "Producttwise Report");
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
                    Message = ex.Message,
                    Data = ""
                });

            }

        }

        //==============End========================

        //========Order Wise====================

        [HttpGet]
        public async Task<ActionResult> ExportToExcel_order(int id = 0, string barcode = "", int supplierid = 0, int qty = 0, int type = 0, int catgeoryid = 0,string f="",string t="")
        {
            var Result = await _orderRepository.getorderlist(0, 0, "All","",0,f,t);
            //exportfile(result.ToList());
            DataTable dt = new DataTable();
            dt.Columns.Add("Order Id", typeof(string));
            dt.Columns.Add("Order Date", typeof(string));
            dt.Columns.Add("MRP", typeof(string));
            dt.Columns.Add("QTY", typeof(string));

            foreach (var item in Result)
            {

                dt.Rows.Add(item.orderid, item.orderdate, item.amount, item.qty);


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

        public void exportfile_order(List<order_list_model> model)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("OrderWise Report");
                var currentRow = 1;
                worksheet.Cell(currentRow, 1).Value = "Order Id";
                worksheet.Cell(currentRow, 2).Value = "Order Date";
                worksheet.Cell(currentRow, 3).Value = "MRP";
                worksheet.Cell(currentRow, 4).Value = "QTY";


                foreach (var item in model)
                {
                    {
                        currentRow++;
                        worksheet.Cell(currentRow, 1).Value = item.orderid;
                        worksheet.Cell(currentRow, 2).Value = item.orderdate;
                        worksheet.Cell(currentRow, 3).Value = item.amount;
                        worksheet.Cell(currentRow, 4).Value = item.qty;



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

        public async Task<ActionResult> ExportPdf_Order(string f = "", string t = "")
        {
            try
            {
                if (f == "")
                {
                    f = DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd");
                }
                if (t == "")
                {
                    t = DateTime.Now.ToString("yyyy-MM-dd");
                }
                StreamReader sr = new StreamReader("Challan/OrderReport.html");
                string s = sr.ReadToEnd();
                var result = await _orderRepository.getorderlist(1, 0, "", "", 0, f, t);
                string html = "";
                string items = "";
                int i = 1;
                foreach (var item in result)
                {
                    items += "<tr>" +
                        "<td style=\"border: 1px solid #dddddd; text-align: left; padding: 8px;\">" + i + "</td>" +
                        "<td style=\"border: 1px solid #dddddd; text-align: left; padding: 8px;\">" + item.orderid + "</td>" +
                        "<td style=\"border: 1px solid #dddddd; text-align: left; padding: 8px;\"></td>" +
                         "<td style=\"border: 1px solid #dddddd; text-align: left; padding: 8px;\">" + item.category + "</td>" +
                         "<td style=\"border: 1px solid #dddddd; text-align: left; padding: 8px;\">" + item.qty + "</td>" +
                          "<td style=\"border: 1px solid #dddddd; text-align: left; padding: 8px;\">" + item.orderdate + "</td>" +
                        "</tr>";


                    i = i + 1;
                }
                s = s.Replace("{report}", "Orderwise Report");
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
                    Message = ex.Message,
                    Data = ""
                });

            }

        }

        //==============End========================



        //========Department Wise====================

        [HttpGet]
        public async Task<ActionResult> ExportToExcel_department(int id = 0, string barcode = "", int supplierid = 0, int qty = 0, int type = 0, int catgeoryid = 0)
        {
            var result = await _orderRepository.getorderbydepartment();
            //exportfile(result.ToList());
            DataTable dt = new DataTable();
            dt.Columns.Add("Order Id", typeof(string));
            dt.Columns.Add("Department", typeof(string));
            dt.Columns.Add("Product", typeof(string));
            dt.Columns.Add("Category", typeof(string));
            dt.Columns.Add("MRP", typeof(string));
            dt.Columns.Add("QTY", typeof(string));
            dt.Columns.Add("Date", typeof(string));

            foreach (var item in result)
            {

                dt.Rows.Add(item.orderid, item.department, item.Description, item.Category, item.Rate, item.qty, item.orderdate);


            }
            string base64String;

            using (var wb = new XLWorkbook())
            {
                var sheet = wb.AddWorksheet(dt, "Department Wise Report");

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

        public void exportfile_department(List<Order_department> model)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("DepartmentWise Report");
                var currentRow = 1;
                worksheet.Cell(currentRow, 1).Value = "Order Id";
                worksheet.Cell(currentRow, 2).Value = "Department";
                worksheet.Cell(currentRow, 3).Value = "Product";
                worksheet.Cell(currentRow, 4).Value = "Category";
                worksheet.Cell(currentRow, 5).Value = "MRP";
                worksheet.Cell(currentRow, 6).Value = "QTY";
                worksheet.Cell(currentRow, 7).Value = "Date";


                foreach (var item in model)
                {
                    {
                        currentRow++;
                        worksheet.Cell(currentRow, 1).Value = item.orderid;
                        worksheet.Cell(currentRow, 2).Value = item.department;
                        worksheet.Cell(currentRow, 3).Value = item.Description;
                        worksheet.Cell(currentRow, 4).Value = item.Category;
                        worksheet.Cell(currentRow, 5).Value = item.Rate;
                        worksheet.Cell(currentRow, 6).Value = item.qty;
                        worksheet.Cell(currentRow, 7).Value = item.orderdate;


                    }
                }
                using var stream = new MemoryStream();
                workbook.SaveAs(stream);
                var content = stream.ToArray();
                Response.Clear();
                Response.Headers.Add("content-disposition", "attachment;filename=DepartmentWiseReport.xls");
                Response.ContentType = "application/xls";
                Response.Body.WriteAsync(content);
                //Response.Body.Flush();
                // return Redirect("../Report/StockReport");
            }




        }

        public async Task<ActionResult> ExportPdf_Department(string f = "", string t = "")
        {
            try
            {
                if (f == "")
                {
                    f = DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd");
                }
                if (t == "")
                {
                    t = DateTime.Now.ToString("yyyy-MM-dd");
                }
                StreamReader sr = new StreamReader("Challan/OrderReport.html");
                string s = sr.ReadToEnd();
                var result = await _orderRepository.getorderbydepartment(f, t);
                string html = "";
                string items = "";
                int i = 1;
                foreach (var item in result)
                {
                    items += "<tr>" +
                        "<td style=\"border: 1px solid #dddddd; text-align: left; padding: 8px;\">" + i + "</td>" +
                        "<td style=\"border: 1px solid #dddddd; text-align: left; padding: 8px;\">" + item.orderid + "</td>" +
                        "<td style=\"border: 1px solid #dddddd; text-align: left; padding: 8px;\"></td>" +
                         "<td style=\"border: 1px solid #dddddd; text-align: left; padding: 8px;\"></td>" +
                         "<td style=\"border: 1px solid #dddddd; text-align: left; padding: 8px;\">" + item.qty + "</td>" +
                          "<td style=\"border: 1px solid #dddddd; text-align: left; padding: 8px;\">" + item.orderdate + "</td>" +
                        "</tr>";


                    i = i + 1;
                }
                s = s.Replace("{report}", "Departmentwise Report");
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
                    Message = ex.Message,
                    Data = ""
                });

            }

        }

        //==============End========================



        //========Branch Wise====================

        [HttpGet]
        public async Task<ActionResult> ExportToExcel_branch(int id = 0, string barcode = "", int supplierid = 0, int qty = 0, int type = 0, int catgeoryid = 0)
        {
            var result = await _orderRepository.getorderbybranch();
            //exportfile(result.ToList());
            DataTable dt = new DataTable();
            dt.Columns.Add("Order Id", typeof(string));
            dt.Columns.Add("Branch", typeof(string));
            dt.Columns.Add("Product", typeof(string));
            dt.Columns.Add("Category", typeof(string));
            dt.Columns.Add("MRP", typeof(string));
            dt.Columns.Add("QTY", typeof(string));
            dt.Columns.Add("Date", typeof(string));

            foreach (var item in result)
            {

                dt.Rows.Add(item.orderid, item.department, item.Description, item.Category, item.Rate, item.qty, item.orderdate);


            }
            string base64String;

            using (var wb = new XLWorkbook())
            {
                var sheet = wb.AddWorksheet(dt, "Branch Wise Report");

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

        public void exportfile_branch(List<Order_department> model)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("BranchWise Report");
                var currentRow = 1;
                worksheet.Cell(currentRow, 1).Value = "Order Id";
                worksheet.Cell(currentRow, 2).Value = "Department";
                worksheet.Cell(currentRow, 3).Value = "Product";
                worksheet.Cell(currentRow, 4).Value = "Category";
                worksheet.Cell(currentRow, 5).Value = "MRP";
                worksheet.Cell(currentRow, 6).Value = "QTY";
                worksheet.Cell(currentRow, 7).Value = "Date";


                foreach (var item in model)
                {
                    {
                        currentRow++;
                        worksheet.Cell(currentRow, 1).Value = item.orderid;
                        worksheet.Cell(currentRow, 2).Value = item.department;
                        worksheet.Cell(currentRow, 3).Value = item.Description;
                        worksheet.Cell(currentRow, 4).Value = item.Category;
                        worksheet.Cell(currentRow, 5).Value = item.Rate;
                        worksheet.Cell(currentRow, 6).Value = item.qty;
                        worksheet.Cell(currentRow, 7).Value = item.orderdate;


                    }
                }
                using var stream = new MemoryStream();
                workbook.SaveAs(stream);
                var content = stream.ToArray();
                Response.Clear();
                Response.Headers.Add("content-disposition", "attachment;filename=BranchWiseReport.xls");
                Response.ContentType = "application/xls";
                Response.Body.WriteAsync(content);
                //Response.Body.Flush();
                // return Redirect("../Report/StockReport");
            }




        }

        //==============End========================
    }
}
