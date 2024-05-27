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
        // Request a redirect to the external login provider
        return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Login", new { ReturnUrl = returnUrl }));
    }

    // GET: /Login/ExternalLoginCallback
    public ActionResult ExternalLoginCallback(string returnUrl)
    {
        var loginInfo = HttpContext.GetOwinContext().Authentication.GetExternalLoginInfo();
        if (loginInfo == null)
        {
            return RedirectToAction("Login");
        }

        // Sign in the user with this external login provider if the user already has a login
        var identity = new ClaimsIdentity(loginInfo.ExternalIdentity.Claims, DefaultAuthenticationTypes.ApplicationCookie);
        HttpContext.GetOwinContext().Authentication.SignIn(new AuthenticationProperties { IsPersistent = false }, identity);

        // Extract user information from loginInfo
        var email = loginInfo.ExternalIdentity.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

        // Check if the user already exists in the database
        var user = db.users.FirstOrDefault(u => u.email == email);
        if (user == null)
        {
            // Create a new user record
            user = new users
            {
                name = loginInfo.ExternalIdentity.Name,
                email = email,
                username = null,
                password = null, 
                id_typeusers = 1,
                ngaycapnhat = (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds,
                ngaytao = (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds
            };

            db.users.Add(user);
            db.SaveChanges();
        }

        // Store user information in session
        SessionHelper.SetUser(user);

        return RedirectToLocal(returnUrl);
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
