using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using CTDT.Helper;
using CTDT.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
namespace CTDT.Controllers
{
    public class SurveyController : Controller
    {
        dbSurveyEntities db = new dbSurveyEntities();
        // GET: Survey
        public ActionResult Survey(int id)
        {
            string jsonData = string.Join("", db.survey.Where(d => d.surveyID == id).Select(x => x.surveyData));

            jsonData = Regex.Unescape(jsonData);

            JObject jsonObject = JObject.Parse(jsonData);

            return View(jsonObject);
        }
        [HttpPost]
        public ActionResult AddAnswer(answer_response answer)
        {
            var getuser = SessionHelper.GetUser();
            string getctdtString = Session["XTCTDT"] as string;
            string getsvString = Session["XTSV"] as string;

            int? getctdt = null;
            int? getsv = null;

            if (int.TryParse(getctdtString, out int ctdtResult))
            {
                getctdt = ctdtResult;
            }

            if (int.TryParse(getsvString, out int svResult))
            {
                getsv = svResult;
            }

            var status = "";
            if (ModelState.IsValid)
            {
                status = "Khảo sát thành công";
                DateTime now = DateTime.UtcNow;
                int unixTimestamp = (int)(now.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                answer.time = unixTimestamp;
                answer.id_users = getuser.id_users;
                answer.id_ctdt = getctdt;
                answer.id_sv = getsv;
                db.answer_response.Add(answer);
                db.SaveChanges();
            }

            return Json(new { status = status }, JsonRequestBehavior.AllowGet);
        }
    }
}