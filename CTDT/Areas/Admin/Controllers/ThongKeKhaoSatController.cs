﻿using CTDT.Models;
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
    public class ThongKeKhaoSatController : Controller
    {
        dbSurveyEntities db = new dbSurveyEntities();
        // GET: Admin/ThongKeKhaoSat
        public ActionResult ThongKeSVChuaKhaoSat()
        {
            ViewBag.CTDTList = new SelectList(db.ctdt.OrderBy(l => l.id_ctdt), "id_ctdt", "ten_ctdt");
            ViewBag.PKSList = new SelectList(db.survey
                .Where(l => db.answer_response.Any(aw => aw.id_sv != null && aw.surveyID == l.surveyID))
                .OrderBy(l => l.surveyID), "surveyID", "surveyTitle");
            return View();
        }
        [HttpGet]
        public ActionResult LoadSVChuaKhaoSat(int pageNumber = 1, int pageSize = 10, int ctdt = 0, int survey = 0)
        {
            var query = db.sinhvien.AsQueryable();

            if (ctdt != 0)
            {
                query = query.Where(ct => ct.lop.ctdt.id_ctdt == ctdt);
            }
            var totalRecords = query.Count();
            var GetSV = query
                .Where(sv => !db.answer_response.Any(aw => aw.id_sv == sv.id_sv && (survey == 0 || aw.surveyID == survey)))
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

            var filteredRecords = query.Count(sv => !db.answer_response.Any(aw => aw.id_sv == sv.id_sv && (survey == 0 || aw.surveyID == survey)));
            var totalPages = (int)Math.Ceiling((double)filteredRecords / pageSize);

            return Json(new { data = GetSV, totalPages = totalPages }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ExportToExcel(int ctdt = 0, int survey = 0)
        {
            var query = db.sinhvien.AsQueryable();

            if (ctdt != 0)
            {
                query = query.Where(ct => ct.lop.ctdt.id_ctdt == ctdt);
            }

            var GetSV = query
                .Where(sv => !db.answer_response.Any(aw => aw.id_sv == sv.id_sv && (survey == 0 || aw.surveyID == survey)))
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
                string fileName = $"SinhVienChuaKhaoSat_{timestamp}.xlsx";
                string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                string folderPath = Server.MapPath("~/App_Data/SinhVienChuaKhaoSat");
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