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
using OTS.UI.Models;
using System.Data;
using System.Text;

namespace OTS.UI.Controllers
{
    public class SupplierController : Controller
    {
        private readonly ICostRepository _costRepository;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IBranchRepository _branchRepository;
        private readonly IClientRepository _clientRepository;
        private readonly ICorpRepository _corpRepository;
        private readonly IVendorRepository _vendorRepository;
        private readonly Iproductrepository _iproductrepository;
        private readonly IOrderRepository _orderRepository;

        public SupplierController(ICostRepository costRepository, IDepartmentRepository departmentRepository, IBranchRepository branchRepository, IClientRepository clientRepository, ICorpRepository corpRepository,IVendorRepository vendorRepository,Iproductrepository iproductrepository,IOrderRepository orderRepository)
        {
            _costRepository = costRepository; 
            _departmentRepository = departmentRepository;
            _branchRepository = branchRepository;
            _clientRepository = clientRepository;
            _corpRepository = corpRepository;
            _vendorRepository = vendorRepository;
            _iproductrepository = iproductrepository;
            _orderRepository = orderRepository;
        }
        public async Task<ActionResult> Index()
        {
            combomodel model=new combomodel();
            var result = await _vendorRepository.getsuppdash();
            foreach (var item in result)
            {
                ViewBag.product_uploaded = item.total_product.ToString();
                ViewBag.complete_order = item.complete_order.ToString();
                ViewBag.icompelete_order = item.incomplete_order.ToString();
            }
            var result1 = await _orderRepository.getorderlistforvendor(0, "Unproceed");
            List<order_list_model> orderlist = new List<order_list_model>();
            foreach (var item in result1)
            {
                order_list_model orderModel = new order_list_model();
                orderModel.orderid= item.orderid;
                orderModel.orderdate= item.orderdate;
                orderModel.qty = item.qty;
                orderModel.amount = item.amount;
                orderModel.status = item.status;
                orderModel.department= item.department;
                orderModel.shipping_address = item.shipping_address;
                orderModel.billing_address = item.billing_address;
                orderModel.shippingaddress2 = item.shippingaddress2;
                orderModel.remark = item.remark;
                orderModel.category = item.category;
                orderlist.Add(orderModel);
            }
            model.order_list_model = orderlist;
            return View(model);
        }

