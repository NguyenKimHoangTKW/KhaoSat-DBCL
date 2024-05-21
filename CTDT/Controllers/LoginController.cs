﻿using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;

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