using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OTS.Controllers
{
    public class MastersController : Controller
    {
        // GET: Masters
        public ActionResult CostCenter()
        {
            return View();
        }

        public ActionResult Branch()
        {
            return View();
        }

        public ActionResult client()
        {
            return View();
        }
        public ActionResult Department()
        {
            return View();
        }
    }
}