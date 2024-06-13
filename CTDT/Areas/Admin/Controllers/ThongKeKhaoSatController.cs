using CTDT.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;
using System.IO;
namespace CTDT.Areas.Admin.Controllers
{
    [AdminAuthorize]
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
        public ActionResult LoadSVChuaKhaoSat(int pageNumber = 1, int pageSize = 10, int ctdt = 0, int survey = 0, bool completed = false)
        {
            var hasAnswerResponse = db.answer_response;
            if (hasAnswerResponse.Any(aw => aw.id_sv != null && (survey == 0 || aw.surveyID == survey) && aw.id_ctdt != null))
            {
                var query = db.sinhvien.Where(x => x.lop.status == true).AsQueryable();
                if (ctdt != 0)
                {
                    query = query.Where(ct => ct.lop.ctdt.id_ctdt == ctdt);
                }
                var totalRecords = query.Count();
                if (completed)
                {
                    query = query.Where(sv => db.answer_response.Any(aw => aw.id_sv == sv.id_sv && (survey == 0 || aw.surveyID == survey)));
                }
                else
                {
                    query = query.Where(sv => !db.answer_response.Any(aw => aw.id_sv == sv.id_sv && (survey == 0 || aw.surveyID == survey)));
                }
                var filteredRecords = query.Count();
                var GetSV = query
                    .OrderBy(l => l.id_sv)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .AsEnumerable()
                    .Select(x => new
                    {
                        IDSV = x.id_sv,
                        MSSV = x.ma_sv,
                        Hoten = x.hovaten,
                        NgaySinh =x.ngaysinh.ToString("dd-MM-yyyy"),
                        SDT = x.sodienthoai,
                        CTDT = x.lop?.ctdt?.ten_ctdt ?? "",
                        Lop = x.lop?.ma_lop ?? ""
                    })
                    .ToList();
                var totalPages = (int)Math.Ceiling((double)filteredRecords / pageSize);
                return Json(new { data = GetSV, totalPages = totalPages }, JsonRequestBehavior.AllowGet);
            }
            else if (hasAnswerResponse.Any(aw => aw.id_sv == null && (survey == 0 || aw.surveyID == survey) && aw.id_ctdt != null))
            {
                var query = db.ctdt.AsQueryable();
                if (ctdt != 0)
                {
                    query = query.Where(ct => ct.id_ctdt == ctdt);
                }
                var totalRecords = query.Count();
                if (completed)
                {
                    query = query.Where(sv => db.answer_response.Any(aw => aw.id_ctdt == sv.id_ctdt && (survey == 0 || aw.surveyID == survey)));
                }
                else
                {
                    query = query.Where(sv => !db.answer_response.Any(aw => aw.id_ctdt == sv.id_ctdt && (survey == 0 || aw.surveyID == survey)));
                }
                var filteredRecords = query.Count();
                var GetSV = query
                    .OrderBy(l => l.id_ctdt)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .AsEnumerable()
                    .Select(x => new
                    {
                        IDCTDT = x.id_ctdt,
                        Tenkhoa = x.khoa?.ten_khoa ?? "",
                        TenCTDT = x.ten_ctdt
                    })
                    .ToList();
                var totalPages = (int)Math.Ceiling((double)filteredRecords / pageSize);
                return Json(new { data = GetSV, totalPages = totalPages }, JsonRequestBehavior.AllowGet);
            }
            else if (hasAnswerResponse.Any(aw => aw.id_CBVC != null && (survey == 0 || aw.surveyID == survey) && aw.id_donvi != null))
            {
                var query = db.CanBoVienChuc.AsQueryable();
                if (completed)
                {
                    query = query.Where(sv => db.answer_response.Any((aw => aw.id_CBVC == sv.id_CBVC && survey == 0 || aw.surveyID == survey)));
                }
                else
                {
                    query = query.Where(sv => !db.answer_response.Any((aw => aw.id_CBVC == sv.id_CBVC && survey == 0 || aw.surveyID == survey)));
                }
                var filteredRecords = query.Count();
                var GetSV = query
                    .OrderBy(l => l.id_CBVC)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .AsEnumerable()
                    .Select(x => new
                    {
                        IDCBVC = x.id_CBVC,
                        TenCBVC = x.TenCBVC,
                        MaCBVC = x.MaCBVC,
                        NgaySinh = x.NgaySinh.HasValue ? x.NgaySinh.Value.ToString("dd-MM-yyyy") : "",
                        Email = x.Email,
                        DonVi = x.DonVi?.name_donvi ?? "",
                        ChuongTrinh = x.ChuongTrinhDaoTao?.name_chuongtrinhdaotao ?? "",
                        ChucVu = x.ChucVu?.name_chucvu ?? "",
                        MaDonVi = x.DonVi?.id_donvi ?? 0
                    })
                    .ToList();
                var totalPages = (int)Math.Ceiling((double)filteredRecords / pageSize);
                return Json(new { data = GetSV, totalPages = totalPages }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { data = (object)null }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ExportToExcel(int ctdt = 0, int survey = 0, bool completed = false)
        {
            var query = db.sinhvien.Where(x => x.lop.status == true).AsQueryable();
            bool hasAnswerResponse = db.answer_response.Any(aw => aw.id_sv != null && (survey == 0 || aw.surveyID == survey));
            if (hasAnswerResponse)
            {
                if (ctdt != 0)
                {
                    query = query.Where(ct => ct.lop.ctdt.id_ctdt == ctdt);
                }
                if (completed)
                {
                    query = query.Where(sv => db.answer_response.Any(aw => aw.id_sv == sv.id_sv && (survey == 0 || aw.surveyID == survey)));
                }
                else
                {
                    query = query.Where(sv => !db.answer_response.Any(aw => aw.id_sv == sv.id_sv && (survey == 0 || aw.surveyID == survey)));
                }
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                string tittle = completed ? "Đối tượng đã khảo sát" : "Đối tượng chưa khảo sát";
                var GetSV = query
                    .OrderBy(l => l.id_sv)
                    .AsEnumerable()
                    .Select(x => new
                    {
                        MSSV = x.ma_sv,
                        Hoten = x.hovaten,
                        NgaySinh = x.ngaysinh.ToString("yyyy-MM-dd"),
                        SDT = x.sodienthoai,
                        CTDT = x.lop.ctdt.ten_ctdt,
                        Lop = x.lop.ma_lop,
                    })
                    .ToList();

                if (!GetSV.Any())
                {
                    return Json(new { data = (object)null, message = "Không có dữ liệu đối tượng khảo sát ở phiếu này" }, JsonRequestBehavior.AllowGet);
                }

                using (ExcelPackage package = new ExcelPackage())
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("SinhVienChuaKhaoSat");

                    if (GetSV.Any())
                    {
                        var ctdtName = GetSV.First().CTDT;
                        worksheet.Cells["A1:G1"].Merge = true;
                        worksheet.Cells["A1"].Value = tittle;
                        worksheet.Cells["A1"].Style.Font.Bold = true;
                        worksheet.Cells["A1"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                        worksheet.Cells["A2:G2"].Merge = true;
                        worksheet.Cells["A2"].Value = $"CTDT: {ctdtName}";
                        worksheet.Cells["A2"].Style.Font.Bold = true;
                        worksheet.Cells["A2"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    }

                    worksheet.Cells["A3"].Value = "STT";
                    worksheet.Cells["B3"].Value = "Mã sinh viên";
                    worksheet.Cells["C3"].Value = "Họ và tên";
                    worksheet.Cells["D3"].Value = "Ngày sinh";
                    worksheet.Cells["E3"].Value = "Số điện thoại";
                    worksheet.Cells["F3"].Value = "CTĐT";
                    worksheet.Cells["G3"].Value = "Lớp";

                    int row = 4;
                    int stt = 1;

                    foreach (var sv in GetSV)
                    {
                        worksheet.Cells[row, 1].Value = stt++;
                        worksheet.Cells[row, 2].Value = sv.MSSV;
                        worksheet.Cells[row, 3].Value = sv.Hoten;
                        worksheet.Cells[row, 4].Value = sv.NgaySinh;
                        worksheet.Cells[row, 5].Value = sv.SDT;
                        worksheet.Cells[row, 6].Value = sv.CTDT;
                        worksheet.Cells[row, 7].Value = sv.Lop;
                        row++;
                    }

                    worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                    string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
                    string status = completed ? "DoiTuongDaKhaoSat" : "DoiTuongChuaKhaoSat";
                    string fileName = $"{status}_{timestamp}.xlsx";
                    string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                    string folderPath = Server.MapPath("~/DataExport/DoiTuongKhaoSat");
                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }

                    string filePath = Path.Combine(folderPath, fileName);
                    FileInfo file = new FileInfo(filePath);
                    package.SaveAs(file);

                    byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
                    return File(fileBytes, contentType, fileName);
                }
            }
            else
            {
                return Json(new { data = (object)null, message = "Không có dữ liệu đối tượng khảo sát ở phiếu này" }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}