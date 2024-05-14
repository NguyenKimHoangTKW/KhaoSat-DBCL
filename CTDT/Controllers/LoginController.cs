using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CTDT.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        [HttpGet]
        public ActionResult Login(string returnUrl = "/")
        {
            // Gọi phương thức Challenge để bắt đầu quá trình đăng nhập bằng Google.
            return View(new AuthenticationProperties { RedirectUri = returnUrl });
        }


    }
}