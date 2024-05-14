using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
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
    }
}