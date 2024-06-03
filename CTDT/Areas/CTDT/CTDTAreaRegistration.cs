using System.Web.Mvc;

namespace CTDT.Areas.CTDT
{
    public class CTDTAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "CTDT";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "CTDT_default",
                "CTDT/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}