using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using ZibicoLive.Entity.DTO;
using ZibicoLive.Script.Helpers;

namespace ZibicoLive.Script.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        [HttpGet]
        public string Js(string id)
        {
            if (!string.IsNullOrWhiteSpace(id))
            {
                #region Domain Control
                string referer = Request.Headers["Referer"]?.ToString();
                if (string.IsNullOrWhiteSpace(referer)) return "";
                Uri baseUri = new Uri(referer);
                var hostname = baseUri.Host;
                bool control = SessionHelpers.Control(id, hostname);
                if (!control) return "";
                #endregion

                string raw = "";
                using (StreamReader sr = new StreamReader(HostingEnvironment.MapPath("~/tags/js/tags.js")))
                {
                    raw = sr.ReadToEnd();
                    raw = raw.Replace("{{url}}", $"{CommonHelpers.GetAppSettingKey("ScriptServerUrl")}/home/live/{id}");
                }
                return raw;
            }
            return "alert('warning');";
        }
        public ActionResult Live(string id)
        {
            ViewBag.ScriptServer = CommonHelpers.GetAppSettingKey("ScriptServerUrl");
            if (!string.IsNullOrWhiteSpace(id))
            {
                #region Domain Control
                string referer = Request.Headers["Referer"]?.ToString();
                if (string.IsNullOrWhiteSpace(referer)) return HttpNotFound("403");
                Uri baseUri = new Uri(referer);
                var hostname = baseUri.Host;
                bool control = SessionHelpers.Control(id, hostname);
                if (!control) return HttpNotFound("403");
                #endregion

                ViewBag.ConnectionId = id;
                var sessionId = HttpContext.Session.SessionID;
                var sessionResult = SessionHelpers.LastConnectionControl(sessionId);
                if (sessionResult != null)
                {
                    ViewBag.LastConnection = sessionResult.Username;
                    DTOLive dto = LiveHelpers.GetLive(sessionId, id);
                    ViewBag.DTOLive = dto;
                }

                return View();
            }
            return HttpNotFound("404");
        }
    }
}