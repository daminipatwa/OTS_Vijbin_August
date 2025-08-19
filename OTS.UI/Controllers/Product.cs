using ClosedXML.Excel;
using Dapper;
using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using OTS.Data.Models;
using OTS.Data.Repository;
using System.Data;
using System.IO;

namespace OTS.UI.Controllers
{
    public class Product : Controller
    {
        private readonly Iproductrepository _productrepository;
        private readonly ICorpRepository _corpRepository;
        private readonly IClientRepository _clientRepository;

        public Product(Iproductrepository productrepository, ICorpRepository corpRepository,IClientRepository clientRepository)
        {
            _productrepository = productrepository;
            _corpRepository = corpRepository;
            _clientRepository = clientRepository;
        }
        public async Task<ActionResult> list()
        {
            var result= await _productrepository.getdataBycategorysync("Visiting Card", 0);
            return View(result);
        }

        public ActionResult visitingcard(int id)
        {
            TempData["productid"] = id;
            return View();
        }
        public async Task<ActionResult> printing()
        {
            var result = await _productrepository.getdataBycategorysync("Printing", 0);
            return View(result);
        }
        public async Task<ActionResult> product_list(int id,int row)
        {
            if (id == 1)
            {
                
                var result = await _productrepository.getdataBycategorysync("Stationery", row);
                
                return View(result);
            }


            return View();
        }

        public async Task<ActionResult> product_upload()
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
        public async Task<ActionResult> product_upload(IFormFile file,int clientid)
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
                    ObjParm.Add("@vendorid", Request.Cookies["userid"].ToString());
                    ObjParm.Add("@clientid", clientid);
                    ObjParm.Add("@createdby", Convert.ToInt32(Request.Cookies["userid"].ToString()));
                    ObjParm.Add("@result", dbType: DbType.Int32, direction: ParameterDirection.Output, size: 5215585);
                    await _productrepository.AddAsync(ObjParm);
                    //var id = ObjParm.Get<dynamic>("@result");
                }
               
                    TempData["MsgType"] = "Success";
                    
                        TempData["Msg"] = "Record Updated Successfully";
                    
                
                return View(); // Redirect to a view showing success or list of products
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
                       
                    };
                    //Add the Employee to the List of Employees
                    employees.Add(employee);
                }
            }
            //Finally return the List of Employees
            return employees;
        }

        [HttpPost]
        public async Task<ActionResult> product_upload_images(List<IFormFile> file)
        {
            foreach (var files in file)
            {
                if (files.Length > 0)
                {
                    var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(files.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", uniqueFileName);
                    using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                    {
                        //This will save to Local folder
                        await files.CopyToAsync(stream);
                    }
                    
                }
            }
                return View("product_upload");
        }

        //==============Vendor Pages=========================
        public async Task<ActionResult> productlist(int id, int row)
        {
            
                var result = await _productrepository.getdataBycategorysync("Stationery", row);

                return View(result);

            return View();
        }

        public async Task<ActionResult> upload_visitingcard()
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

            var result = await _productrepository.getdataBycategorysync("Visiting Card", 0);
            foreach (var item in result)
            {
                ProductModel model = new ProductModel();
                model.id = item.id;
                model.Description = item.Description;
                model.ImageUrl = item.ImageUrl;
                productlist.Add(model);
            }
            combolist.product_Model = productlist;
            combolist.Client_Model = corptlist;
            return View(combolist);
        }

        [HttpPost]
        public async Task<ActionResult> upload_visitingcard(int clientid,string desc,IFormFile file)
        {
            var uniqueFileName = "";
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
            DynamicParameters ObjParm = new DynamicParameters();
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
            await _productrepository.AddAsync(ObjParm);

            TempData["MsgType"] = "Success";

            TempData["Msg"] = "Record Updated Successfully";
            return Redirect("../Product/upload_visitingcard");
        }

        public async Task<ActionResult> Add_printing()
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

            var result = await _productrepository.getdataBycategorysync("Printing", 0);
            foreach (var item in result)
            {
                ProductModel model = new ProductModel();
                model.id = item.id;
                model.Description = item.Description;
                model.size = item.size;
                productlist.Add(model);
            }
            combolist.product_Model = productlist;
            combolist.Client_Model = corptlist;
            return View(combolist);
        }

        [HttpPost]
        public async Task<ActionResult> Add_printing(int clientid, string desc, string size)
        {
            DynamicParameters ObjParm = new DynamicParameters();
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
            await _productrepository.AddAsync(ObjParm);

            TempData["MsgType"] = "Success";

            TempData["Msg"] = "Record Updated Successfully";
            return Redirect("../Product/Add_printing");
        }
    }
}
