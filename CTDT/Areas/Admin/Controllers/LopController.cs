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
        public ActionResult LoadDataLop(int pageNumber = 1, int pageSize = 10, string keyword = "")
        {
            try
            {
                IQueryable<lop> query = db.lop;
                if (!string.IsNullOrEmpty(keyword))
                {
                    query = query.Where(l => l.ma_lop.ToLower().Contains(keyword.ToLower())
                                          || l.ctdt.ten_ctdt.ToLower().Contains(keyword.ToLower()));
                }

                var lop = query
                    .OrderBy(l => l.id_lop)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(l => new
                    {
                        IdLop = l.id_lop,
                        MaCTDT = l.ctdt.ten_ctdt,
                        MaLop = l.ma_lop,
                        NgayCapNhat = l.ngaycapnhat,
                        NgayTao = l.ngaytao,
                    }).ToList();

                var totalRecords = query.Count();
                var totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

                return Json(new { data = lop, totalPages = totalPages, status = "Load dữ liệu thành công" }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { status = "Load dữ liệu thất bại" }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpGet]
        public ActionResult GetByID(int id)
        {
            var item = db.lop.Where(k => k.id_lop == id)
                .Select(k => new
                {
                    id_lop = k.id_lop,
                    id_ctdt = k.id_ctdt,
                    ma_lop = k.ma_lop,
                    ngaytao = k.ngaytao,
                    ngaycapnhat = k.ngaycapnhat
                }).FirstOrDefault();

            var status = item != null ? "Success" : "Not Found";
            return Json(new { data = item, status = status }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Edit(lop l)
        {
            DateTime now = DateTime.UtcNow;
            int unixTimestamp = (int)(now.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            var status = "";

            var lopp = db.lop.Find(l.id_lop);
            if (lopp != null)
            {
                if (string.IsNullOrEmpty(l.ma_lop))
                {
                    status = "Tên lớp không được để trống";
                }
                else if (db.lop.Any(x => x.ma_lop == l.ma_lop && x.id_lop != l.id_lop))
                {
                    status = "Tên lớp đang bị trùng";
                }
                else
                {
                    lopp.ma_lop = l.ma_lop;
                    lopp.id_ctdt = l.id_ctdt;
                    lopp.ngaycapnhat = unixTimestamp;
                    db.SaveChanges();
                    status = "Cập nhật thông tin thành công";
                }
            }
            else
            {
                status = "Cập nhật thông tin thất bại";
            }

            return Json(new { status = status }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult Delete(int id)
        {
            var status = "";
            try
            {
                var lop = db.lop.Find(id);
                if (lop != null)
                {
                    db.lop.Remove(lop);
                    db.SaveChanges();
                    status = "Xóa lớp thành công";
                }
                else
                {
                    status = "Không tìm thấy lớp cần xóa";
                }
            }
            catch (Exception ex)
            {
                status = "Xóa lớp thất bại: " + ex.Message;
            }
            return Json(new { status = status }, JsonRequestBehavior.AllowGet);
        }
    }
}