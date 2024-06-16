using CTDT.Helper;
using CTDT.Models;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace CTDT.Controllers
{
    public class HomeController : Controller
    {
        dbSurveyEntities db = new dbSurveyEntities();
        public ActionResult Index()
        {
            var email = TempData["Email"] as string;
            ViewBag.Email = email;
            return View();
        }
        public ActionResult PhieuKhaoSat(int id)
        {
            Session["IDPhieu"] = id;
            ViewBag.id = Session["IDPhieu"];
            var phieukhaosat = db.survey
                                 .Where(x => x.surveyID == id)
                                 .Select(x => new
                                 {
                                     MaPhieu = x.surveyID,
                                     TenPhieu = x.hedaotao.ten_hedaotao,
                                 })
                                 .ToList();
            return View(phieukhaosat);
        }

        [HttpGet]
        public ActionResult LoadHeDaoTao()
        {
            var hedaotao = db.hedaotao.Select(c => new
            {
                MaHDT = c.id_hedaotao,
                TenHDT = c.ten_hedaotao,
            }).ToList();

            return Content(JsonConvert.SerializeObject(new { data = hedaotao, TotalItems = hedaotao.Count, status = "Load Dữ liệu thành công" }), "application/json");
        }

        [HttpGet]
        public ActionResult LoadPhieuKhaoSat(int id)
        {
            var phieukhaosat = db.survey.Where(c=>c.surveyStatus == 1).Select(c => new
            {
                MaPhieu = c.surveyID,
                TenPKS = c.surveyTitle,
                MoTaPhieu = c.surveyDescription,
                MaHDT = c.id_hedaotao,
                TenHDT = c.hedaotao.ten_hedaotao,
                TenLoaiKhaoSat = c.LoaiKhaoSat.name_loaikhaosat,
            }).Where(p => p.MaHDT == id).ToList();
            return Content(JsonConvert.SerializeObject(new { data = phieukhaosat, TotalItems = phieukhaosat.Count, status = "Load Dữ liệu thành công" }), "application/json");
        }
        //Xác thực
        public ActionResult XacThucCTDT(int id)
        {
            Session.Remove("XTDV");
            Session.Remove("CBVC");
            ViewBag.id = id;
            var xacthuc = db.survey.Where(x => x.surveyID == id).ToList();
            ViewBag.KhoaList = new SelectList(db.khoa.OrderBy(l => l.id_khoa), "id_khoa", "ten_khoa");
            return View(xacthuc);
        }
        public ActionResult XacThuc(int id)
        {
            Session.Remove("XTDV");
            Session.Remove("CBVC");
            ViewBag.id = id;
            var xacthuc = db.survey.Where(x => x.surveyID == id).ToList();
            ViewBag.KhoaList = new SelectList(db.khoa.OrderBy(l => l.id_khoa), "id_khoa", "ten_khoa");
            return View();
        }
        public ActionResult XacThucbySV(int id)
        {
            Session.Remove("XTDV");
            Session.Remove("CBVC");
            ViewBag.id = id;
            var xacthuc = db.survey.Where(x => x.surveyID == id).ToList();
            ViewBag.KhoaList = new SelectList(db.khoa.OrderBy(l => l.id_khoa), "id_khoa", "ten_khoa");
            return View();
        }
        public ActionResult XacThucbyCBVC(int id)
        {
            Session.Remove("SelectedSvByXT");
            Session.Remove("SelectedKhoaByXTCTDT");
            Session.Remove("SelectedCTDTByXTCTDT");
            ViewBag.id = id;
            var xacthuc = db.survey.Where(x => x.surveyID == id).ToList();
            ViewBag.DonViList = new SelectList(db.DonVi.OrderBy(l => l.id_donvi), "id_donvi", "name_donvi");
            return View();
        }
        //
        // Load CTDT từ Khoa
        public ActionResult LoadCTDTByKhoa(int khoaId)
        {
            db.Configuration.ProxyCreationEnabled = false;
            List<ctdt> ctdt = db.ctdt.Where(x=>x.id_khoa == khoaId).ToList();
            return Json(ctdt, JsonRequestBehavior.AllowGet);
        }
        // Load Lớp từ CTDT
        public ActionResult LoadLopByCTDT(int CTDTID)
        {
            db.Configuration.ProxyCreationEnabled = false;
            List<lop> lop = db.lop.Where(x => x.id_ctdt == CTDTID && x.status == true).ToList();
            return Json(lop, JsonRequestBehavior.AllowGet);
        }
        // Load Sinh Viên từ Lớp
        public ActionResult LoadSVByLop(int LopID)
        {
            db.Configuration.ProxyCreationEnabled = false;
            List<sinhvien> sinhvien = db.sinhvien.Where(x => x.id_lop == LopID).ToList();
            return Json(sinhvien, JsonRequestBehavior.AllowGet);
        }
        // SaveDataXacThuc
        [HttpPost]
        public ActionResult SaveDataXacThucByCBVC(string donvi)
        {
            var user = SessionHelper.GetUser();
            try
            {
                Session["XTDV"] = donvi;

                if (int.TryParse(donvi, out int intDV))
                {
                    var surveyResponse = db.answer_response.FirstOrDefault(x => x.id_users == user.id_users && x.id_donvi == intDV);
                    if (surveyResponse != null)
                    {
                        return Json(new { success = false, message = "Tài khoản này đã thực hiện khảo sát cho Đơn vị này!" });
                    }

                    var canBoVienChuc = db.CanBoVienChuc.FirstOrDefault(x => x.Email == user.email);
                    if (canBoVienChuc == null)
                    {
                        return Json(new { success = false, message = "Tài khoản bạn không thể thực hiện khảo sát vì Email bạn đang sử dụng không nằm trong dữ liệu CBVC, vui lòng đổi Email để tiếp tục" });
                    }
                    else
                    {
                        Session["CBVC"] = canBoVienChuc.id_CBVC;
                    }
                    
                }
                else
                {
                    return Json(new { success = false, message = "Dữ liệu đầu vào không hợp lệ" });
                }

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult SaveDataXacThucCTDT(string ctdt, string sv, int surveyid)
        {
            try
            {
                var user = SessionHelper.GetUser();
                Session["XTCTDT"] = ctdt;
                Session["XTSV"] = sv;

                if (int.TryParse(ctdt, out int intCtdt) && int.TryParse(sv, out int intSv))
                {
                    var surveyResponse = db.answer_response.FirstOrDefault(x => x.id_ctdt == intCtdt && x.id_sv == intSv && x.surveyID == surveyid && x.id_users == user.id_users);
                    if (surveyResponse != null)
                    {
                        return Json(new { success = false, message = "Sinh viên này đã thực hiện khảo sát" });
                    }
                }
                else
                {
                    return Json(new { success = false, message = "Dữ liệu đầu vào không hợp lệ" });
                }

                int idPhieu = Convert.ToInt32(Session["IDPhieu"]);
                return Json(new { success = true, idPhieu = idPhieu });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        [HttpPost]
        public ActionResult SaveDataXacThucbySV(string ctdt, string sv, int surveyid)
        {
            var user = SessionHelper.GetUser();
            try
            {
                Session["XTCTDT"] = ctdt;
                Session["XTSV"] = sv;

                if (int.TryParse(ctdt, out int intCtdt) && int.TryParse(sv, out int intSv))
                {
                    var surveyResponse = db.answer_response.SingleOrDefault(x => x.id_ctdt == intCtdt && x.id_sv == intSv && x.surveyID == surveyid && x.id_users == user.id_users);
                    if (surveyResponse != null)
                    {
                        return Json(new { success = false, message = "Sinh viên này đã thực hiện khảo sát" });
                    }
                    else if (!user.email.EndsWith("@student.tdmu.edu.vn"))
                    {
                        return Json(new { success = false, message = "Tài khoản bạn không thể thực hiện khảo sát, vui lòng login bằng tài khoản Email của Sinh viên." });
                    }
                }
                else
                {
                    return Json(new { success = false, message = "Dữ liệu đầu vào không hợp lệ" });
                }

                int idPhieu = Convert.ToInt32(Session["IDPhieu"]);
                return Json(new { success = true, idPhieu = idPhieu });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        [HttpPost]
        public ActionResult SaveDataXacThucCTDTWithoutSV(string ctdt, int surveyid)
        {
            var user = SessionHelper.GetUser();
            try
            {                
                Session["XTCTDT"] = ctdt;

                if (int.TryParse(ctdt, out int intCtdt))
                {
                    var surveyResponse = db.answer_response.FirstOrDefault(x => x.id_ctdt == intCtdt && x.id_users == user.id_users && x.surveyID == surveyid);
                    if (surveyResponse != null)
                    {
                        return Json(new { success = false, message = "Tài khoản này đã khảo sát chương trình đào tạo này rồi" });
                    }

                }
                else
                {
                    return Json(new { success = false, message = "Dữ liệu đầu vào không hợp lệ" });
                }

                int idPhieu = Convert.ToInt32(Session["IDPhieu"]);
                return Json(new { success = true, idPhieu = idPhieu });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult GetSvIdByMssv(string mssv, int surveyid)
        {
            var user = SessionHelper.GetUser();
            var student = db.sinhvien.SingleOrDefault(s => s.ma_sv == mssv && s.lop.status == true);
            if (student != null)
            {
                var surveyResponse = db.answer_response.FirstOrDefault(r => r.id_sv == student.id_sv && r.surveyID == surveyid && r.id_users == user.id_users);
                if (surveyResponse != null)
                {
                    return Json(new { success = false, message = "Sinh viên này đã thực hiện khảo sát" });
                }
                else
                {
                    return Json(new { success  = true, svId = student.id_sv, ctdt = student.lop.ctdt.id_ctdt });
                }
            }
            else
            {
                return Json(new { success = false, message = "MSSV không tồn tại." });
            }
        }
        //
        [HttpPost]
        public ActionResult ClearSession()
        {
            Session.Remove("SelectedSvByXT");
            Session.Remove("SelectedKhoaByXTCTDT");
            Session.Remove("SelectedCTDTByXTCTDT");
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }
    }
}