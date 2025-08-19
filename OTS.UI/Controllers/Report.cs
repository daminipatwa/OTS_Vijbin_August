using Microsoft.AspNetCore.Mvc;

namespace OTS.UI.Controllers
{
    public class Report : Controller
    {
        public IActionResult OrderWise()
        {
            return View();
        }

        public IActionResult ProductWise()
        {
            return View();
        }
        public IActionResult BranchWise()
        {
            return View();
        }
        public IActionResult DefaultWise()
        {
            return View();
        }
    }
}
