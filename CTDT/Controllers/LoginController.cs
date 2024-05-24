using System.Web;
using System.Web.Mvc;
using Microsoft.Owin.Security;
using System.Security.Claims;
using System.Linq;
using Microsoft.AspNet.Identity;

public class LoginController : Controller
{
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

        // Store user information in session
        var userInfo = new UserInfoViewModel
        {
            Name = loginInfo.ExternalIdentity.Name,
            Email = loginInfo.ExternalIdentity.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value,
            Provider = loginInfo.Login.LoginProvider
        };
        Session[UserInfoSessionKey] = userInfo;

        return RedirectToLocal(returnUrl);
    }

    // GET: /Login/ExternalLoginFailure
    [HttpGet]
    public ActionResult ExternalLoginFailure()
    {
        return View();
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

public class UserInfoViewModel
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string Provider { get; set; }
}
