using CTDT.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Drawing.Printing;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace CTDT.Areas.Admin.Controllers
{
    public class CTDTController : Controller
    {
        dbSurveyEntities db = new dbSurveyEntities();
        // GET: Admin/CTDT
        public ActionResult ViewCTDT()
        {
            ViewBag.KhoaList = new SelectList(db.khoa.OrderBy(l => l.id_khoa), "id_khoa", "ten_khoa");
            return View();
        }
        [HttpGet]
        public ActionResult GetByID(int id)
        {
            var status = "";
            var item = db.ctdt.Where(k => k.id_ctdt == id)
                .Select(k => new
                {
                    MaCTDT = k.id_ctdt,
                    MaKhoa = k.id_khoa,
                    TenCTDT = k.ten_ctdt,
                    NgayTao = k.ngaytao,
                    NgayCapNhat = k.ngaycapnhat
                }).FirstOrDefault();
            return Json(new { data = item, status = status }, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public ActionResult LoadDataCTDT(int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var ctdt = db.ctdt
                    .OrderBy(l => l.id_ctdt)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(c => new
                    {
                        MaCTDT = c.id_ctdt,
                        TenCTDT = c.ten_ctdt,
                        NgayCapNhat = c.ngaycapnhat,
                        NgayTao = c.ngaytao,
                        TenKhoa = c.khoa.ten_khoa,
                    }).ToList();
                var totalRecords = db.ctdt.Count();
                var totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
                return Json(new { data = ctdt, totalPages = totalPages, status = "Load Dữ liệu thành công" }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { status = "Load dữ liệu thất bại" }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public ActionResult Add(ctdt ct)
        {
            var status = "";
            DateTime now = DateTime.UtcNow;
            if (ModelState.IsValid)
            {
                status = "Thêm mới CTĐT thành công";
                int unixTimestamp = (int)(now.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                ct.ngaycapnhat = unixTimestamp;
                ct.ngaytao = unixTimestamp;
                db.ctdt.Add(ct);
                db.SaveChanges();
            }
            return Json(new { status = status }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult Edit(ctdt ct)
        {
            var status = "";
            var cTDT = db.ctdt.Find(ct.id_ctdt);
            if (cTDT != null)
            {
                if (ct.ten_ctdt == null)
                {
                    status = "Không được để trống tên CTĐT";
                }
                else
                {
                    cTDT.id_khoa = ct.id_khoa;
                    cTDT.id_ctdt = ct.id_ctdt;
                    cTDT.ngaycapnhat = ct.ngaycapnhat;
                    cTDT.ngaytao = ct.ngaytao;
                    cTDT.ten_ctdt = ct.ten_ctdt;
                    db.SaveChanges();
                    status = "Cập nhật lại CTĐT thành công";
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
            var ctdtt = db.ctdt.Find(id);
            if (ctdtt != null)
            {
                db.ctdt.Remove(ctdtt);
                db.SaveChanges();
                status = "Xóa CTĐT thành công";
            }
            else
            {
                status = "Xóa CTĐT thất bại";
            }
            return Json(new { status = status }, JsonRequestBehavior.AllowGet);
        }
    }
}