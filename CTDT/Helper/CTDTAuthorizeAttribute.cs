using CTDT.Helper;
using System;
using System.Web;
using System.Web.Mvc;

public class CTDTAuthorizeAttribute : AuthorizeAttribute
{
    protected override bool AuthorizeCore(HttpContextBase httpContext)
    {
        var user = SessionHelper.GetUser();
        if (user == null || user.id_typeusers != 3)
        {
            return false;
        }
        return true;
    }

    protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
    {
        filterContext.Result = new RedirectResult("~/Home/Index");
    }
}
