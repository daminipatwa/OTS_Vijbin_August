using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OTS.Controllers
{
    public class accountController : Controller
    {
        // GET: account
        public ActionResult login()
        {
            return View();
        }
    }
}