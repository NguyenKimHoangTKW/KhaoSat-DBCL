using System.Web;
using System.Web.Optimization;

namespace CTDT
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new Bundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));
            // Client Side

            bundles.Add(new ScriptBundle("~/bundles/firebase").Include(
                        "~/Scripts/Firebase.js"));
            bundles.Add(new ScriptBundle("~/bundles/loginversion").Include(
                        "~/Scripts/LoginVersion.js"));
            bundles.Add(new ScriptBundle("~/bundles/User/HeDaoTao").Include(
                        "~/Scripts/User/Hedaotao.js"));
            bundles.Add(new ScriptBundle("~/bundles/User/Phieukhaosat").Include(
                        "~/Scripts/User/Phieukhaosat.js"));
            //
            // Xác thực
            bundles.Add(new ScriptBundle("~/bundles/User/Xacthuc").Include(
                        "~/Scripts/User/Xacthuc.js"));
            bundles.Add(new ScriptBundle("~/bundles/User/XacthucCTDT").Include(
                        "~/Scripts/User/XacthucCTDT.js"));
            bundles.Add(new ScriptBundle("~/bundles/User/XacthucBySV").Include(
                        "~/Scripts/User/XacthucBySinhVien.js"));
            //
            // PKS
            bundles.Add(new ScriptBundle("~/bundles/User/AnswerPKS").Include(
                        "~/Scripts/User/Survey/AnswerPKS.js"));
            bundles.Add(new ScriptBundle("~/bundles/User/SurveyForm").Include(
                        "~/Scripts/User/Survey/SurveyForm.js"));
            bundles.Add(new ScriptBundle("~/bundles/User/ListAnswerSurvey").Include(
                        "~/Scripts/User/Survey/ListAnswerSurvey.js"));
            bundles.Add(new ScriptBundle("~/bundles/User/Survey").Include(
                        "~/Scripts/User/Survey/Survey.js"));
            //
            // Admin
            BundleTable.EnableOptimizations = true;
        }
    }
}
