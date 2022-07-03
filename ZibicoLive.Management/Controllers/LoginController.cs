using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ZibicoLive.Entity;
using ZibicoLive.Entity.Models;
using ZibicoLive.Management.Auth;

namespace ZibicoLive.Management.Controllers
{
    public class LoginController : Controller
    {
        BaseRepository db = new BaseRepository();
        #region PAGES
        public ActionResult Index()
        {
            return View();
        }
        #endregion
        #region GET
        public ActionResult Logout()
        {
            if (UserSession.Info != null)
            {
                Session.Abandon();
                HttpCookie user = new HttpCookie("User");
                user.Expires = DateTime.Now.AddDays(-30);
                Response.Cookies.Add(user);
                UserSession.Info = null;
            }
            return Redirect("/yonetim/giris-yap");
        }
        #endregion
        #region POST
        [HttpPost]
        public ActionResult Login(UserPOCO loginUser)
        {
            if (UserSession.Info != null) return Redirect("/");

            if (loginUser == null | loginUser.Username == null | loginUser.Password == null)
            {
                TempData["Error"] = "Kullanıcı adı veya şifreniz yanlış.";
                return Redirect("/yonetim/giris-yap");
            }
            else if (loginUser.Username.Length > 50 || loginUser.Password.Length > 50)
            {
                TempData["Error"] = "Kullanıcı adı veya şifreniz yanlış.";
                return Redirect("/yonetim/giris-yap");
            }


            UserPOCO userPoco = db.GetList<UserPOCO>("Username=@name AND Password=@pass AND Status=@st", new { name = loginUser.Username, pass = loginUser.Password, st = 1 }).FirstOrDefault();

            if (userPoco != null)
            {
                bool remember = Convert.ToBoolean(Request.Form["Remember"]);
                if (remember)
                {
                    HttpCookie user = new HttpCookie("User");
                    user.Expires = DateTime.Now.AddDays(30);
                    user.Values.Add("Username", userPoco.Username);
                    user.Values.Add("SKey", userPoco.UniqueID.ToString());
                    Response.Cookies.Add(user);
                }

                UserSession.Info = userPoco;

                return Redirect("/");
            }
            else
            {
                TempData["Error"] = "Kullanıcı adı veya şifreniz yanlış.";
                return Redirect("/yonetim/giris-yap");
            }
        }
        #endregion

    }
}