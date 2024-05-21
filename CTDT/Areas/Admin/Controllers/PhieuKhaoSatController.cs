using CTDT.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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
            return View();
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
                TieuDePhieu = p.surveyTitle,
                MoTaPhieu = p.surveyDescription,
                NgayTao = p.surveyTimeStart,
                NgayChinhSua = p.surveyTimeMake,
                LoaiKhaoSat = p.LoaiKhaoSat.name_loaikhaosat,
            }).ToList();
            return Json(new {data = ListPhieu, status = "Load dữ liệu thành công"}, JsonRequestBehavior.AllowGet);
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
    }
}