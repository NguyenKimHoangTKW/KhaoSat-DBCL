using CTDT.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CTDT.Areas.Admin.Controllers
{
    public class ThongKeKhaoSatController : Controller
    {
        dbSurveyEntities db = new dbSurveyEntities();
        // GET: Admin/ThongKeKhaoSat
        public ActionResult ThongKeSVChuaKhaoSat()
        {
            ViewBag.CTDTList = new SelectList(db.ctdt.OrderBy(l => l.id_ctdt), "id_ctdt", "ten_ctdt");
            ViewBag.PKSList = new SelectList(db.survey.OrderBy(l => l.surveyID), "surveyID", "surveyTitle");
            return View();
        }
        [HttpGet]
        public ActionResult LoadSVChuaKhaoSat(int ctdt = 0, int survey = 0)
        {
            var query = db.answer_response.AsQueryable();
            if (ctdt != 0)
            {
                query = query.Where(ct => ct.ctdt.id_ctdt == ctdt);
            }
            if(survey != 0)
            {
                query = query.Where(sr => sr.surveyID == survey);
            }
            var GetSV = query
                .Where(sv => !db.sinhvien.Any(aw => aw.id_sv == sv.id_sv))
                .AsEnumerable()
                .Select(x => new
                {
                    MSSV = x.sinhvien.ma_sv,
                    Hoten = x.sinhvien.hovaten,
                    NgaySinh = x.sinhvien.ngaysinh.ToString("yyyy-MM-dd"),
                    SDT = x.sinhvien.sodienthoai,
                    CTDT = x.ctdt.ten_ctdt,
                    Lop = x.sinhvien.lop.ma_lop,
                })
                .ToList();

            return Json(new { data = GetSV }, JsonRequestBehavior.AllowGet);
        }
    }
}