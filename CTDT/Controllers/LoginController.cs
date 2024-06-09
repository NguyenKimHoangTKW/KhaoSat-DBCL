using CTDT.Helper;
using CTDT.Models;
using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

public class LoginController : Controller
{
    dbSurveyEntities db = new dbSurveyEntities();
    private static bool firebaseInitialized = false;
    private static readonly object lockObj = new object();

    private void InitializeFirebase()
    {
        if (!firebaseInitialized)
        {
            lock (lockObj)
            {
                if (!firebaseInitialized)
                {
                    var pathToServiceAccountKey = Server.MapPath("~/App_Data/serviceAccountKey.json");
                    FirebaseApp.Create(new AppOptions()
                    {
                        Credential = GoogleCredential.FromFile(pathToServiceAccountKey),
                    });
                    firebaseInitialized = true;
                }
            }
        }
    }

    [HttpPost]
    public async Task<ActionResult> LoginWithGoogle(string token)
    {
        InitializeFirebase();

        try
        {
            var decodedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(token);
            var uid = decodedToken.Uid;
            UserRecord userRecord = await FirebaseAuth.DefaultInstance.GetUserAsync(uid);
            string email = userRecord.Email;
            string name = userRecord.DisplayName;
            string avatarUrl = userRecord.PhotoUrl;
            DateTime now = DateTime.UtcNow;
            int unixTimestamp = (int)(now.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            var user = db.users.FirstOrDefault(u => u.email == email);
            if (user == null)
            {
                user = new users
                {
                    email = email,
                    name = name,
                    avatarUrl = avatarUrl,
                    username = email,
                    ngaycapnhat = unixTimestamp,
                    ngaytao = unixTimestamp,
                    id_typeusers = 1
                };
                db.users.Add(user);
            }
            else
            {
                user.name = name;
                user.avatarUrl = avatarUrl;
                user.ngaytao = unixTimestamp;
            }
            db.SaveChanges();
            SessionHelper.SetUser(user);
            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Logout()
    {
        HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
        SessionHelper.ClearUser();
        return RedirectToAction("Index", "Home");
    }
}
