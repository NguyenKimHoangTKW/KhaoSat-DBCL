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
            ViewBag.id = id;
            string jsonData = string.Join("", db.survey.Where(d => d.surveyID == id).Select(x => x.surveyData));

            jsonData = Regex.Unescape(jsonData);

            JObject jsonObject = JObject.Parse(jsonData);

            return View(jsonObject);
        }
        [HttpPost]
        public ActionResult AddAnswer(answer_response answer)
        {
            var getuser = SessionHelper.GetUser();

            int? getctdt = TryParseSessionValue("XTCTDT");
            int? getsv = TryParseSessionValue("XTSV");
            int? getdonvi = TryParseSessionValue("XTDV");
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
                answer.id_donvi = getdonvi;
                db.answer_response.Add(answer);
                db.SaveChanges();
            }
            return Json(new { status = status }, JsonRequestBehavior.AllowGet);
        }

        private int? TryParseSessionValue(string key)
        {
            string sessionValue = Session[key] as string;
            if (int.TryParse(sessionValue, out int result))
            {
                return result;
            }
            return null;
        }

        // Load Biễu mẫu đã khảo sát
        public ActionResult SurveyForm(int iduser)
        {
            var listsurvey = db.answer_response.Where(x => x.id_users == iduser).ToList();
            var surveyIds = listsurvey.Select(x => x.surveyID).ToList();
            ViewBag.surveyIds = surveyIds;
            return View(listsurvey);
        }

        [HttpGet]
        public ActionResult LoadSurveyForm(string ids)
        {
            if (string.IsNullOrEmpty(ids))
            {
                return Json(new { data = new List<object>(), status = "Không có dữ liệu." }, JsonRequestBehavior.AllowGet);
            }

            var surveyIds = ids.Split(',').Select(int.Parse).ToList();
            var ListSurveyForm = db.survey
                .Where(x => surveyIds.Contains(x.surveyID))
                .Select(f => new
                {
                    MaPhieu = f.surveyID,
                    TieuDePhieu = f.surveyTitle,
                    MoTaPhieu = f.surveyDescription,
                }).ToList();

            if (ListSurveyForm.Count == 0)
            {
                return Json(new { data = new List<object>(), status = "Không có dữ liệu." }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { data = ListSurveyForm, status = "Load dữ liệu thành công" }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult ListAnswerSurvey(int id)
        {
            ViewBag.id = id;
            var answerlist = db.answer_response.Where(x=>x.surveyID == id).ToList();
            return View(answerlist);
        }

        [HttpGet]
        public ActionResult LoadListAnswerSurvey(int id, int iduser)
        {
            var listsurvey = db.answer_response
                .Where(x => x.surveyID == id && x.id_users == iduser)
                .Select(f => new
                {
                    TenCTDT = f.ctdt.ten_ctdt,
                    TenSV = f.sinhvien.hovaten,
                    MSSV = f.sinhvien.ma_sv,
                    Khoa = f.ctdt.khoa.ten_khoa,
                    MaPhieu = f.id,
                    Donvi = f.DonVi.name_donvi,
                    CBVC = f.CanBoVienChuc.TenCBVC,
                    ThoiGianKhaoSat = f.time,
                })
                .ToList();

            return Json(new { data = listsurvey }, JsonRequestBehavior.AllowGet);
        }

        //


        // Kết quả phiếu khảo sát Client
        public ActionResult AnswerPKS(int id)
        {
            ViewBag.id = id;
            var litsKQ = db.answer_response.Where(kq => kq.id == id).ToList();
            foreach (var item in litsKQ)
            {
                ViewBag.IDPhieu = item.surveyID;
            }
            return View(litsKQ);
        }
        public ActionResult AnswerSurvey(int id)
        {
            var answers = db.answer_response.Where(d => d.id == id).Select(x => x.json_answer).ToList();
            List<JObject> surveyData = new List<JObject>();
            foreach (var answer in answers)
            {
                JObject answerObject = JObject.Parse(answer);
                surveyData.Add(answerObject);
            }
            return Content(JsonConvert.SerializeObject(surveyData), "application/json");
        }
        //
    }
}