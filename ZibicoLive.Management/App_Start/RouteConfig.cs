using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace ZibicoLive.Management
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                            name: "anasayfa",
                            url: "yonetim",
                            defaults: new { controller = "home", action = "Index", id = UrlParameter.Optional }
                        );
            routes.MapRoute(
                            name: "giris",
                            url: "yonetim/giris-yap",
                            defaults: new { controller = "login", action = "Index", id = UrlParameter.Optional }
                        );
            routes.MapRoute(
                name: "cikis",
                url: "yonetim/cikis-yap",
                defaults: new { controller = "login", action = "logout", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
