using System;
using System.Web.Mvc;
using ZibicoLive.Entity;

namespace ZibicoLive.Management.Auth
{
    public class AuthAttribute : ActionFilterAttribute
    {
        BaseRepository db = new BaseRepository();
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (UserSession.Info == null)
            {
                filterContext.Result = new RedirectResult("/yonetim/giris-yap");
            }
        }
    }
}