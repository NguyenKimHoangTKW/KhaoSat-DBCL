using CTDT.Models;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CTDT.Areas.Admin.Controllers
{
    public class CBVCController : Controller
    {
        dbSurveyEntities db = new dbSurveyEntities();
        // GET: Admin/CBVC
        public ActionResult ViewCBVC()
        {
            return View();
        }
        [HttpGet]
        public ActionResult LoadData(int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var query = db.CanBoVienChuc.AsQueryable();
                var GetCBVC = query
                    .OrderBy(l => l.id_CBVC)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .AsEnumerable()
                    .Select(x => new
                    {
                        TenCBVC = x.TenCBVC ?? "Không có dữ liệu",
                        MaCBVC = x.MaCBVC ?? "Không có dữ liệu",
                        NgaySinh = x.NgaySinh.HasValue ? x.NgaySinh.Value.ToString("dd-MM-yyyy") : "",
                        Email = x.Email ?? "Không có dữ liệu",
                        DonVi = x.DonVi?.name_donvi ?? "Không có dữ liệu",
                        ChuongTrinh = x.ChuongTrinhDaoTao?.name_chuongtrinhdaotao ?? "Không có dữ liệu",
                        ChucVu = x.ChucVu?.name_chucvu ?? "Không có dữ liệu",
                        NamHoatDong = x.namhoatdong,
                        TrangThai = x.status,
                    })
                    .ToList();
                var totalRecords = db.CanBoVienChuc.Count();
                var totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
                return Json(new { data = GetCBVC, totalPages = totalPages, totalItems = totalRecords }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { status = "Load dữ liệu thất bại" }, JsonRequestBehavior.AllowGet);
            }
        }

        public int MapTenDonViToIDDonVi(string tendonvi)
        {
            var donvi = db.DonVi.Where(k => k.name_donvi == tendonvi).FirstOrDefault();
            return donvi?.id_donvi ?? 0;
        }
        public int MapTenChucVuToIDChucVu(string tenchucvu)
        {
            var chucvu = db.ChucVu.Where(cv => cv.name_chucvu == tenchucvu).FirstOrDefault();
            return chucvu?.id_chucvu ?? 0;
        }
        public int MapChuongTrinhToIDChuongTrinh(string tenchuongtrinh)
        {
            var chuongtrinh = db.ChuongTrinhDaoTao.Where(cv => cv.name_chuongtrinhdaotao == tenchuongtrinh).FirstOrDefault();
            return chuongtrinh?.id_chuongtrinhdaotao ?? 0;
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

                        for (int row = 2; row <= worksheet.Dimension.End.Row; row++)
                        {
                            var ngaySinhText = worksheet.Cells[row, 4].Text;
                            var donViText = worksheet.Cells[row, 6].Text;
                            var chucVuText = worksheet.Cells[row, 7].Text;
                            var chuongTrinhText = worksheet.Cells[row, 8].Text;

                            var madonvi = !string.IsNullOrEmpty(donViText) ? MapTenDonViToIDDonVi(donViText) : (int?)null;
                            var machucvu = !string.IsNullOrEmpty(chucVuText) ? MapTenChucVuToIDChucVu(chucVuText) : (int?)null;
                            var mactdt = !string.IsNullOrEmpty(chuongTrinhText) ? MapChuongTrinhToIDChuongTrinh(chuongTrinhText) : (int?)null;

                            DateTime? ngaySinh = null;
                            if (DateTime.TryParseExact(ngaySinhText, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedNgaySinh))
                            {
                                ngaySinh = parsedNgaySinh;
                            }

                            var cbvc = new CanBoVienChuc
                            {
                                MaCBVC = string.IsNullOrEmpty(worksheet.Cells[row, 2].Text) ? null : worksheet.Cells[row, 2].Text,
                                TenCBVC = worksheet.Cells[row, 3].Text,
                                NgaySinh = ngaySinh,
                                Email = string.IsNullOrEmpty(worksheet.Cells[row, 5].Text) ? null : worksheet.Cells[row, 5].Text,
                                status = false,
                                namhoatdong = now.Year,
                                id_chucvu = machucvu,
                                id_donvi = madonvi,
                                id_chuongtrinhdaotao = mactdt,
                            };

                            db.CanBoVienChuc.Add(cbvc);
                        }

                        db.SaveChanges();

                        return Json(new { status = "Thêm cán bộ viên chức thành công" }, JsonRequestBehavior.AllowGet);
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