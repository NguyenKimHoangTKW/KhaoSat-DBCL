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
            bool hasAnswerResponseForStudent = db.answer_response.Any(aw => aw.id_sv != null && (survey == 0 || aw.surveyID == survey));
            bool hasAnswerResponseForProgram = db.answer_response.Any(aw => aw.id_sv == null && (survey == 0 || aw.surveyID == survey) && aw.id_ctdt != null);
            bool hasAnswerResponseForStaff = db.answer_response.Any(aw => aw.id_CBVC != null && (survey == 0 || aw.surveyID == survey) && aw.id_donvi != null);
            if (hasAnswerResponseForStudent)
            {
                var query = db.sinhvien.Where(x => x.lop.status == true).AsQueryable();
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
                var filteredRecords = query.Count();
                var GetSV = query
                    .OrderBy(l => l.id_sv)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .AsEnumerable()
                    .Select(x => new
                    {
                        IDSV = x.id_sv,
                        MSSV = x.ma_sv ?? "Không có dữ liệu",
                        Hoten = x.hovaten ?? "Không có dữ liệu",
                        NgaySinh =x.ngaysinh.ToString("dd-MM-yyyy"),
                        SDT = x.sodienthoai ?? "Không có dữ liệu",
                        CTDT = x.lop?.ctdt?.ten_ctdt ?? "Không có dữ liệu",
                        Lop = x.lop?.ma_lop ?? "Không có dữ liệu"
                    })
                    .ToList();
                var totalPages = (int)Math.Ceiling((double)filteredRecords / pageSize);
                return Json(new { data = GetSV, totalPages = totalPages }, JsonRequestBehavior.AllowGet);
            }
            else if (hasAnswerResponseForProgram)
            {
                var query = db.ctdt.AsQueryable();
                if (ctdt != 0)
                {
                    query = query.Where(ct => ct.id_ctdt == ctdt);
                }
                if (completed)
                {
                    query = query.Where(ct => db.answer_response.Any(aw => aw.id_ctdt == ct.id_ctdt && (survey == 0 || aw.surveyID == survey)));
                }
                else
                {
                    query = query.Where(ct => !db.answer_response.Any(aw => aw.id_ctdt == ct.id_ctdt && (survey == 0 || aw.surveyID == survey)));
                }

                var filteredRecords = query.Count();
                var GetPrograms = query
                    .OrderBy(l => l.id_ctdt)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .AsEnumerable()
                    .Select(x => new
                    {
                        IDCTDT = x.id_ctdt,
                        Tenkhoa = x.khoa?.ten_khoa ?? "Không có dữ liệu",
                        TenCTDT = x.ten_ctdt ?? "Không có dữ liệu"
                    })
                    .ToList();
                var totalPages = (int)Math.Ceiling((double)filteredRecords / pageSize);
                return Json(new { data = GetPrograms, totalPages = totalPages }, JsonRequestBehavior.AllowGet);
            }
            else if (hasAnswerResponseForStaff)
            {
                var query = db.CanBoVienChuc.AsQueryable();
                if (completed)
                {
                    query = query.Where(cbvc => db.answer_response.Any(aw => aw.id_CBVC == cbvc.id_CBVC && (survey == 0 || aw.surveyID == survey)));
                }
                else
                {
                    query = query.Where(cbvc => !db.answer_response.Any(aw => aw.id_CBVC == cbvc.id_CBVC && (survey == 0 || aw.surveyID == survey)));
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
                        TenCBVC = x.TenCBVC ?? "Không có dữ liệu",
                        MaCBVC = x.MaCBVC ?? "Không có dữ liệu",
                        NgaySinh = x.NgaySinh.HasValue ? x.NgaySinh.Value.ToString("dd-MM-yyyy") : "Không có dữ liệu",
                        Email = x.Email ?? "Không có dữ liệu",
                        DonVi = x.DonVi?.name_donvi ?? "Không có dữ liệu",
                        ChuongTrinh = x.ChuongTrinhDaoTao?.name_chuongtrinhdaotao ?? "Không có dữ liệu",
                        ChucVu = x.ChucVu?.name_chucvu ?? "Không có dữ liệu",
                        MaDonVi = x.DonVi?.id_donvi ?? 0,
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
            bool hasAnswerResponseForStudent = db.answer_response.Any(aw => aw.id_sv != null && (survey == 0 || aw.surveyID == survey));
            bool hasAnswerResponseForProgram = db.answer_response.Any(aw => aw.id_sv == null && (survey == 0 || aw.surveyID == survey) && aw.id_ctdt != null);
            bool hasAnswerResponseForStaff = db.answer_response.Any(aw => aw.id_CBVC != null && (survey == 0 || aw.surveyID == survey) && aw.id_donvi != null);

            if (hasAnswerResponseForStudent)
            {
                var query = db.sinhvien.Where(x => x.lop.status == true).AsQueryable();
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

                var GetSV = query
                    .OrderBy(l => l.id_sv)
                    .AsEnumerable()
                    .Select(x => new
                    {
                        MSSV = x.ma_sv,
                        Hoten = x.hovaten,
                        NgaySinh = x.ngaysinh.ToString("yyyy-MM-dd"),
                        SDT = x.sodienthoai,
                        CTDT = x.lop?.ctdt?.ten_ctdt ?? "",
                        Lop = x.lop?.ma_lop ?? ""
                    })
                    .ToList();

                return ExportDataToExcel(GetSV, completed ? "Đối tượng đã khảo sát" : "Đối tượng chưa khảo sát", "SinhVien");
            }
            else if (hasAnswerResponseForProgram)
            {
                var query = db.ctdt.AsQueryable();
                if (ctdt != 0)
                {
                    query = query.Where(ct => ct.id_ctdt == ctdt);
                }
                if (completed)
                {
                    query = query.Where(ct => db.answer_response.Any(aw => aw.id_ctdt == ct.id_ctdt && (survey == 0 || aw.surveyID == survey)));
                }
                else
                {
                    query = query.Where(ct => !db.answer_response.Any(aw => aw.id_ctdt == ct.id_ctdt && (survey == 0 || aw.surveyID == survey)));
                }

                var GetPrograms = query
                    .OrderBy(l => l.id_ctdt)
                    .AsEnumerable()
                    .Select(x => new
                    {
                        IDCTDT = x.id_ctdt,
                        Tenkhoa = x.khoa?.ten_khoa ?? "",
                        TenCTDT = x.ten_ctdt
                    })
                    .ToList();

                return ExportDataToExcel(GetPrograms, completed ? "Chương trình đã khảo sát" : "Chương trình chưa khảo sát", "ChuongTrinh");
            }
            else if (hasAnswerResponseForStaff)
            {
                var query = db.CanBoVienChuc.AsQueryable();
                if (completed)
                {
                    query = query.Where(cbvc => db.answer_response.Any(aw => aw.id_CBVC == cbvc.id_CBVC && (survey == 0 || aw.surveyID == survey)));
                }
                else
                {
                    query = query.Where(cbvc => !db.answer_response.Any(aw => aw.id_CBVC == cbvc.id_CBVC && (survey == 0 || aw.surveyID == survey)));
                }

                var GetStaff = query
                    .OrderBy(l => l.id_CBVC)
                    .AsEnumerable()
                    .Select(x => new
                    {
                        IDCBVC = x.id_CBVC,
                        TenCBVC = x.TenCBVC,
                        MaCBVC = x.MaCBVC,
                        NgaySinh = x.NgaySinh.HasValue ? x.NgaySinh.Value.ToString("yyyy-MM-dd") : "",
                        Email = x.Email,
                        DonVi = x.DonVi?.name_donvi ?? "",
                        ChuongTrinh = x.ChuongTrinhDaoTao?.name_chuongtrinhdaotao ?? "",
                        ChucVu = x.ChucVu?.name_chucvu ?? "",
                        MaDonVi = x.DonVi?.id_donvi ?? 0
                    })
                    .ToList();

                return ExportDataToExcel(GetStaff, completed ? "Cán bộ viên chức đã khảo sát" : "Cán bộ viên chức chưa khảo sát", "CanBoVienChuc");
            }
            else
            {
                return Json(new { data = (object)null, message = "Không có dữ liệu đối tượng khảo sát ở phiếu này" }, JsonRequestBehavior.AllowGet);
            }
        }
        private ActionResult ExportDataToExcel<T>(List<T> data, string title, string sheetName)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (ExcelPackage package = new ExcelPackage())
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(sheetName);

                if (data.Any())
                {
                    var properties = typeof(T).GetProperties();

                    worksheet.Cells["A1:" + GetExcelColumnName(properties.Length) + "1"].Merge = true;
                    worksheet.Cells["A1"].Value = title;
                    worksheet.Cells["A1"].Style.Font.Bold = true;
                    worksheet.Cells["A1"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    for (int i = 0; i < properties.Length; i++)
                    {
                        worksheet.Cells[2, i + 1].Value = properties[i].Name;
                    }

                    int row = 3;
                    foreach (var item in data)
                    {
                        for (int col = 0; col < properties.Length; col++)
                        {
                            worksheet.Cells[row, col + 1].Value = properties[col].GetValue(item)?.ToString() ?? "";
                        }
                        row++;
                    }

                    worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                    string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
                    string fileName = $"{sheetName}_{timestamp}.xlsx";
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
                else
                {
                    return Json(new { data = (object)null, message = "Không có dữ liệu đối tượng khảo sát ở phiếu này" }, JsonRequestBehavior.AllowGet);
                }
            }
        }

        private string GetExcelColumnName(int index)
        {
            const string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string result = "";
            while (index > 0)
            {
                index--;
                result = letters[index % 26] + result;
                index /= 26;
            }
            return result;
        }
    }
}