using CTDT.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace CTDT.Areas.Admin.Controllers
{
    public class PhieuKhaoSatController : Controller
    {
        dbSurveyEntities db = new dbSurveyEntities();
        // GET: Admin/PhieuKhaoSat
        public ActionResult Index()
        {
            ViewBag.HDT = new SelectList(db.hedaotao, "id_hedaotao", "ten_hedaotao");
            ViewBag.LKS = new SelectList(db.LoaiKhaoSat, "id_loaikhaosat", "name_loaikhaosat");
            return View();
        }
        [HttpPost]
        public ActionResult NewSurvey(survey s)
        {
            var status = "";
            DateTime now = DateTime.UtcNow;
            if (ModelState.IsValid)
            {
                int unixTimestamp = (int)(now.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                s.surveyTimeMake = unixTimestamp;
                s.surveyTimeUpdate = unixTimestamp;
                db.survey.Add(s);
                db.SaveChanges();
                status = "Tạo mới phiếu khảo sát thành công";
            }
            return Json(new { status = status}, JsonRequestBehavior.AllowGet);
        }

        public ActionResult KetQuaPKS(int id)
        {
            ViewBag.id = id;
            var litsKQ = db.answer_response.Where(kq => kq.surveyID == id).ToList();
            return View(litsKQ);
        }
        [HttpGet]
        public ActionResult LoadPhieu()
        {
            var ListPhieu = db.survey.Select(p => new
            {
                MaPhieu = p.surveyID,
                TenHDT = p.hedaotao.ten_hedaotao,
                MaHDT = p.id_hedaotao,
                TieuDePhieu = p.surveyTitle,
                MoTaPhieu = p.surveyDescription,
                NgayTao = p.surveyTimeStart,
                NgayChinhSua = p.surveyTimeMake,
                LoaiKhaoSat = p.LoaiKhaoSat.name_loaikhaosat,
                MaLKS = p.id_loaikhaosat,
            }).ToList();
            return Json(new { data = ListPhieu, status = "Load dữ liệu thành công" }, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public ActionResult LoadKetQuaPKS(int id)
        {
            var ListKQPKS = db.answer_response.Where(kq => kq.surveyID == id).Select(kq => new
            {
                MaKQ = kq.id,
                Email = kq.users.email,
                SinhVien = kq.sinhvien.hovaten,
                ThoiGianThucHien = kq.time,
            }).ToList();
            return Json(new {status= "Load dữ liệu thành công" , data = ListKQPKS }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AnswerSurvey(int id)
        {
            var answers = db.answer_response.Where(d => d.surveyID == id).Select(x => x.json_answer).ToList();
            string jsonData = "[" + string.Join(",", answers) + "]";
            JArray surveyData = JArray.Parse(jsonData);
            return Content(surveyData.ToString(), "application/json");
        }
    }
}