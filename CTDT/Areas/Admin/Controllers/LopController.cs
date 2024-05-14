using CTDT.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CTDT.Areas.Admin.Controllers
{
    public class LopController : Controller
    {
        dbSurveyEntities db = new dbSurveyEntities();
        // GET: Admin/Lop
        public ActionResult ViewLop()
        {
            ViewBag.CTDTList = new SelectList(db.ctdt.OrderBy(l => l.id_ctdt), "id_ctdt", "ten_ctdt");
            return View();
        }
        [HttpPost]
        public ActionResult Add(lop l)
        {
            var status = "";
            DateTime now = DateTime.UtcNow;
            if (ModelState.IsValid)
            {
                status = "Thêm mới Lớp thành công";
                int unixTimestamp = (int)(now.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                l.ngaycapnhat = unixTimestamp;
                l.ngaytao = unixTimestamp;
                db.lop.Add(l);
                db.SaveChanges();
            }
            return Json(new { status = status }, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult LoadDataLop()
        {
            try
            {
                var lop = db.lop.Select(l => new
                {
                    IdLop = l.id_lop,
                    MaCTDT = l.ctdt.ten_ctdt,
                    MaLop = l.ma_lop,
                    NgayCapNhat = l.ngaycapnhat,
                    NgayTao = l.ngaytao,
                }).ToList();
                return Json(new { data = lop, status = "Load dữ liệu thành công" }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { status = "Load dữ liệu thất bại"}, JsonRequestBehavior.AllowGet);
            }
        }
    }
}