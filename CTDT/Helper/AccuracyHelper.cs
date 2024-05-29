using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CTDT.Helper
{
    public static class AccuracyHelper
    {

        public static void SetSelectedCTDT(CTDT.Models.ctdt ctdt)
        {
            HttpContext.Current.Session["SelectedCTDTByXTCTDT"] = ctdt;
        }

        public static CTDT.Models.ctdt GetSelectedCTDT()
        {
            return HttpContext.Current.Session["SelectedCTDTByXTCTDT"] as CTDT.Models.ctdt;
        }
        public static void SetSelectedSv(CTDT.Models.sinhvien sv)
        {
            HttpContext.Current.Session["SelectedSvByXT"] = sv;
        }

        public static CTDT.Models.sinhvien GetSelectedSv()
        {
            return HttpContext.Current.Session["SelectedSvByXT"] as CTDT.Models.sinhvien;
        }
    }
}