        public async Task<ActionResult> master(int id=0,string t="")
        {
            combomodel combomodel = new combomodel();
            List<CostCenter> costlist= new List<CostCenter>();
            List<DepartmentModel> DepartmentList = new List<DepartmentModel>();
            List<BranchModel> branchlist = new List<BranchModel>();
            CostCenter costCenter=new CostCenter();
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
                combomodel.costCenterModel= costCenter;

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
            else {
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
        public async Task<ActionResult> User(int id=0)
        {
            if (id == 0)
            {
                combomodel combolist = new combomodel();
                List<ClientModel> list = new List<ClientModel>();

                var result = await _clientRepository.getdatasync();
                foreach (var item in result)
                {
                    ClientModel model = new ClientModel();
                    model.Id = item.Id;
                    model.Name = item.Name;
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
                foreach (var item in result5)
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

                combolist.Client_Model = list;
                combolist.costCenters = costlist;
                combolist.departmentModels = departmentlist;
                combolist.branchModels = branchlist;
                combolist.CorpUserModels = corptlist;
                combolist.UserTypeModels = usertypetlist;
                return View(combolist);
            }
            else {
                combomodel combolist = new combomodel();
                List<ClientModel> list = new List<ClientModel>();

                var result = await _clientRepository.getdatasync();
                foreach (var item in result)
                {
                    ClientModel model = new ClientModel();
                    model.Id = item.Id;
                    model.Name = item.Name;
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

                combolist.Client_Model = list;
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
        public async Task<ActionResult> UserList()
        {
            var result = await _corpRepository.getdatasync();
            return View(result);
        }

        public async Task<ActionResult> Client()
        {
            var result = await _clientRepository.getdatasync();
            return View(result);
        }
        public async Task<ActionResult> Edit_Client(int id)
        {
            var result = await _clientRepository.getdataByIdsync(id);

            return View(result);
        }
        [HttpPost]
        public async Task<ActionResult> Client(ClientModel Dept)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    DynamicParameters ObjParm = new DynamicParameters();
                    ObjParm.Add("@id", Dept.Id);
                    ObjParm.Add("@name", Dept.Name);
                    ObjParm.Add("@emailid", Dept.emailid);
                    ObjParm.Add("@mobileno", Dept.mobileno);
                    ObjParm.Add("@state", Dept.state);
                    ObjParm.Add("@gst", Dept.gst);
                    ObjParm.Add("@location", Dept.location);
                    ObjParm.Add("@category", Dept.category);
                    ObjParm.Add("@region", Dept.region);
                    if (Dept.Id == 0)
                    {

                        Dept.createdby = Convert.ToInt32(Request.Cookies["userid"].ToString());
                        ObjParm.Add("@createdby", Dept.createdby);
                    }
                    else
                    {
                        Dept.updatedby = Convert.ToInt32(Request.Cookies["userid"].ToString());
                        ObjParm.Add("@updatedby", Dept.updatedby);
                    }
                    ObjParm.Add("@result", dbType: DbType.Int32, direction: ParameterDirection.Output, size: 5215585);

                    await _clientRepository.AddAsync(ObjParm);
                    var id = ObjParm.Get<dynamic>("@result");
                    if (id > 0)
                    {
                        TempData["MsgType"] = "Success";
                        if (Dept.Id == 0)
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
                return RedirectToAction("Client");
            }
            catch (Exception ex)
            {
                TempData["MsgType"] = "Error";
                TempData["Msg"] = ex.Message;
                return View();
            }
        }


        public string Delete_client(int id)
        {
            try
            {
                _clientRepository.DeleteAsync(id);

                return "Record Deleted Successfully";
            }
            catch (Exception ex)
            {


                return ex.Message;
            }


        }
        public ActionResult frenchises() {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> frenchises(Frenchises_Model cost)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    DynamicParameters ObjParm = new DynamicParameters();
                    ObjParm.Add("@id", cost.id);
                    ObjParm.Add("@first_name", cost.first_name);
                    ObjParm.Add("@last_name", cost.last_name);
                    ObjParm.Add("@emailid", cost.emailid);
                    ObjParm.Add("@gst", cost.gst);
                    ObjParm.Add("@phoneno", cost.phoneno);
                    ObjParm.Add("@registerno", cost.register);
                    ObjParm.Add("@intrest", cost.intrest);
                    ObjParm.Add("@question", cost.question);
                    ObjParm.Add("@location", cost.location);
                    ObjParm.Add("@accept", cost.accept);
                    ObjParm.Add("@type", cost.type);
                    if (cost.id == 0)
                    {

                        cost.createdby = Convert.ToInt32(Request.Cookies["userid"].ToString());
                        ObjParm.Add("@createdby", cost.createdby);
                    }
                    else
                    {
                        cost.updatedby = Convert.ToInt32(Request.Cookies["userid"].ToString());
                        ObjParm.Add("@updatedby", cost.updatedby);
                    }
                    ObjParm.Add("@result", dbType: DbType.Int32, direction: ParameterDirection.Output, size: 5215585);

                    await _vendorRepository.Addfrenchises(ObjParm);
                    var id = ObjParm.Get<dynamic>("@result");
                    if (id > 0)
                    {
                        TempData["MsgType"] = "Success";
                        if (cost.id == 0)
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
                return Redirect("../Supplier/frenchises");
            }
            catch (Exception ex)
            {
                TempData["MsgType"] = "Error";
                TempData["Msg"] = ex.Message;
                return View();
            }
        }

        public async Task<ActionResult> Viewfrenchises()
        {
            var result = await _vendorRepository.getfrenchises(Convert.ToInt32(Request.Cookies["userid"].ToString()));
            return View(result);
        }

        public async Task<ActionResult> UploadProduct()
        {
            
            List<ClientModel> list = new List<ClientModel>();

            var result = await _clientRepository.getdatasync();
            foreach (var item in result)
            {
                ClientModel model = new ClientModel();
                model.Id = item.Id;
                model.Name = item.Name;
                list.Add(model);
            }
            return View(list);
        }

        [HttpPost]
        public async Task<ActionResult> UploadProduct(IFormFile file, int clientid)
        {
            if (file != null && file.Length > 0)
            {


                var productlist = ParseExcelFile(file.OpenReadStream());
                foreach (var item in productlist)
                {
                    DynamicParameters ObjParm = new DynamicParameters();
                    ObjParm.Add("@Description", item.Description);
                    ObjParm.Add("@Make", item.Make);
                    ObjParm.Add("@HSN", item.HSN);
                    ObjParm.Add("@Quantity", item.Quantity);
                    ObjParm.Add("@Rate", item.Rate);
                    ObjParm.Add("@Unit", item.Unit);
                    ObjParm.Add("@stock", item.stock);
                    ObjParm.Add("@Category", item.Category);
                    ObjParm.Add("@ImageUrl", item.ImageUrl);
                    ObjParm.Add("@gst", item.gst);
                    ObjParm.Add("@vendorid", Request.Cookies["userid"].ToString());
                    ObjParm.Add("@clientid", clientid);
                    ObjParm.Add("@createdby", Convert.ToInt32(Request.Cookies["userid"].ToString()));
                    ObjParm.Add("@result", dbType: DbType.Int32, direction: ParameterDirection.Output, size: 5215585);
                    await _iproductrepository.AddAsync(ObjParm);
                    //var id = ObjParm.Get<dynamic>("@result");
                }

                TempData["MsgType"] = "Success";

                TempData["Msg"] = "Record Updated Successfully";


                return Redirect("../supplier/UploadProduct"); // Redirect to a view showing success or list of products
            }
            return View();
        }

        public List<ProductModel> ParseExcelFile(Stream stream)
        {
            var employees = new List<ProductModel>();
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
                    var employee = new ProductModel
                    {
                        Description = row.Cell(1).GetValue<string>(),
                        Make = row.Cell(2).GetValue<string>(),
                        HSN = row.Cell(3).GetValue<string>(),
                        Quantity = row.Cell(4).GetValue<string>(),
                        Rate = row.Cell(5).GetValue<string>(),
                        Unit = row.Cell(6).GetValue<string>(),
                        stock = row.Cell(7).GetValue<string>(),
                        Category = row.Cell(8).GetValue<string>(),
                        ImageUrl = row.Cell(9).GetValue<string>(),
                        gst = row.Cell(10).GetValue<string>(),

                    };
                    //Add the Employee to the List of Employees
                    employees.Add(employee);
                }
            }
            //Finally return the List of Employees
            return employees;
        }

        public async Task<ActionResult> Stationary(int id, int row)
        {
            var result = await _iproductrepository.getdataBycategorysync("Stationery", row);

            return View(result);
        }
        public async Task<ActionResult> Housekeeping(int id, int row)
        {
            var result = await _iproductrepository.getdataBycategorysync("houskeeping", row);

            return View(result);
        }

        public async Task<ActionResult> VisitingCard()
        {
            var result = await _iproductrepository.getdataBycategorysync("Visiting Card", 0);
            return View(result);
        }
        public async Task<ActionResult> Printing()
        {
            combomodel combolist = new combomodel();
            List<ProductModel> productlist = new List<ProductModel>();
            var result = await _iproductrepository.getdataBycategorysync("Printing", 0);
            foreach (var item in result)
            {
                ProductModel model = new ProductModel();
                model.id = item.id;
                model.Description = item.Description;
                model.size = item.size;
                model.clientid = item.clientid;
                productlist.Add(model);
            }
            combolist.product_Model = productlist;
           
            return View(combolist);
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
                model.qty = item.qty;
                model.updated_qty = item.updated_qty;
                model.rate = item.rate;
                model.hsncode = item.hsncode;
                model.Description = item.Description;
                model.category = item.category;
                model.gst_per = item.gst_per;
                model.gst_amt = item.gst_amt;
                order_Details.Add(model);
            }
            order_Combo.order_List_Model = order_List_Model;
            order_Combo.order_list = order_Details;
            return View(order_Combo);
        }

        public async Task<ActionResult> EditOrder(int id)
        {
            var Result = await _orderRepository.get_order_details(id);
            return View(Result);
        }
        public async Task<ActionResult> AllOrder(string f = "", string t = "")
        {
            if (f == "")
            {
                f = DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd");
            }
            if (t == "")
            {
                t = DateTime.Now.AddDays(2).ToString("yyyy-MM-dd");
            }
            var result = await _orderRepository.getorderlist(6,0,"","",0,f,t);
            return View(result);
        }
        public async Task<ActionResult> UnproceedOrder(string f="", string t="")
        {
            if (f == "")
            {
                f = DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd");
            }
            if (t == "")
            {
                t = DateTime.Now.ToString("yyyy-MM-dd");
            }
            var result = await _orderRepository.getorderlistforvendor(0, "Unproceed",0,f,t);
            return View(result);
        }

        [HttpGet]
        public async Task<ActionResult> ExportToExcel_UnproceedOrder(string f = "", string t = "")
        {
            var result = await _orderRepository.getorderlistforvendor(0, "Unproceed", 0, f, t);
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
                var sheet = wb.AddWorksheet(dt, "UnproceedOrder");

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

        public async Task<ActionResult> ExportPdf_UnproceedOrder(string f = "", string t = "")
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
                StreamReader sr = new StreamReader("Challan/htmlpage.html");
                string s = sr.ReadToEnd();
                var result = await _orderRepository.getorderlistforvendor(0, "Unproceed", 0, f, t);
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
                s = s.Replace("{report}", "Unproceed Order");
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
        public async Task<ActionResult> ApprovedOrder(string f = "",string t="")
        {
            if (f == "")
            {
                f = DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd");
            }
            if (t == "")
            {
                t = DateTime.Now.ToString("yyyy-MM-dd");
            }
            var result = await _orderRepository.getorderlistforvendor(12,"",0,f,t);
            return View(result);
        }

        [HttpGet]
        public async Task<ActionResult> ExportToExcel_ApprovedOrder(string f = "", string t = "")
        {
            var result = await _orderRepository.getorderlistforvendor(12, "", 0, f, t);
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
                var result = await _orderRepository.getorderlistforvendor(12, "", 0, f, t);
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
        public async Task<ActionResult> ReleaseOrder()
        {
            var result = await _orderRepository.getorderlistforvendor(16);
            return View(result);
        }
        public async Task<ActionResult> DeniedOrder()
        {
            var result = await _orderRepository.getorderlistforvendor(13);
            return View(result);
        }

        public ActionResult quotation()
        {
            return View();
        }
        public ActionResult Viewquotation()
        {
            return View();
        }

        //Report//
        public async Task<ActionResult> OrderwiseReport(string f="",string t="")
        {
            if (f == "")
            {
                f = DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd");
            }
            if (t == "")
            {
                t = DateTime.Now.ToString("yyyy-MM-dd");
            }
            var result = await _orderRepository.getorderlist(16,0,"","",0,f,t);
            return View(result);
        }
        public async Task<ActionResult> ProductwiseReport(string f = "", string t = "")
        {
            if (f == "")
            {
                f = DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd");
            }
            if (t == "")
            {
                t = DateTime.Now.ToString("yyyy-MM-dd");
            }
            var result = await _orderRepository.getorderbyproduct(0,f,t);
            return View(result);
        }
        public ActionResult CostwiseReport()
        {
            return View();
        }
        public async Task<ActionResult> DepartmentReport(string f = "", string t = "")
        {
            if (f == "")
            {
                f = DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd");
            }
            if (t == "")
            {
                t = DateTime.Now.ToString("yyyy-MM-dd");
            }
            var result = await _orderRepository.getorderbydepartment(f,t);
            return View(result);
        }
        public async Task<ActionResult> BranchwiseReport()
        {
            var result = await _orderRepository.getorderbybranch();
            return View(result);
        }

        public async Task<ActionResult> createchallan()
        {
            var result = await _orderRepository.getorderlistforvendor(12);
            return View(result);
        }

        public async Task<ActionResult> viewchallan(int id)
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
                model.qty = item.qty;
                model.updated_qty = item.updated_qty;
                model.rate = item.rate;
                model.hsncode = item.hsncode;
                model.Description = item.Description;
                model.category = item.category;

                order_Details.Add(model);
            }
            order_Combo.order_List_Model = order_List_Model;
            order_Combo.order_list = order_Details;
            return View(order_Combo);
        }

        [HttpPost]
        public async Task<ActionResult> viewchallan(string awbno, string curiourname, string mode, int id, IFormFile file, string filepath="")
        {

            try
            {
                //if (ModelState.IsValid)
                //  {
                var uniqueFileName = "";
                if (file != null)
                {
                    if (file.Length > 0)
                    {
                        uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", uniqueFileName);
                        using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                        {
                            //This will save to Local folder
                            await file.CopyToAsync(stream);
                        }

                    }

                }
                else
                {
                    uniqueFileName = filepath;
                }

                DynamicParameters ObjParm = new DynamicParameters();
                ObjParm.Add("@id", "0");
                ObjParm.Add("@awbno", awbno);
                ObjParm.Add("@name", curiourname);
                ObjParm.Add("@mode", mode);
                ObjParm.Add("@file_path", uniqueFileName);
                ObjParm.Add("@orderid", id);

                await _vendorRepository.AddShipping(ObjParm);
                TempData["MsgType"] = "Success";

                TempData["Msg"] = "Record Updated Successfully";
                // }
                return Redirect("../Supplier/createchallan");
            }
            catch (Exception)
            {

                TempData["MsgType"] = "Error";

                TempData["Msg"] = "Somthing Went Wrong";
                // }
                return Redirect("../Supplier/createchallan");
            }
        }

        public async Task<ActionResult> ChallanList()
        {
            var result = await _orderRepository.getorderlistforvendor(14);
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
                order_List_Model.clientname=item.clientname;
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
                    item1 += @" <tr>
                        <td>"+g+@"</td>
                        <td>"+ item.Description + @"</td>
                        <td>"+item.hsncode + @"</td>
                        <td>" + item.rate+ @"</td> <td>" + item.gst_per + @"</td>
                        <td>" + item.qty+@"</td>
                    </tr>";
                    //item1 += "<tr><td>" + g + "</td><td>" + item.Description + "</td><td>" + item.updated_qty + "</td><td>PCS</td><td>"+item.rate+"</td></tr>";
                }
                else {
                    item1 += @" <tr>
                        <td>" + g + @"</td>
                        <td>" + item.Description + @"</td>
                        <td>" + item.hsncode + @"</td>
                        <td>" + item.rate + @"</td>  <td>" + item.gst_per + @"</td>

                        <td>" + item.qty + @"</td>
                    </tr>";
                    //item1 += "<tr><td>" + g + "</td><td>" + item.Description + "</td><td>" + item.qty + "</td><td>PCS</td><td>"+item.rate+"</td></tr>";
                }

                g = g + 1;
            }
            order_Combo.order_List_Model = order_List_Model;
            order_Combo.order_list = order_Details;

            s = s.Replace("{challanno}", order_List_Model.orderid)
                .Replace("{date}", order_List_Model.orderdate)
                .Replace("{buyername}", order_List_Model.clientname)
                .Replace("{addressline1}", order_List_Model.billing_address)
                .Replace("{consigneename}", order_List_Model.clientname)
                .Replace("{addressline2}", order_List_Model.shipping_address)
                .Replace("{item}", item1)
                .Replace("{gst}", order_List_Model.gst)
                .Replace("{totalAmount}", order_List_Model.amount)
                .Replace("{totalAmountWords}", NumberToWords.ConvertToWords(Convert.ToDecimal(order_List_Model.amount)));

            

            TempData["html"] = s;
            

            return View();
        }
        public ActionResult challan_add()
        {
            return View();
        }
        public async Task<ActionResult> Invoice()
        {
            var result = await _orderRepository.getinvoicelistforvendor(0);
            return View(result);
        }

