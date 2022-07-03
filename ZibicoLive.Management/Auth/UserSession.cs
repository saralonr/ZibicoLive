using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ZibicoLive.Entity.Models;

namespace ZibicoLive.Management.Auth
{
    public static class UserSession
    {
        public static UserPOCO Info { get { return HttpContext.Current.Session["UserSession"] as UserPOCO; } set { HttpContext.Current.Session["UserSession"] = value; } }
    }
}