using Microsoft.AspNetCore.Mvc;
using OTS.Data.Models;
using OTS.Data.Repository;
using OTS.UI.Models;
using System.Diagnostics;

namespace OTS.UI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IClientRepository _clientRepository;

        public HomeController(ILogger<HomeController> logger,IClientRepository clientRepository)
        {
            _logger = logger;
            _clientRepository = clientRepository;
        }

        public async Task<ActionResult> getnotification()
        {
            List<NotificationModel> Modellist = new List<NotificationModel>();
            var result = await _clientRepository.getnotoficationsync(Convert.ToInt32(Request.Cookies["clientid"].ToString()), "Client");
            foreach (var item in result)
            {
                NotificationModel Model = new NotificationModel();
                Model.id = item.id;
                Model.url = item.url;
                Model.msg = item.msg;
              
                Modellist.Add(Model);
            }
            return Ok(Modellist);
        }

        public async Task<ActionResult> getnotification1()
        {
            List<NotificationModel> Modellist = new List<NotificationModel>();
            var result = await _clientRepository.getnotoficationsync(Convert.ToInt32(Request.Cookies["userid"].ToString()), "Supplier");
            foreach (var item in result)
            {
                NotificationModel Model = new NotificationModel();
                Model.id = item.id;
                Model.url = item.url;
                Model.msg = item.msg;

                Modellist.Add(Model);
            }
            return Ok(Modellist);
        }

        public async Task<string> update_notification(int id)
        {
            await _clientRepository.update_notification(id);
            return "Success";
        }
        public ActionResult Index()
        { 
            return View();
        }
        public ActionResult About()
        {
            return View();
        }

        public ActionResult franchises()
        {
            return View();
        }
    }
}
