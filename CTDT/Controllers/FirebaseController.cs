using System.Collections.Generic;
using System.Configuration;
using System.Web.Mvc;

namespace CTDT.Controllers
{
    public class FirebaseController : Controller
    {
        [HttpGet]
        public ActionResult GetConfig()
        {
            var firebaseConfig = new Dictionary<string, string>
            {
                { "apiKey", ConfigurationManager.AppSettings["Firebase:apiKey"] },
                { "authDomain", ConfigurationManager.AppSettings["Firebase:authDomain"] },
                { "projectId", ConfigurationManager.AppSettings["Firebase:projectId"] },
                { "storageBucket", ConfigurationManager.AppSettings["Firebase:storageBucket"] },
                { "messagingSenderId", ConfigurationManager.AppSettings["Firebase:messagingSenderId"] },
                { "appId", ConfigurationManager.AppSettings["Firebase:appId"] },
                { "measurementId", ConfigurationManager.AppSettings["Firebase:measurementId"] }
            };

            return Json(firebaseConfig, JsonRequestBehavior.AllowGet);
        }
    }
}
