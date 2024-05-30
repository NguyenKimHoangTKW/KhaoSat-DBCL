using CTDT.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CTDT.Areas.Admin.Controllers
{
    public class NguoiDungController : Controller
    {
        dbSurveyEntities db = new dbSurveyEntities();

        // GET: Admin/NguoiDung
        public ActionResult ViewUser()
        {
            ViewBag.CTDTList = new SelectList(db.ctdt.OrderBy(l => l.id_ctdt), "id_ctdt", "ten_ctdt");
            ViewBag.TypeUserList = new SelectList(db.typeusers.OrderBy(l => l.id_typeusers), "id_typeusers", "name_typeusers");
            return View();
        }

        [HttpGet]
        public ActionResult GetByID(int id)
        {
            var item = db.users.Where(k => k.id_users == id)
                .Select(x => new
                {
                    id_users = x.id_users,
                    name = x.name,
                    email = x.email,
                    id_typeusers = x.id_typeusers,
                    id_ctdt = x.id_ctdt,
                    ngaycapnhat = x.ngaycapnhat,
                    ngaytao = x.ngaytao
                }).FirstOrDefault();

            if (item == null)
            {
                return Json(new { status = "Không tìm thấy người dùng" }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { data = item, status = "Load dữ liệu thành công" }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult LoadUser(int pageNumber = 1, int pageSize = 10, string keyword = "")
        {
            try
            {
                IQueryable<users> query = db.users;
                if (!string.IsNullOrEmpty(keyword))
                {
                    query = query.Where(l => l.email.ToLower().Contains(keyword.ToLower())
                                          || l.name.ToLower().Contains(keyword.ToLower()));
                }

                var Listuser = query
                .OrderBy(l => l.id_users)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new
                {
                    MaUser = x.id_users,
                    TenUser = x.name,
                    EmailUser = x.email,
                    MaChucVu = x.id_typeusers,
                    ChucVu = x.typeusers.name_typeusers,
                    NgayCapNhat = x.ngaycapnhat,
                    NgayTao = x.ngaytao,
                    TypeCTDT = x.ctdt.ten_ctdt
                }).ToList();

                var totalRecords = db.users.Count();
                var totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

                return Json(new { data = Listuser, totalPages = totalPages, status = "Load dữ liệu thành công" }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { status = "Load dữ liệu thất bại" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult Edit(users us)
        {
            DateTime now = DateTime.UtcNow;
            var status = "";
            var user = db.users.Find(us.id_users);
            int unixTimestamp = (int)(now.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            if (user != null)
            {
                user.name = us.name;
                user.email = us.email;
                user.ngaytao = us.ngaytao;
                user.ngaycapnhat = unixTimestamp;
                user.id_ctdt = us.id_ctdt;
                user.id_typeusers = us.id_typeusers;
                db.SaveChanges();
                status = "Cập nhật thông tin tài khoản thành công";
            }
            else
            {
                status = "Cập nhật thông tin tài khoản thất bại";
            }
            return Json(new { status = status }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult Delete(int id)
        {
            var status = "";
            try
            {
                var user = db.users.Find(id);
                if (user != null)
                {
                    db.users.Remove(user);
                    db.SaveChanges();
                    status = "Xóa tài khoản thành công";
                }
                else
                {
                    status = "Không tìm thấy tài khoản cần xóa";
                }
            }
            catch (Exception ex)
            {
                status = "Xóa tài khoản thất bại: " + ex.Message;
            }
            return Json(new { status = status }, JsonRequestBehavior.AllowGet);
        }
    }
}