        public async Task<ActionResult> ViewInvoice()
        {
            var result = await _orderRepository.getinvoicelistforvendor(1);
            return View(result);
        }

        public async Task<ActionResult> CreateInvoice(int id)
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
                model.qty = item.qty;
                model.updated_qty = item.updated_qty;
                model.rate = item.rate;
                model.hsncode = item.hsncode;
                model.Description = item.Description;
                model.category = item.category;
                model.gst_amt = item.gst_amt;
                model.gst_per = item.gst_per;
                order_Details.Add(model);
            }
            order_Combo.order_List_Model = order_List_Model;
            order_Combo.order_list = order_Details;
            return View(order_Combo);
        }

        [HttpPost]
        public async Task<ActionResult> Create_Invoice(int id)
        {
            try
            {
               
                    DynamicParameters ObjParm = new DynamicParameters();
                    ObjParm.Add("@id", id);
                   
                    await _vendorRepository.update_invoice(ObjParm);
                    
                    TempData["MsgType"] = "Success";
                    TempData["Msg"] = "Invoice Create Successfully";
                        
                
                return Redirect("../Supplier/viewinvoice");
            }
            catch (Exception ex)
            {
                TempData["MsgType"] = "Error";
                TempData["Msg"] = ex.Message;
                return View();
            }
        }
        public ActionResult about()
        {
            return View();
        }
        public ActionResult runtimeorder()
        {
            return View();
        }

        public async Task<ActionResult> UploadedProduct()
        {
            var result = await _iproductrepository.getdataUploadedProduct();

            return View(result);
        }
        public async Task<ActionResult> UploadedVisitingcard(int id=0)
        {
            if (id == 0)
            {
                combomodel combolist = new combomodel();
                List<ClientModel> corptlist = new List<ClientModel>();
                List<ProductModel> productlist = new List<ProductModel>();
                var result2 = await _clientRepository.getdatasync();
                foreach (var item in result2)
                {
                    ClientModel model = new ClientModel();
                    model.Id = item.Id;
                    model.Name = item.Name;
                    corptlist.Add(model);
                }

                ProductModel product = new ProductModel();
                combolist.productModel = product;
                combolist.Client_Model = corptlist;
                return View(combolist);
            }
            else {
                combomodel combolist = new combomodel();
                List<ClientModel> corptlist = new List<ClientModel>();
                List<ProductModel> productlist = new List<ProductModel>();
                var result2 = await _clientRepository.getdatasync();
                foreach (var item in result2)
                {
                    ClientModel model = new ClientModel();
                    model.Id = item.Id;
                    model.Name = item.Name;
                    corptlist.Add(model);
                }

                var result = await _iproductrepository.getdataByIdsync(id,"Visiting Card", 0);
               
                    ProductModel product = new ProductModel();
                product.id = result.id;
                product.Description = result.Description;
                product.ImageUrl = result.ImageUrl;
                product.clientid = result.clientid;


                combolist.productModel = product;
                combolist.Client_Model = corptlist;
                return View(combolist);
            }
        }

        [HttpPost]
        public async Task<ActionResult> UploadedVisitingcard(int id,int clientid, string desc, IFormFile file,string filepath1="")
        {
            var uniqueFileName = "";
            if (file != null)
            {
                if (file.Length > 0)
                {
                    uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", uniqueFileName);
                    using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                    {
                        //This will save to Local folder
                        await file.CopyToAsync(stream);
                    }

                }
               
            }
            else
            {
                uniqueFileName = filepath1;
            }
            DynamicParameters ObjParm = new DynamicParameters();
            ObjParm.Add("@id",id );
            ObjParm.Add("@Description", desc);
            ObjParm.Add("@Make", "");
            ObjParm.Add("@HSN", "");
            ObjParm.Add("@Quantity", "1");
            ObjParm.Add("@Rate", "0");
            ObjParm.Add("@Unit", "Pcs");
            ObjParm.Add("@stock", "0");
            ObjParm.Add("@Category", "Visiting Card");
            ObjParm.Add("@ImageUrl", uniqueFileName);
            ObjParm.Add("@vendorid", Request.Cookies["userid"].ToString());
            ObjParm.Add("@clientid", clientid);
            ObjParm.Add("@createdby", Convert.ToInt32(Request.Cookies["userid"].ToString()));
            ObjParm.Add("@result", dbType: DbType.Int32, direction: ParameterDirection.Output, size: 5215585);
            await _iproductrepository.AddAsync(ObjParm);

            TempData["MsgType"] = "Success";

            TempData["Msg"] = "Record Updated Successfully";
            return Redirect("../Supplier/VisitingCard");
        }

        public string Delete_product(int id,int clientid)
        {
            try
            {
                _iproductrepository.DeleteProduct(id, clientid);

                return "Record Deleted Successfully";
            }
            catch (Exception ex)
            {


                return ex.Message;
            }


        }

        public async Task<ActionResult> UploadPrinting(int id = 0)
        {
            if (id == 0)
            {
                combomodel combolist = new combomodel();
                List<ClientModel> corptlist = new List<ClientModel>();
                List<ProductModel> productlist = new List<ProductModel>();
                var result2 = await _clientRepository.getdatasync();
                foreach (var item in result2)
                {
                    ClientModel model = new ClientModel();
                    model.Id = item.Id;
                    model.Name = item.Name;
                    corptlist.Add(model);
                }
                ProductModel product = new ProductModel();
                combolist.productModel = product;
                combolist.Client_Model = corptlist;
                return View(combolist);
            }
            else {
                combomodel combolist = new combomodel();
                List<ClientModel> corptlist = new List<ClientModel>();
                List<ProductModel> productlist = new List<ProductModel>();
                var result2 = await _clientRepository.getdatasync();
                foreach (var item in result2)
                {
                    ClientModel model = new ClientModel();
                    model.Id = item.Id;
                    model.Name = item.Name;
                    corptlist.Add(model);
                }

                var result = await _iproductrepository.getdataByIdsync(id, "Printing", 0);

                ProductModel product = new ProductModel();
                product.id = result.id;
                product.Description = result.Description;
                product.size = result.size;
                product.clientid = result.clientid;


                combolist.productModel = product;
                combolist.Client_Model = corptlist;
                return View(combolist);
            }
        }


        [HttpPost]
        public async Task<ActionResult> UploadPrinting(int id,int clientid, string desc, string size)
        {
            DynamicParameters ObjParm = new DynamicParameters();
            ObjParm.Add("@id", id);
            ObjParm.Add("@Description", desc);
            ObjParm.Add("@Make", "");
            ObjParm.Add("@HSN", "");
            ObjParm.Add("@Quantity", "1");
            ObjParm.Add("@Rate", "0");
            ObjParm.Add("@Unit", "Pcs");
            ObjParm.Add("@stock", "0");
            ObjParm.Add("@Category", "Printing");
            ObjParm.Add("@ImageUrl", "");
            ObjParm.Add("@size", size);
            ObjParm.Add("@vendorid", Request.Cookies["userid"].ToString());
            ObjParm.Add("@clientid", clientid);
            ObjParm.Add("@createdby", Convert.ToInt32(Request.Cookies["userid"].ToString()));
            ObjParm.Add("@result", dbType: DbType.Int32, direction: ParameterDirection.Output, size: 5215585);
            await _iproductrepository.AddAsync(ObjParm);

            TempData["MsgType"] = "Success";

            TempData["Msg"] = "Record Updated Successfully";
            return Redirect("../Supplier/Printing");
        }

        public async Task<ActionResult> uploadagreement()
        {
            combomodel combolist = new combomodel();
            List<ClientModel> corptlist = new List<ClientModel>();
            List<ProductModel> productlist = new List<ProductModel>();
            var result2 = await _clientRepository.getdatasync();
            foreach (var item in result2)
            {
                ClientModel model = new ClientModel();
                model.Id = item.Id;
                model.Name = item.Name;
                corptlist.Add(model);
            }
            ProductModel product = new ProductModel();
            combolist.productModel = product;
            combolist.Client_Model = corptlist;
            return View(combolist);
        }
        [HttpPost]
        public async Task<ActionResult> uploadagreement(AgreementModel Model, IFormFile file,string filepath1="")
        {
            var uniqueFileName = "";
            if (file != null)
            {
                if (file.Length > 0)
                {
                    uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", uniqueFileName);
                    using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                    {
                        //This will save to Local folder
                        await file.CopyToAsync(stream);
                    }

                }

            }
            else
            {
                uniqueFileName = filepath1;
            }
            DynamicParameters ObjParm = new DynamicParameters();
            ObjParm.Add("@id", Model.id);
            ObjParm.Add("@clientid", Model.clientid);
            ObjParm.Add("@file_path", uniqueFileName);
            ObjParm.Add("@result", dbType: DbType.Int32, direction: ParameterDirection.Output, size: 5215585);
            await _vendorRepository.Addagreement(ObjParm);

            TempData["MsgType"] = "Success";

            TempData["Msg"] = "Record Updated Successfully";
            return Redirect("../Supplier/Viewagreement");
        }
        public async Task<ActionResult> Viewagreement()
        {
            var result = await _vendorRepository.getagreement(Convert.ToInt32(Request.Cookies["userid"].ToString()));
            return View(result);
        }

        public async Task<ActionResult> Department(int id = 0, string t = "")
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

        public async Task<ActionResult> branch(int id = 0, string t = "")
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

        [HttpPost]
        public async Task<ActionResult> product_upload_images(List<IFormFile> file)
        {
            foreach (var files in file)
            {
                if (files.Length > 0)
                {
                    var uniqueFileName = files.FileName;
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", uniqueFileName);
                    using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                    {
                        //This will save to Local folder
                        await files.CopyToAsync(stream);
                    }

                }
            }
            return Redirect("../Supplier/UploadProduct");
        }

        public async Task<ActionResult> AddProduct()
        {
            combomodel combolist = new combomodel();
            List<ClientModel> corptlist = new List<ClientModel>();
            var result2 = await _clientRepository.getdatasync();
            foreach (var item in result2)
            {
                ClientModel model = new ClientModel();
                model.Id = item.Id;
                model.Name = item.Name;

                corptlist.Add(model);
            }
            combolist.Client_Model = corptlist;
            return View(combolist);
        }

        [HttpPost]
        public async Task<ActionResult> AddProduct(IFormFile file,ProductModel modl)
        {
            if (file != null && file.Length > 0)
            {


                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", uniqueFileName);
                using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                {
                    //This will save to Local folder
                    await file.CopyToAsync(stream);
                }
                modl.ImageUrl = "uploads/" + uniqueFileName;

                DynamicParameters ObjParm = new DynamicParameters();
                    ObjParm.Add("@Description", modl.Description);
                    ObjParm.Add("@Make", modl.Make);
                    ObjParm.Add("@HSN", modl.HSN);
                    ObjParm.Add("@Quantity", modl.Quantity);
                    ObjParm.Add("@Rate", modl.Rate);
                    ObjParm.Add("@Unit", modl.Unit);
                    ObjParm.Add("@stock", modl.stock);
                    ObjParm.Add("@Category", modl.Category);
                    ObjParm.Add("@ImageUrl", modl.ImageUrl);
                    ObjParm.Add("@vendorid", Request.Cookies["userid"].ToString());
                    ObjParm.Add("@clientid", modl.clientid);
                    ObjParm.Add("@createdby", Convert.ToInt32(Request.Cookies["userid"].ToString()));
                    ObjParm.Add("@result", dbType: DbType.Int32, direction: ParameterDirection.Output, size: 5215585);
                    await _iproductrepository.AddAsync(ObjParm);
                    //var id = ObjParm.Get<dynamic>("@result");
                

                TempData["MsgType"] = "Success";

                TempData["Msg"] = "Record Updated Successfully";


                return Redirect("../Supplier/AddProduct"); // Redirect to a view showing success or list of products
            }
            return View();
        }

        public async Task<ActionResult> Printinvoice(int id)
        {
            StreamReader sr = new StreamReader("Challan/invoice1.html");
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
            decimal gst_amt = 0;
            foreach (var item in Result1)
            {
                gst_amt = gst_amt + Convert.ToDecimal(item.gst_amt);
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
                    item1 += "<tr><td>" + g + "</td><td>" + item.Description + "</td><td>" + item.hsncode +"</td><td>" + item.gst_per + "</td><td>"+ item.updated_qty + "</td><td>" + item.rate + "</td><td></td><td></td><td>" + Convert.ToDecimal(item.updated_qty)* Convert.ToDecimal(item.rate) + "</td></tr>";
                }
                else
                {
                    item1 += "<tr><td>" + g + "</td><td>" + item.Description + "</td><td>" + item.hsncode + "</td><td>" + item.gst_per + "</td><td>" + item.qty + "</td><td>" + item.rate + "</td><td></td><td></td><td>" + Convert.ToDecimal(item.qty) * Convert.ToDecimal(item.rate) + "</td></tr>";
                }

                g = g + 1;
            }
            order_Combo.order_List_Model = order_List_Model;
            order_Combo.order_list = order_Details;

            s = s.Replace("{invoiceno}", order_List_Model.orderid).Replace("{date}", order_List_Model.orderdate).Replace("{buyername}", order_List_Model.clientname).Replace("{addressline1}", order_List_Model.billing_address).Replace("{consigneename}", order_List_Model.clientname).Replace("{addressline2}", order_List_Model.shipping_address).Replace("{item}", item1).Replace("{cgst}", (Convert.ToDecimal(gst_amt) /2).ToString()).Replace("{sgst}", (Convert.ToDecimal(gst_amt) /2).ToString()).Replace("{grandtotal}",order_List_Model.amount);
            TempData["html"] = s;


            return View();
        }

        [HttpPost]
        public async Task<ActionResult> updatestock(int pid,int type,int qty,int i)
        {
            try
            {
               
                    DynamicParameters ObjParm = new DynamicParameters();
                    ObjParm.Add("@id", pid);
                    ObjParm.Add("@type",type);
                    ObjParm.Add("@qty", qty);
                    
                    await _vendorRepository.updatestock(ObjParm);

                TempData["MsgType"] = "Success";
                TempData["Msg"] = "Stock Update Successfully";
                if (i == 0)
                {
                    return Redirect("../Supplier/Stationary");
                }
                else {
                    return Redirect("../Supplier/Housekeeping");
                }
            }
            catch (Exception ex)
            {
                TempData["MsgType"] = "Error";
                TempData["Msg"] = ex.Message;
                return View();
            }
        }


        //========Product Wise====================

        [HttpGet]
        public async Task<ActionResult> ExportToExcel(int id = 0, string barcode = "", int supplierid = 0, int qty = 0, int type = 0, int catgeoryid = 0,string f="",string t="")
        {

            if (f == "")
            {
                f = DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd");
            }
            if (t == "")
            {
                t = DateTime.Now.ToString("yyyy-MM-dd");
            }
            var result = await _orderRepository.getorderbyproduct(0,f,t);
            //exportfile(result.ToList());
            DataTable dt = new DataTable();
            dt.Columns.Add("Order Id", typeof(string));
            dt.Columns.Add("Product Name", typeof(string));
            dt.Columns.Add("Product Category", typeof(string));
            dt.Columns.Add("GST (%)", typeof(string));
            dt.Columns.Add("HSN Code", typeof(string));
            dt.Columns.Add("Qty", typeof(string));
            dt.Columns.Add("MRP", typeof(string));
            dt.Columns.Add("Date", typeof(string));
            foreach (var item in result)
            {

                dt.Rows.Add(item.orderid, item.name, item.category, item.gst, item.hsn, item.qty,item.rate,item.orderdate);


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

        public async Task<ActionResult> ExportPdf_product(string f="",string t="")
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
              var result = await _orderRepository.getorderbyproduct(0,f,t);
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
        public async Task<ActionResult> ExportToExcel_order(int id = 0, string barcode = "", int supplierid = 0, int qty = 0, int type = 0, int catgeoryid = 0,string f = "", string t = "")
        {
            if (f == "")
            {
                f = DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd");
            }
            if (t == "")
            {
                t = DateTime.Now.ToString("yyyy-MM-dd");
            }
            var result = await _orderRepository.getorderlist(1,0,"","",0,f,t);
            //exportfile(result.ToList());
            DataTable dt = new DataTable();
            dt.Columns.Add("Order Id", typeof(string));
            dt.Columns.Add("Order Date", typeof(string));
            dt.Columns.Add("MRP", typeof(string));
            dt.Columns.Add("QTY", typeof(string));
     
            foreach (var item in result)
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

        public async Task<ActionResult> ExportPdf_Order(string f="",string t="")
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
                var result = await _orderRepository.getorderlist(1,0,"","",0,f,t);
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

                dt.Rows.Add(item.orderid, item.department, item.Description, item.Category,item.Rate, item.qty,item.orderdate);


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

        public async Task<ActionResult> ExportPdf_Department(string f="",string t="")
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
                var result = await _orderRepository.getorderbydepartment(f,t);
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
