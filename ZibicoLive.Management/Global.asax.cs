using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using ZibicoLive.Entity;
using ZibicoLive.Entity.Models;
using ZibicoLive.Management.Auth;

namespace ZibicoLive.Management
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }
        void Session_Start()
        {
            HttpCookie user = Request.Cookies["User"];
            if (user != null)
            {
                string username = user.Values["Username"];
                Guid skey = Guid.Parse(user.Values["SKey"]);

                using (BaseRepository db = new BaseRepository())
                {
                    UserPOCO per = db.GetList<UserPOCO>("Username=@name AND UniqueID=@sk AND Status=@st", new { sk = skey, name = username, st = 1 }).FirstOrDefault(); if (per != null)
                    {
                        UserSession.Info = per;
                    }
                    else
                    {
                        user.Expires = DateTime.Now.AddDays(-30);
                        Response.Cookies.Add(user);
                        UserSession.Info = null;
                        Session.Abandon();
                    }
                }




            }
        }
    }
}
