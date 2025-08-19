using Microsoft.AspNetCore.Mvc;
using OTS.Data.Models;
using OTS.Data.Repository;


namespace OTS.UI.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILoginRepository _loginrepo;
        private readonly ISendMailRepositery _sendMail;

        public AccountController(ILoginRepository loginrepo,ISendMailRepositery sendMail)
        {
            _loginrepo = loginrepo;
            _sendMail = sendMail;
        }

        public async Task<ActionResult> login()
        {
            // _sendMail.Send_Mail("nileshbahadkar96@gmail.com", "Testing Mail", "Testing Mail From OTS");
            return View();
        }

        [HttpPost]

        public async Task<ActionResult> login(string username, string password,string type)
        {
            if (type == "C")
            {
                var result = await _loginrepo.UserLogin(new CorpUserModel { username = username, password = password,usertype=1 });
                if (result != null)
                {
                    CookieOptions options = new CookieOptions();

                    options.Expires = DateTime.Now.AddDays(1);
                    Response.Cookies.Append("Username", result.firstname + " " + result.lastname, options);
                    Response.Cookies.Append("Userid", result.Id.ToString(), options);
                    Response.Cookies.Append("Usertype", type, options);
                    Response.Cookies.Append("UserRoleType", result.usertype.ToString(), options);
                    Response.Cookies.Append("clientid", result.clientid.ToString(), options);
                    return RedirectToAction("Index", "Client");

                }
                else
                {
                    TempData["MSG"] = "Invalid UserName or Password";
                    return View();
                }
            }
            else if (type == "D")
            {
                var result = await _loginrepo.VendorLogin(new VendorModel { username = username, password = password });
                if (result != null)
                {
                    CookieOptions options = new CookieOptions();

                    options.Expires = DateTime.Now.AddDays(1);
                    Response.Cookies.Append("Username", result.name, options);
                    Response.Cookies.Append("Userid", result.Id.ToString(), options);
                    Response.Cookies.Append("Usertype", type, options);
                    Response.Cookies.Append("clientid", "0", options);
                    return RedirectToAction("Index", "Supplier");

                }
                else
                {
                    TempData["MSG"] = "Invalid UserName or Password";
                    return View();
                }

            }
            else if (type == "U")
            {
                var result = await _loginrepo.UserLogin(new CorpUserModel { username = username, password = password,usertype=0 });
                if (result != null)
                {
                    CookieOptions options = new CookieOptions();

                    options.Expires = DateTime.Now.AddDays(1);
                    Response.Cookies.Append("Username", result.username, options);
                    Response.Cookies.Append("Userid", result.Id.ToString(), options);
                    Response.Cookies.Append("Usertype", type, options);
                    Response.Cookies.Append("UserRoleType", result.usertype.ToString(), options);
                    Response.Cookies.Append("clientid", result.vendorid.ToString(), options);
                    Response.Cookies.Append("limit", result.limitamount.ToString(), options);
                    if (result.usertype == 2)
                    {
                        return RedirectToAction("Index", "Client");
                    }
                    else
                    {
                        return Redirect("../User/Index");
                    }

                }
                else
                {
                    TempData["MSG"] = "Invalid UserName or Password";
                    return View();
                }

            }
            return View();
        }

        public ActionResult Logout()
        {
           // HttpContext.Session.Clear();
            foreach (string cookie in Request.Cookies.Keys)
            {
                Response.Cookies.Delete(cookie);
            }
            return Redirect("../Account/Login"); 
        }
    }
}
