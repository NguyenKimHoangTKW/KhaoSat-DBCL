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
    [AdminAuthorize]
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
        private int MapMaLopToIDLop(string malop)
        {
            var lop = db.lop.FirstOrDefault(dt => dt.ma_lop == malop);
            return lop?.id_lop ?? 0;
        }
        [HttpPost]
        public ActionResult UploadExcel(HttpPostedFileBase excelFile)
        {
            if (excelFile != null && excelFile.ContentLength > 0)
            {
                try
                {

                    DateTime now = DateTime.UtcNow;
                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                    using (var package = new ExcelPackage(excelFile.InputStream))
                    {
                        var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                        if (worksheet == null)
                        {
                            return Json(new { status = "Không tìm thấy worksheet trong file Excel" }, JsonRequestBehavior.AllowGet);
                        }
                        int unixTimestamp = (int)(now.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                        for (int row = 2; row <= worksheet.Dimension.End.Row; row++)
                        {
                            string tenLop = worksheet.Cells[row, 10].Value.ToString();
                            int malop = MapMaLopToIDLop(tenLop);

                            var sinhVien = new sinhvien
                            {

                                ma_sv = worksheet.Cells[row, 1].Text,
                                hovaten = worksheet.Cells[row, 2].Text,
                                ngaysinh = DateTime.Parse(worksheet.Cells[row, 3].Text),
                                sodienthoai = worksheet.Cells[row, 4].Text,
                                diachi = worksheet.Cells[row, 5].Text,
                                phai = worksheet.Cells[row, 6].Text,
                                namtotnghiep = worksheet.Cells[row, 7].Text,
                                id_lop = malop,
                                ngaytao = unixTimestamp,
                                ngaycapnhat = unixTimestamp
                            };

                            db.sinhvien.Add(sinhVien);
                        }

                        db.SaveChanges();

                        return Json(new { status = "Thêm sinh viên thành công" }, JsonRequestBehavior.AllowGet);
                    }
                }
                catch (Exception ex)
                {
                    return Json(new { status = $"Đã xảy ra lỗi: {ex.Message}" }, JsonRequestBehavior.AllowGet);
                }
            }

            return Json(new { status = "Vui lòng chọn file Excel" }, JsonRequestBehavior.AllowGet);
        }
    }
}