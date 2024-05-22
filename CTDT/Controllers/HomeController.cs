using CTDT.Models;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace CTDT.Controllers
{
    public class HomeController : Controller
    {
        dbSurveyEntities db = new dbSurveyEntities();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult PhieuKhaoSat(int id)
        {
            Session["IDPhieu"] = id;
            ViewBag.id = Session["IDPhieu"];
            var phieukhaosat = db.survey
                                 .Where(x => x.surveyID == id)
                                 .Select(x => new
                                 {
                                     MaPhieu = x.surveyID,
                                     TenPhieu = x.hedaotao.ten_hedaotao,
                                 })
                                 .ToList();
            return View(phieukhaosat);
        }

        [HttpGet]
        public ActionResult LoadHeDaoTao()
        {
            var hedaotao = db.hedaotao.Select(c => new
            {
                MaHDT = c.id_hedaotao,
                TenHDT = c.ten_hedaotao,
            }).ToList();

            return Content(JsonConvert.SerializeObject(new { data = hedaotao, TotalItems = hedaotao.Count, status = "Load Dữ liệu thành công" }), "application/json");
        }

        [HttpGet]
        public ActionResult LoadPhieuKhaoSat(int id)
        {
            var phieukhaosat = db.survey.Where(c=>c.surveyStatus == 1).Select(c => new
            {
                MaPhieu = c.surveyID,
                TenPKS = c.surveyTitle,
                MoTaPhieu = c.surveyDescription,
                MaHDT = c.id_hedaotao,
                TenHDT = c.hedaotao.ten_hedaotao,
                TenLoaiKhaoSat = c.LoaiKhaoSat.name_loaikhaosat,
            }).Where(p => p.MaHDT == id).ToList();
            return Content(JsonConvert.SerializeObject(new { data = phieukhaosat, TotalItems = phieukhaosat.Count, status = "Load Dữ liệu thành công" }), "application/json");
        }

        public ActionResult XacThucCTDT(int id)
        {
            Session["IDPhieu"] = id;
            var xacthuc = db.survey.Where(x => x.surveyID == id).ToList();
            ViewBag.KhoaList = new SelectList(db.khoa.OrderBy(l => l.id_khoa), "id_khoa", "ten_khoa");
            return View(xacthuc);
        }
        public ActionResult XacThuc(int id)
        {

            ViewBag.id = Session["IDPhieu"];
            var xacthuc = db.survey.Where(x => x.surveyID == id).ToList();
            ViewBag.KhoaList = new SelectList(db.khoa.OrderBy(l => l.id_khoa), "id_khoa", "ten_khoa");
            return View();
        }
        // Load CTDT từ Khoa
        public ActionResult LoadCTDTByKhoa(int khoaId)
        {
            db.Configuration.ProxyCreationEnabled = false;
            List<ctdt> ctdt = db.ctdt.Where(x=>x.id_khoa == khoaId).ToList();
            return Json(ctdt, JsonRequestBehavior.AllowGet);
        }
        // Load Lớp từ CTDT
        public ActionResult LoadLopByCTDT(int CTDTID)
        {
            db.Configuration.ProxyCreationEnabled = false;
            List<lop> lop = db.lop.Where(x => x.id_ctdt == CTDTID).ToList();
            return Json(lop, JsonRequestBehavior.AllowGet);
        }
        // Load Sinh Viên từ Lớp
        public ActionResult LoadSVByLop(int LopID)
        {
            db.Configuration.ProxyCreationEnabled = false;
            List<sinhvien> sinhvien = db.sinhvien.Where(x => x.id_lop == LopID).ToList();
            return Json(sinhvien, JsonRequestBehavior.AllowGet);
        }



        [HttpPost]
        public ActionResult SaveDataXacThucCTDT(string khoa, string ctdt)
        {
            try
            {
                Session["SelectedKhoaByXTCTDT"] = khoa;
                Session["SelectedCTDTByXTCTDT"] = ctdt;

                int idPhieu = Convert.ToInt32(Session["IDPhieu"]);

                return Json(new { success = true, idPhieu = idPhieu });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult SaveDataXacThuc(string khoa, string ctdt, string lop, string sv)
        {
            try
            {
                Session["SelectedCTDTByXT"] = ctdt;
                Session["SelectedKhoaByXT"] = khoa;
                Session["SelectedLopByXT"] = lop;
                Session["SelectedSvByXT"] = sv;
                int idPhieu = Convert.ToInt32(Session["IDPhieu"]);

                return Json(new { success = true, idPhieu = idPhieu });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult ClearSession()
        {
            Session.Remove("SelectedKhoaByXT");
            Session.Remove("SelectedCTDTByXT");
            Session.Remove("SelectedLopByXT");
            Session.Remove("SelectedSvByXT");
            Session.Remove("SelectedKhoaByXTCTDT");
            Session.Remove("SelectedCTDTByXTCTDT");
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }


        public ActionResult test()
        {

            return View();
        }
    }
}