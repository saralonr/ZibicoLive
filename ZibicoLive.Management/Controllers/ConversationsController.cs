using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ZibicoLive.Entity;
using ZibicoLive.Entity.DTO;
using ZibicoLive.Entity.Models;
using ZibicoLive.Management.Auth;

namespace ZibicoLive.Management.Controllers
{
    [Auth]
    public class ConversationsController : Controller
    {
        BaseRepository db = new BaseRepository();
        private JsonResponse rs = new JsonResponse();
        #region PAGE
        public ActionResult List(long? page = 1)
        {
            using (db)
            {
               var list = db.PageList<DTOStepFlow>((long)page, 30, "Select sov.GroupKey,sov.SessionKey,con.Username,sov.Status from StepOptionValues as sov LEFT JOIN ConnectionUsers as con ON con.SessionKey=sov.SessionKey WHERE sov.WebSiteId=@wid AND con.Username IS NOT NULL GROUP BY sov.GroupKey,sov.SessionKey,con.Username,sov.Status", new { wid = UserSession.Info.WebSiteID });
                return View(list);
            }

        }
        public ActionResult Create(Guid? ID)
        {
            if (ID == null) return View();
            else
            {
                using (db)
                {
                   var stepFlow = db.Query<DTOStepFlowSummary>("Select sov.GroupKey,sov.CreatedDate,con.Username,st.ID as StepID,st.Question,so.ID as OptionID,so.OptionDescription as [Option],so.NextStepID,(Select Question from Steps WHERE ID=so.NextStepID) as NextStep from StepOptionValues as sov LEFT JOIN StepOptions as so ON so.ID=sov.OptionID LEFT JOIN Steps as st ON st.ID=sov.StepID LEFT JOIN WebSites as web ON web.ID=sov.WebSiteId LEFT JOIN ConnectionUsers as con ON con.SessionKey=sov.SessionKey WHERE sov.WebSiteId=@wid AND sov.GroupKey = @gid AND con.Username IS NOT NULL GROUP BY sov.GroupKey,sov.CreatedDate,st.ID,st.Question,so.ID,so.OptionDescription,so.NextStepID,con.Username", new { wid= UserSession.Info.WebSiteID  , gid = ID});
                    if (stepFlow == null)
                    {
                        ViewBag.ErrorMessage = "Kayıt bulunamadı.";
                        return View();
                    }
                    ViewBag.FlowSummary = db.Query<DTOStepFlow>("Select sov.GroupKey,sov.SessionKey,con.Username,sov.Status from StepOptionValues as sov LEFT JOIN ConnectionUsers as con ON con.SessionKey=sov.SessionKey WHERE sov.WebSiteId=@wid AND con.Username IS NOT NULL AND sov.GroupKey=@gid GROUP BY sov.GroupKey,sov.SessionKey,con.Username,sov.Status", new { wid = UserSession.Info.WebSiteID, gid = ID }).FirstOrDefault();
                    return View(stepFlow);
                }
            }
        }
        #endregion
    }
}