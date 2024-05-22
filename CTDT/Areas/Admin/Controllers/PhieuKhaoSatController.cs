using CTDT.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace CTDT.Areas.Admin.Controllers
{
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
        public ActionResult KetQuaPKS(int id)
        {
            ViewBag.id = id;
            var litsKQ = db.answer_response.Where(kq => kq.surveyID == id).ToList();
            return View(litsKQ);
        }
        [HttpGet]
        public ActionResult LoadPhieu()
        {
            var ListPhieu = db.survey.Select(p => new
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
            }).ToList();
            return Json(new { data = ListPhieu, status = "Load dữ liệu thành công" }, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public ActionResult LoadKetQuaPKS(int id)
        {
            var ListKQPKS = db.answer_response.Where(kq => kq.surveyID == id).Select(kq => new
            {
                MaKQ = kq.id,
                Email = kq.users.email,
                SinhVien = kq.sinhvien.hovaten,
                ThoiGianThucHien = kq.time,
            }).ToList();
            return Json(new {status= "Load dữ liệu thành công" , data = ListKQPKS }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult AnswerSurvey(int id)
        {
            string jsonData = string.Join("", db.answer_response.Where(d => d.surveyID == id).Select(x => x.json_answer));
            JObject surveyData = JObject.Parse(jsonData);
            JArray pagesArray = (JArray)surveyData["pages"];
            string formattedJson = pagesArray.ToString();
            return Content(formattedJson, "application/json");
        }
        public ActionResult ExportJsonAnswerToExcel(int id)
        {
            var answerResponse = db.answer_response.Find(id);
            if (answerResponse == null)
            {
                return HttpNotFound();
            }

            JObject json = JObject.Parse(answerResponse.json_answer);

            using (ExcelPackage excelPackage = new ExcelPackage())
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("Data");

                int row = 1;
                foreach (var page in json)
                {
                    worksheet.Cells[row, 1].Value = page.Value["title"].ToString();
                    row++;

                    foreach (var element in page.Value["elements"])
                    {
                        string title = element["title"].ToString();
                        string text = element["response"]["text"].ToString();

                        worksheet.Cells[row, 1].Value = title;
                        worksheet.Cells[row, 2].Value = text;
                        row++;
                    }

                    row++;
                }

                string fileName = "data_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
                string filePath = Path.Combine(Server.MapPath("~/Content"), fileName);
                FileInfo excelFile = new FileInfo(filePath);

                excelPackage.SaveAs(excelFile);

                string fileUrl = Url.Content("~/Content/Exports/" + fileName);
                return Json(new { fileUrl = fileUrl }, JsonRequestBehavior.AllowGet);
            }
        }

    }
}