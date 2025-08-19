using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OTS.Controllers
{
    public class UserController : Controller
    {
        // GET: User
        public ActionResult Add()
        {
            return View();
        }

        public ActionResult List()
        {
            return View();
        }
    }
}