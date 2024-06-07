using System.Web;
using System.Web.Mvc;
using Microsoft.Owin.Security;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using CTDT.Models;
using System.Linq;
using System;
using CTDT.Helper;

public class LoginController : Controller
{
    dbSurveyEntities db = new dbSurveyEntities();
    private IAuthenticationManager AuthenticationManager => HttpContext.GetOwinContext().Authentication;

    [AllowAnonymous]
    public ActionResult Login(string returnUrl)
    {
        ViewBag.ReturnUrl = returnUrl;
        return View();
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public ActionResult ExternalLogin(string provider, string returnUrl)
    {
        return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Login", new { ReturnUrl = returnUrl }));
    }

    [AllowAnonymous]
    public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
    {
        var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
        if (loginInfo == null)
        {
            return RedirectToAction("Login");
        }

        var email = loginInfo.Email;
        var name = loginInfo.ExternalIdentity.FindFirstValue(ClaimTypes.Name);
        var profilePictureClaim = loginInfo.ExternalIdentity.Claims.FirstOrDefault(c => c.Type == "picture");
        var profilePictureUrl = profilePictureClaim?.Value;
        DateTime now = DateTime.UtcNow;
        // Create a claims identity and add the email claim
        var identity = new ClaimsIdentity(loginInfo.ExternalIdentity.Claims, DefaultAuthenticationTypes.ApplicationCookie);
        identity.AddClaim(new Claim(ClaimTypes.Email, email));
        var user = db.users.SingleOrDefault(u => u.email == email);
        int unixTimestamp = (int)(now.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        if (user == null)
        {
            user = new users
            {
                email = email,
                name = name,
                id_typeusers = 1,
                ngaytao = unixTimestamp,
                ngaycapnhat = unixTimestamp,
                avatarUrl = profilePictureUrl
            };
            db.users.Add(user);
            db.SaveChanges();
        }
        else
        {
            user.name = name;
            user.ngaycapnhat = unixTimestamp;
            user.avatarUrl = profilePictureUrl;
            db.SaveChanges();
        }
        AuthenticationManager.SignIn(new AuthenticationProperties { IsPersistent = false }, identity);
        SessionHelper.SetUser(user);

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

    internal class ChallengeResult : HttpUnauthorizedResult
    {
        public ChallengeResult(string provider, string redirectUri)
        {
            LoginProvider = provider;
            RedirectUri = redirectUri;
        }

        public string LoginProvider { get; set; }
        public string RedirectUri { get; set; }

        public override void ExecuteResult(ControllerContext context)
        {
            var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
            context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
        }
    }
}
