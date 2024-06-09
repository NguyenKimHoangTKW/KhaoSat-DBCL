using CTDT.Helper;
using CTDT.Models;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CTDT.Areas.CTDT.Controllers
{
    public class ThongKeKhaoSatController : Controller
    {
        dbSurveyEntities db = new dbSurveyEntities();
        public ActionResult Index()
        {
            return View();
        }
        // GET: CTDT/ThongKeKhaoSat
        public ActionResult TKSVCKS()
        {
            ViewBag.PKSList = new SelectList(db.survey.OrderBy(l => l.surveyID), "surveyID", "surveyTitle");
            return View();
        }
        [HttpGet]
        public ActionResult LoadSVChuaKhaoSat(int pageNumber = 1, int pageSize = 10, int survey = 0, bool completed = false)
        {
            var user = SessionHelper.GetUser();
            var query = db.sinhvien.Where(x=> x.lop.ctdt.id_ctdt == user.id_ctdt).AsQueryable();

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
                    MSSV = x.ma_sv,
                    Hoten = x.hovaten,
                    NgaySinh = x.ngaysinh.ToString("yyyy-MM-dd"),
                    SDT = x.sodienthoai,
                    CTDT = x.lop.ctdt.ten_ctdt,
                    Lop = x.lop.ma_lop,
                })
                .ToList();

            var totalPages = (int)Math.Ceiling((double)filteredRecords / pageSize);

            return Json(new { data = GetSV, totalPages = totalPages }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ExportToExcel(int survey = 0, bool completed = false)
        {
            var user = SessionHelper.GetUser();
            var query = db.sinhvien.Where(x => x.lop.ctdt.id_ctdt == user.id_ctdt).AsQueryable();

            if (completed)
            {
                query = query.Where(sv => db.answer_response.Any(aw => aw.id_sv == sv.id_sv && (survey == 0 || aw.surveyID == survey)));
            }
            else
            {
                query = query.Where(sv => !db.answer_response.Any(aw => aw.id_sv == sv.id_sv && (survey == 0 || aw.surveyID == survey)));
            }

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

            using (ExcelPackage package = new ExcelPackage())
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("SinhVienChuaKhaoSat");

                if (GetSV.Any())
                {
                    var ctdtName = GetSV.First().CTDT;
                    worksheet.Cells["A1:G1"].Merge = true;
                    worksheet.Cells["A1"].Value = $"CTDT: {ctdtName}";
                    worksheet.Cells["A1"].Style.Font.Bold = true;
                    worksheet.Cells["A1"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                }

                worksheet.Cells["A2"].Value = "STT";
                worksheet.Cells["B2"].Value = "Mã sinh viên";
                worksheet.Cells["C2"].Value = "Họ và tên";
                worksheet.Cells["D2"].Value = "Ngày sinh";
                worksheet.Cells["E2"].Value = "Số điện thoại";
                worksheet.Cells["F2"].Value = "CTĐT";
                worksheet.Cells["G2"].Value = "Lớp";

                int row = 3;
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

                string folderPath = Server.MapPath("~/DataExport/DoiTuongKhaoSat-CTDT");
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
    }
}