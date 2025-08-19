using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using OTS.Data.Models;
using OTS.Data.Repository;
using System.Data;
using System.Runtime.InteropServices;

namespace OTS.UI.Controllers
{
    public class MastersController : Controller
    {
        private readonly ICostRepository _costRepository;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IBranchRepository _branchRepository;
        private readonly IClientRepository _clientRepository;

        public MastersController(ICostRepository costRepository,IDepartmentRepository departmentRepository,IBranchRepository branchRepository,IClientRepository clientRepository)
        {
           _costRepository = costRepository;
           _departmentRepository = departmentRepository;
            _branchRepository = branchRepository;
            _clientRepository = clientRepository;
        }

        
        public async Task<ActionResult> CostCenter()
        {

           
           var result= await _costRepository.getdatasync();
            return View(result);
        }

        public async Task<ActionResult> Edit_CostCenter(int id)
        {
           var result=await  _costRepository.getdataByIdsync(id);

            return View(result);
        }


        [HttpPost]
        public async Task<ActionResult> CostCenter(CostCenter cost)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    DynamicParameters ObjParm = new DynamicParameters();
                    ObjParm.Add("@id", cost.Id);
                    ObjParm.Add("@name", cost.Name);

                    if (cost.Id == 0)
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

                    await _costRepository.AddAsync(ObjParm);
                    var id = ObjParm.Get<dynamic>("@result");
                    if (id > 0)
                    {
                        TempData["MsgType"] = "Success";
                        if (cost.Id == 0)
                        {
                            TempData["Msg"] = "Record Saved Successfully";
                        }
                        else {
                            TempData["Msg"] = "Record Updated Successfully";
                        }
                    }
                    else
                    {
                        TempData["MsgType"] = "Error";
                        TempData["Msg"] = "Something went wrong";
                    }
                }
                return Redirect("../Supplier/Master?t=C");
            }
            catch (Exception ex)
            {
                TempData["MsgType"] = "Error";
                TempData["Msg"] = ex.Message;
                return View();
            }
        }

        public string Delete_costcenter(int id)
        {
            try
            {
                _costRepository.DeleteAsync(id);
                
                return "Record Deleted Successfully";
            }
            catch (Exception ex)
            {

                
                return ex.Message;
            }
           

        }
        public async Task<ActionResult> Branch()
        {
            var result = await _branchRepository.getdatasync();
            return View(result);
        }

        public async Task<ActionResult> Edit_branch(int id)
        {
            var result = await _branchRepository.getdataByIdsync(id);

            return View(result);
        }

        [HttpPost]
        public async Task<ActionResult> Branch(BranchModel cost)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    DynamicParameters ObjParm = new DynamicParameters();
                    ObjParm.Add("@id", cost.Id);
                    ObjParm.Add("@name", cost.Name);

                    if (cost.Id == 0)
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

                    await _branchRepository.AddAsync(ObjParm);
                    var id = ObjParm.Get<dynamic>("@result");
                    if (id > 0)
                    {
                        TempData["MsgType"] = "Success";
                        if (cost.Id == 0)
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
                return Redirect("../Supplier/Branch");
            }
            catch (Exception ex)
            {
                TempData["MsgType"] = "Error";
                TempData["Msg"] = ex.Message;
                return View();
            }
        }

        public string Delete_branch(int id)
        {
            try
            {
                _branchRepository.DeleteAsync(id);

                return "Record Deleted Successfully";
            }
            catch (Exception ex)
            {


                return ex.Message;
            }


        }

        public async Task<ActionResult> client()
        {
            var result = await _clientRepository.getdatasync();
            return View(result);
        }
        public async Task<ActionResult> Edit_client(int id)
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
        public async Task<ActionResult> Department()
        {
            var result = await _departmentRepository.getdatasync();
            return View(result);
        }

        public async Task<ActionResult> Edit_Department(int id)
        {
            var result = await _departmentRepository.getdataByIdsync(id);

            return View(result);
        }

        [HttpPost]
        public async Task<ActionResult> Department(DepartmentModel Dept)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    DynamicParameters ObjParm = new DynamicParameters();
                    ObjParm.Add("@id", Dept.Id);
                    ObjParm.Add("@name", Dept.Name);

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

                    await _departmentRepository.AddAsync(ObjParm);
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
                return Redirect("../Supplier/Department");
            }
            catch (Exception ex)
            {
                TempData["MsgType"] = "Error";
                TempData["Msg"] = ex.Message;
                return View();
            }
        }

        public string Delete_department(int id)
        {
            try
            {
                _departmentRepository.DeleteAsync(id);

                return "Record Deleted Successfully";
            }
            catch (Exception ex)
            {


                return ex.Message;
            }


        }
    }
}
