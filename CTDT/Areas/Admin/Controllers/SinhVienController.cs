using CTDT.Models;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Google.Cloud.Translation.V2;
namespace CTDT.Areas.Admin.Controllers
{
    public class SinhVienController : Controller
    {
        dbSurveyEntities db = new dbSurveyEntities();
        // GET: Admin/SinhVien
        public ActionResult ViewSinhVien()
        {
            return View();
        }
        [HttpGet]
        public ActionResult LoadSinhVien(int pageNumber = 1, int pageSize = 10, string keyword = "")
        {
            try
            {
                IQueryable<sinhvien> query = db.sinhvien;

                if (!string.IsNullOrEmpty(keyword))
                {
                    keyword = keyword.ToLower();
                    query = query.Where(l => l.ma_sv.ToLower().Contains(keyword)
                                          || l.hovaten.ToLower().Contains(keyword));
                }

                var totalRecords = query.Count();
                var totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

                var listsv = query
                    .OrderBy(sv => sv.id_sv)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .AsEnumerable()
                    .Select(sv => new
                    {
                        IDSV = sv.id_sv,
                        MSSV = sv.ma_sv,
                        HoTen = sv.hovaten,
                        NgaySinh = sv.ngaysinh.ToString("dd-MM-yyy"),
                        SDT = sv.sodienthoai,
                        DiaChi = sv.diachi,
                        GioiTinh = sv.phai,
                        NamTotNghiep = sv.namtotnghiep,
                        NgayTao = sv.ngaytao,
                        NgayCapNhat = sv.ngaycapnhat,
                    }).ToList();

                return Json(new { data = listsv, totalPages = totalPages, status = "Load dữ liệu thành công" }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { status = "Không tìm thấy sinh viên" }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}