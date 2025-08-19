using Dapper;
using DocumentFormat.OpenXml.EMMA;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OTS.Data.Models;
using OTS.Data.Repository;
using System.Data;

namespace OTS.UI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class commonapiController : ControllerBase
    {
        private readonly Iproductrepository _productrepository;
        private readonly IcartRepository _cartrepository;
        private readonly IClientRepository _clientRepository;
        private readonly ICostRepository _costRepository;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IBranchRepository _branchRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly ICorpRepository _corpRepository;

        public commonapiController(Iproductrepository productrepository, ICorpRepository corpRepository, IcartRepository cartrepository, IClientRepository clientRepository, ICostRepository costRepository, IDepartmentRepository departmentRepository, IBranchRepository branchRepository, IOrderRepository orderRepository)
        {
            _productrepository = productrepository;
            _corpRepository = corpRepository;
            _cartrepository = cartrepository;
            _clientRepository = clientRepository;
            _costRepository = costRepository;
            _departmentRepository = departmentRepository;
            _branchRepository = branchRepository;
            _orderRepository = orderRepository;
        }

        
        [HttpGet]
        public async Task<ActionResult> pincode_data()
        {
            List<NotificationModel> Modellist = new List<NotificationModel>();
            var result = await _clientRepository.getnotoficationsync(Convert.ToInt32(Request.Cookies["clientid"].ToString()), "Client");
            foreach (var item in result)
            {
                NotificationModel Model = new NotificationModel();
                Model.url = item.url;
                Model.msg = item.msg;
                Modellist.Add(Model);
            }
            return Ok(Modellist);
        }

        [HttpGet]
        public async Task<ActionResult> single_reject_challan(int id,int status,string remark)
        {

            DynamicParameters ObjParm = new DynamicParameters();

            ObjParm.Add("@userid", Convert.ToInt32(Request.Cookies["userid"].ToString()));
            ObjParm.Add("@status", status);
            ObjParm.Add("@remark", remark);
            ObjParm.Add("@id", id);
            await _corpRepository.single_reject_challan(ObjParm);
           
            return Ok("Success");
        }
    }
}
