using CTDT.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace CTDT.Areas.Admin.Controllers
{
    [AdminAuthorize]
    public class PhieuKhaoSatController : Controller
    {
        dbSurveyEntities db = new dbSurveyEntities();
        // GET: Admin/PhieuKhaoSat
        public ActionResult Index()
        {
            ViewBag.HDT = new SelectList(db.hedaotao, "id_hedaotao", "ten_hedaotao");
            ViewBag.LKS = new SelectList(db.LoaiKhaoSat, "id_loaikhaosat", "name_loaikhaosat");
            return View();
        }
        [HttpPost]
        public ActionResult NewSurvey(survey s)
        {
            var status = "";
            DateTime now = DateTime.UtcNow;
            if (ModelState.IsValid)
            {
                int unixTimestamp = (int)(now.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                s.surveyTimeMake = unixTimestamp;
                s.surveyTimeUpdate = unixTimestamp;
                db.survey.Add(s);
                db.SaveChanges();
                status = "Tạo mới phiếu khảo sát thành công";
            }
            return Json(new { status = status}, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult AddSurvey(int id)
        {
            ViewBag.ID = id;
            var items = db.survey.Where(x => x.surveyID == id).ToList();
           return View(items);
        }
        public ActionResult KetQuaPKS(int id)
        {
            ViewBag.id = id;
            var litsKQ = db.answer_response.Where(kq => kq.surveyID == id).ToList();
            ViewBag.CTDTList = new SelectList(db.ctdt.OrderBy(l => l.id_ctdt), "id_ctdt", "ten_ctdt");
            ViewBag.DonViList = new SelectList(db.DonVi.OrderBy(l => l.id_donvi), "id_donvi", "name_donvi");
            return View(litsKQ);
        }
        public ActionResult AnswerPKS(int id)
        {
            ViewBag.id = id;
            var litsKQ = db.answer_response.Where(kq => kq.id == id).ToList();
            foreach (var item in litsKQ)
            {
                ViewBag.IDPhieu = item.surveyID;
            }
            return View(litsKQ);
        }
        [HttpGet]
        public ActionResult LoadPhieu(int pageNumber = 1, int pageSize = 10, int hdt = 0, int loaiks = 0)
        {
            try
            {
                var query = db.survey.AsQueryable();

                if (hdt != 0)
                {
                    query = query.Where(p => p.id_hedaotao == hdt);
                }

                if (loaiks != 0)
                {
                    query = query.Where(p => p.id_loaikhaosat == loaiks);
                }

                var totalRecords = query.Count();
                var totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

                var ListPhieu = query
                    .OrderBy(l => l.surveyID)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(p => new
                    {
                        MaPhieu = p.surveyID,
                        TenHDT = p.hedaotao.ten_hedaotao,
                        MaHDT = p.id_hedaotao,
                        TieuDePhieu = p.surveyTitle,
                        MoTaPhieu = p.surveyDescription,
                        NgayTao = p.surveyTimeStart,
                        NgayChinhSua = p.surveyTimeMake,
                        LoaiKhaoSat = p.LoaiKhaoSat.name_loaikhaosat,
                        MaLKS = p.id_loaikhaosat,
                        TrangThai = p.surveyStatus,
                    }).ToList();

                return Json(new { data = ListPhieu, totalPages = totalPages, status = "Load dữ liệu thành công" }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { status = "Load dữ liệu thất bại" }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpGet]
        public ActionResult LoadKetQuaPKS(int id, int pageNumber = 1, int pageSize = 10)
        {
            bool hasAnswerResponseForStudent = db.answer_response.Any(aw => aw.id_sv != null && (aw.surveyID == id));
            bool hasAnswerResponseForProgram = db.answer_response.Any(aw => aw.id_sv == null && (aw.surveyID == id) && aw.id_ctdt != null);
            bool hasAnswerResponseForStaff = db.answer_response.Any(aw => aw.id_CBVC != null && (aw.surveyID == id) && aw.id_donvi != null);
            if (hasAnswerResponseForStudent)
            {
                var totalRecords = db.answer_response.Count(aw => aw.surveyID == id && aw.id_sv != null);
                var GetStuden = db.answer_response
                    .Where(kq => kq.surveyID == id && kq.id_sv != null)
                    .OrderBy(kq => kq.id)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(kq => new
                    {
                        MaKQ = kq.id,
                        Email = kq.users.email,
                        SinhVien = kq.sinhvien.hovaten,
                        ThoiGianThucHien = kq.time,
                        CTDT = kq.ctdt.ten_ctdt,
                        MaAnswer = kq.id,
                        MSSV = kq.sinhvien.ma_sv,
                    }).ToList();

                var totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
                return Json(new { status = "Load dữ liệu thành công", totalPages = totalPages, data = GetStuden }, JsonRequestBehavior.AllowGet);
            }
            else if (hasAnswerResponseForProgram)
            {
                var totalRecords = db.answer_response.Count(aw => aw.surveyID == id && aw.id_sv == null && aw.id_ctdt != null);
                var GetProgram = db.answer_response
                    .Where(kq => kq.surveyID == id && kq.id_sv == null && kq.id_ctdt != null)
                    .OrderBy(kq => kq.id)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(kq => new
                    {
                        MaKQ = kq.id,
                        Email = kq.users.email,
                        IDCTDT = kq.id_ctdt,
                        Tenkhoa = kq.ctdt.khoa.ten_khoa ?? "",
                        TenCTDT = kq.ctdt.ten_ctdt,
                        ThoiGianThucHien = kq.time,
                    }).ToList();

                var totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
                return Json(new { status = "Load dữ liệu thành công", totalPages = totalPages, data = GetProgram }, JsonRequestBehavior.AllowGet);
            }
            else if (hasAnswerResponseForStaff)
            {
                var totalRecords = db.answer_response.Count(aw => aw.surveyID == id && aw.id_CBVC != null && aw.id_donvi != null);
                var GetStaff = db.answer_response
                    .Where(kq => kq.surveyID == id && kq.id_CBVC != null && kq.id_donvi != null)
                    .OrderBy(kq => kq.id)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(kq => new
                    {
                        MaKQ = kq.id,
                        DonVi = kq.DonVi.name_donvi,
                        Email = kq.users.email,
                        CBVC = kq.CanBoVienChuc.TenCBVC,
                        ThoiGianThucHien = kq.time,
                    }).ToList();

                var totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
                return Json(new { status = "Load dữ liệu thành công", totalPages = totalPages, data = GetStaff }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { data = (object)null }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult AnswerSurvey(int id)
        {
            var answers = db.answer_response.Where(d => d.id == id).Select(x => x.json_answer).ToList();
            List<JObject> surveyData = new List<JObject>();

            foreach (var answer in answers)
            {
                JObject answerObject = JObject.Parse(answer);
                surveyData.Add(answerObject);
            }

            return Content(JsonConvert.SerializeObject(surveyData), "application/json");
        }
        public ActionResult ExportExcelSurvey(int id, int cttdt = 0, int donvi = 0)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            bool hasAnswerResponseForStudent = db.answer_response.Any(aw => aw.id_sv != null && aw.surveyID == id);
            bool hasAnswerResponseForProgram = db.answer_response.Any(aw => aw.id_sv == null && aw.surveyID == id && aw.id_ctdt != null);
            bool hasAnswerResponseForStaff = db.answer_response.Any(aw => aw.id_CBVC != null && aw.surveyID == id && aw.id_donvi != null);
            if (hasAnswerResponseForStudent)
            {
                var query = db.answer_response.AsQueryable();
                if (cttdt != 0)
                {
                    query = query.Where(p => p.id_ctdt == cttdt);
                }
                var answers = query.Where(d => d.surveyID == id && d.id_ctdt == cttdt)
                                 .Select(x => new
                                 {
                                     DauThoiGian = x.time,
                                     x.json_answer,
                                     Email = x.users.email,
                                     MSSV = x.sinhvien.ma_sv,
                                     HoTen = x.sinhvien.hovaten,
                                     NgaySinh = (DateTime?)x.sinhvien.ngaysinh,
                                     Lop = x.sinhvien.lop.ma_lop,
                                     CTDT = x.ctdt.ten_ctdt,
                                     SDT = x.sinhvien.sodienthoai,
                                 }).ToList();
                List<JObject> surveyData = new List<JObject>();

                foreach (var answer in answers)
                {
                    JObject answerObject = JObject.Parse(answer.json_answer);
                    answerObject["DauThoiGian"] = answer.DauThoiGian;
                    answerObject["Email"] = answer.Email;
                    answerObject["MSSV"] = answer.MSSV;
                    answerObject["HoTen"] = answer.HoTen;
                    answerObject["NgaySinh"] = answer.NgaySinh?.ToString("yyyy-MM-dd");
                    answerObject["Lop"] = answer.Lop;
                    answerObject["CTDT"] = answer.CTDT;
                    answerObject["SDT"] = answer.SDT;
                    surveyData.Add(answerObject);
                }
                return Content(JsonConvert.SerializeObject(surveyData), "application/json");
            }
            else if (hasAnswerResponseForProgram)
            {
                var query = db.answer_response.AsQueryable();
                if (cttdt != 0)
                {
                    query = query.Where(p => p.id_ctdt == cttdt);
                }
                var answers = query.Where(d => d.surveyID == id && d.id_ctdt == cttdt)
                                 .Select(x => new
                                 {
                                     DauThoiGian = x.time,
                                     x.json_answer,
                                     Email = x.users.email,
                                     CTDT = x.ctdt.ten_ctdt,
                                 }).ToList();
                List<JObject> surveyData = new List<JObject>();

                foreach (var answer in answers)
                {
                    JObject answerObject = JObject.Parse(answer.json_answer);
                    answerObject["DauThoiGian"] = answer.DauThoiGian;
                    answerObject["Email"] = answer.Email;
                    answerObject["CTDT"] = answer.CTDT;
                    surveyData.Add(answerObject);
                }
                return Content(JsonConvert.SerializeObject(surveyData), "application/json");
            }
            else if (hasAnswerResponseForStaff)
            {
                var query = db.answer_response.AsQueryable();
                if (donvi != 0)
                {
                    query = query.Where(p => p.id_donvi == donvi);
                }
                var answers = query.Where(d => d.surveyID == id && d.id_donvi == donvi)
                                 .Select(x => new
                                 {
                                     DauThoiGian = x.time,
                                     x.json_answer,
                                     HoTen = x.CanBoVienChuc.TenCBVC,
                                     Email = x.users.email,
                                     DonVi = x.DonVi.name_donvi,
                                 }).ToList();
                List<JObject> surveyData = new List<JObject>();

                foreach (var answer in answers)
                {
                    JObject answerObject = JObject.Parse(answer.json_answer);
                    answerObject["DauThoiGian"] = answer.DauThoiGian;
                    answerObject["Email"] = answer.Email;
                    answerObject["HoTen"] = answer.HoTen;
                    answerObject["DonVi"] = answer.DonVi;
                    surveyData.Add(answerObject);
                }
                return Content(JsonConvert.SerializeObject(surveyData), "application/json");
            }
            else
            {
                return new EmptyResult();
            }
        }

        [HttpPost]
        public ActionResult SaveExcelFile()
        {
            try
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                HttpPostedFileBase file = Request.Files["file"];

                if (file != null && file.ContentLength > 0)
                {
                    string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
                    string fileName = $"KetQuaKhaoSat_{timestamp}.xlsx";
                    string directoryPath = Server.MapPath("~/DataExport/KetQuaPKS");
                    string filePath = Path.Combine(directoryPath, fileName);
                    Directory.CreateDirectory(directoryPath);
                    file.SaveAs(filePath);
                    return Json(new { success = true, message = "File saved successfully." });
                }
                else
                {
                    return Json(new { success = false, message = "No file found." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error saving file: " + ex.Message });
            }
        }
    }
}