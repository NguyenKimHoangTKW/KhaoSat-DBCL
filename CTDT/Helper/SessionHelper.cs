using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CTDT.Helper
{
    public static class SessionHelper
    {
        private const string UserInfoSessionKey = "UserInfoSessionKey";

        public static void SetUser(CTDT.Models.users user)
        {
            HttpContext.Current.Session[UserInfoSessionKey] = user;
        }

        public static CTDT.Models.users GetUser()
        {
            return HttpContext.Current.Session[UserInfoSessionKey] as CTDT.Models.users;
        }

        public static void ClearUser()
        {
            HttpContext.Current.Session.Remove(UserInfoSessionKey);
        }

        public static bool IsUserLoggedIn()
        {
            return HttpContext.Current.Session[UserInfoSessionKey] != null;
        }
    }

}