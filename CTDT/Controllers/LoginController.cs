using System.Web;
using System.Web.Mvc;
using Microsoft.Owin.Security;
using System.Security.Claims;
using System.Linq;
using Microsoft.AspNet.Identity;
using CTDT.Models;
using System;
using CTDT.Helper;

public class LoginController : Controller
{
    dbSurveyEntities db = new dbSurveyEntities();
    private const string XsrfKey = "XsrfId";
    private const string UserInfoSessionKey = "UserInfo";

    // GET: Login
    public ActionResult Login()
    {
        return View();
    }

    // POST: /Login/ExternalLogin
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult ExternalLogin(string provider, string returnUrl)
    {
        var properties = new AuthenticationProperties
        {
            RedirectUri = Url.Action("ExternalLoginCallback", "Login", new { ReturnUrl = returnUrl })
        };

        HttpContext.GetOwinContext().Authentication.Challenge(properties, provider);
        return new HttpUnauthorizedResult();
    }

    // GET: /Login/ExternalLoginCallback
    public ActionResult ExternalLoginCallback(string returnUrl)
    {
        var loginInfo = HttpContext.GetOwinContext().Authentication.GetExternalLoginInfo();
        if (loginInfo == null)
        {
            return RedirectToAction("Index","Home");
        }

        // Log the external identity claims
        var externalClaims = loginInfo.ExternalIdentity.Claims.ToList();
        foreach (var claim in externalClaims)
        {
            System.Diagnostics.Debug.WriteLine($"Claim Type: {claim.Type}, Value: {claim.Value}");
        }

        // Continue with your login process...
        var identity = new ClaimsIdentity(loginInfo.ExternalIdentity.Claims, DefaultAuthenticationTypes.ApplicationCookie);
        HttpContext.GetOwinContext().Authentication.SignIn(new AuthenticationProperties { IsPersistent = false }, identity);

        var email = loginInfo.ExternalIdentity.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        var avatarUrl = loginInfo.ExternalIdentity.Claims.FirstOrDefault(c => c.Type == "urn:google:picture")?.Value;
        string userIpAddress = GetClientIpAddress();

        var user = db.users.FirstOrDefault(u => u.email == email);
        if (user == null)
        {
            user = new users
            {
                name = loginInfo.ExternalIdentity.Name,
                email = email,
                username = null,
                password = null,
                id_typeusers = 1,
                ngaycapnhat = (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds,
                ngaytao = (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds,
                userIpAddress = userIpAddress,
                avatarUrl = avatarUrl
            };

            db.users.Add(user);
            db.SaveChanges();
        }
        else
        {
            user.userIpAddress = userIpAddress;
            user.ngaycapnhat = (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
            user.avatarUrl = avatarUrl;
            db.Entry(user).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
        }

        SessionHelper.SetUser(user);
        //if (user.id_typeusers == 3)
        //{
        //    return RedirectToAction("TKSVCKS", "ThongKeKhaoSat", new { area = "CTDT" });
        //}

        //if (user.id_typeusers == 2)
        //{
        //    return RedirectToAction("Index", "PhieuKhaoSat", new { area = "Admin" });
        //}
        return RedirectToLocal(returnUrl);
    }
    private string GetClientIpAddress()
    {
        string ipAddress = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

        if (!string.IsNullOrEmpty(ipAddress))
        {
            string[] addresses = ipAddress.Split(',');
            if (addresses.Length > 0)
            {
                return addresses[0].Trim();
            }
        }
        return Request.ServerVariables["REMOTE_ADDR"];
    }

    // GET: /Login/ExternalLoginFailure
    [HttpGet]
    public ActionResult ExternalLoginFailure()
    {
        return View();
    }

    // POST: /Login/Logout
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Logout()
    {
        HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
        SessionHelper.ClearUser();
        return RedirectToAction("Index", "Home");
    }

    private ActionResult RedirectToLocal(string returnUrl)
    {
        if (Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }
        return RedirectToAction("Index", "Home");
    }

    private void AddErrors(IdentityResult result)
    {
        foreach (var error in result.Errors)
        {
            ModelState.AddModelError("", error);
        }
    }

    private class ChallengeResult : HttpUnauthorizedResult
    {
        public ChallengeResult(string provider, string redirectUri)
            : this(provider, redirectUri, null)
        {
        }

        public ChallengeResult(string provider, string redirectUri, string userId)
        {
            LoginProvider = provider;
            RedirectUri = redirectUri;
            UserId = userId;
        }

        public string LoginProvider { get; set; }
        public string RedirectUri { get; set; }
        public string UserId { get; set; }

        public override void ExecuteResult(ControllerContext context)
        {
            var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
            if (UserId != null)
            {
                properties.Dictionary[XsrfKey] = UserId;
            }
            context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
        }
    }
}   