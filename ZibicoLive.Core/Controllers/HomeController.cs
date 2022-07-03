using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using ZibicoLive.Entity;
using ZibicoLive.Entity.DTO;
using ZibicoLive.Entity.Models;

namespace ZibicoLive.Core.Controllers
{
    [ZbcAuthorize]
    public class HomeController : ApiController
    {
        #region Socket Server Endpoints
        [HttpPost]
        public IHttpActionResult OnConnectSession(ConnectionUserPOCO conn)
        {
            using (BaseRepository db = new BaseRepository())
            {
                db.Insert<ConnectionUserPOCO>(new ConnectionUserPOCO() { ConnectionId = conn.ConnectionId, SessionKey = conn.SessionKey });
            }
            return Ok();
        }
        [HttpPost]
        public IHttpActionResult OnDisconnected(DTOConnection conPost)
        {
            using (BaseRepository db = new BaseRepository())
            {
                var connUser = db.GetList<ConnectionUserPOCO>("ConnectionId=@cid", new { cid = conPost.ConnectionId }).FirstOrDefault();
                db.Delete<ConnectionUserPOCO>(connUser);
            }
            return Ok();
        }
        [HttpPost]
        public IHttpActionResult ChatLogin(DTOChatLogin chatDto)
        {
            var sessionList = new List<string>();
            Guid websiteGuid = Guid.Parse(chatDto.WebSiteUniqueId);
            DTOLive dto = new DTOLive();
            using (BaseRepository db = new BaseRepository())
            {
                dto.WebSite = db.GetList<WebSitePOCO>("UniqueID=@uid AND Status=1", new { uid = chatDto.WebSiteUniqueId }).FirstOrDefault();
                dto.Steps = db.GetList<StepPOCO>("WebSiteId=@wid AND Status=1 ORDER BY StepNumber asc", new { wid = dto.WebSite.ID }).ToList();
                //(Select COUNT(ID) from StepOptionValues where StepID=Steps.ID)>0
                dto.Options = new List<StepOptionPOCO>();
                dto.Values = new List<StepOptionValuePOCO>();

                foreach (StepPOCO item in dto.Steps)
                {
                    var stepOptions = db.GetList<StepOptionPOCO>("StepID=@sid AND Status=1", new { sid = item.ID }).ToList();
                    //(Select COUNT(ID) from StepOptionValues where StepID= StepOptions.StepID )>0
                    dto.Options.AddRange(stepOptions);
                }
                foreach (StepOptionPOCO item in dto.Options)
                {
                    var stepOptionValues = db.GetList<StepOptionValuePOCO>("Status=1 AND OptionID=@oid AND SessionKey=@session", new { oid = item.ID, session = chatDto.SessionKey }).ToList();
                    dto.Values.AddRange(stepOptionValues);
                }

                db.Insert<ConnectionUserPOCO>(new ConnectionUserPOCO() { ConnectionId = chatDto.ConnectionId, IPAddress = chatDto.IpAddress, SessionKey = chatDto.SessionKey, UniqueId = websiteGuid, Username = chatDto.Username });
                var list = db.GetList<ConnectionUserPOCO>("SessionKey=@session", new { session = chatDto.SessionKey }).ToList();
                list.ForEach(x => sessionList.Add(x.ConnectionId));
            }
            return Json(dto);
        }
        [HttpPost]
        public IHttpActionResult SetStepOption(DTOSetOption stepDto)
        {
            DTOLiveSingle dto = new DTOLiveSingle();
            using (BaseRepository db = new BaseRepository())
            {
                var option = db.GetList<StepOptionPOCO>("UniqueID=@uid AND Status=1", new { uid = stepDto.OptionId }).FirstOrDefault();
                var step = db.GetList<StepPOCO>("ID=@id AND Status=1", new { id = option.StepID }).FirstOrDefault();
                var webSite = db.GetList<WebSitePOCO>("UniqueID=@uid AND Status=1", new { uid = stepDto.WebSiteUniqueId }).FirstOrDefault();

                var lastStepOptionValue = db.GetList<StepOptionValuePOCO>("SessionKey=@sky AND Status=1", new { sky = stepDto.SessionKey }).FirstOrDefault();
                Guid groupKey = Guid.NewGuid();
                if (lastStepOptionValue != null) groupKey = (Guid)lastStepOptionValue.GroupKey;

                db.Insert<StepOptionValuePOCO>(new StepOptionValuePOCO() { OptionID = option.ID, SessionKey = stepDto.SessionKey, StepID = step.ID, WebSiteId = webSite.ID, UniqueId = Guid.NewGuid(), CreatedDate = DateTime.Now, Status = 1,GroupKey=groupKey });

                dto.Step = db.GetList<StepPOCO>("Status=1 AND ID=@id", new { stepNo = step.StepNumber, id = option.NextStepID }).FirstOrDefault();
                if (dto.Step == null)
                {
                    dto = null;
                    return Json(dto);
                }
                if (dto.Step.IsFinished == true)
                {
                    return Json(dto);
                }
                dto.WebSite = webSite;
                dto.Options = new List<StepOptionPOCO>();

                var stepOptions = db.GetList<StepOptionPOCO>("StepID=@sid AND Status=1", new { sid = dto.Step.ID }).ToList();
                dto.Options.AddRange(stepOptions);
            }
            return Json(dto);
        }
        [HttpPost]
        public IHttpActionResult FinishChatStep(DTOFinishChatStep finishDto)
        {
            bool result = false;
            using (BaseRepository db = new BaseRepository())
            {
                var allValues = db.GetList<StepOptionValuePOCO>("SessionKey=@session AND Status=1", new { session = finishDto.SessionKey }).ToList();
                List<long> valueList = new List<long>();
                allValues.ForEach(x => valueList.Add(x.ID));

                if (valueList.Count > 0)
                {
                    var query = db.Execute("UPDATE StepOptionValues SET Status=0 WHERE ID IN(@list)", new { list = valueList });
                    if (query > 0) result = true;
                }
                return Json(result);
            }
        }
        #endregion
        #region Script Server Endpoints
        [HttpPost]
        public IHttpActionResult GetLiveSteps(DTOGetLive livePost)
        {
            DTOLive dto = new DTOLive();
            using (BaseRepository db = new BaseRepository())
            {
                dto.ConnectionUser = db.GetList<ConnectionUserPOCO>("SessionKey=@session AND Username is NOT NULL", new { session = livePost.SessionKey }).FirstOrDefault();

                dto.WebSite = db.GetList<WebSitePOCO>("UniqueID=@uid AND Status=1", new { uid = livePost.WebSiteUniqueId }).FirstOrDefault();

                var stepOptionValues = db.GetList<StepOptionValuePOCO>("Status=1 AND SessionKey=@session ORDER BY ID asc", new { session = livePost.SessionKey }).ToList();
                dto.Values = stepOptionValues;
                List<long> steps = new List<long>();
                foreach (StepOptionValuePOCO item in stepOptionValues)
                {
                    steps.Add(item.StepID);
                }

                if (steps.Count > 0)
                {
                    dto.Steps = db.GetList<StepPOCO>("WebSiteId=@wid AND Status=1 AND ID IN(@id)", new { wid = dto.WebSite.ID, id = steps }).ToList();
                }
                else
                {
                    var step = db.GetList<StepPOCO>("WebSiteId=@wid AND Status=1 AND StepNumber>-1", new { wid = dto.WebSite.ID }).FirstOrDefault();
                    dto.Steps = new List<StepPOCO>() { step };
                }

                if (dto.Values.Count > 0)
                {
                    var lastOptionValue = stepOptionValues.LastOrDefault();
                    var lastOptionId = lastOptionValue.OptionID;
                    var lastOption = db.GetList<StepOptionPOCO>("ID=@id AND Status=1", new { id = lastOptionId }).FirstOrDefault();
                    var lastStep = db.GetList<StepPOCO>("ID=@id AND Status=1", new { id = lastOption.NextStepID }).FirstOrDefault();
                    dto.Steps.Add(lastStep);
                }

                //(Select COUNT(ID) from StepOptionValues where StepID=Steps.ID)>0
                dto.Options = new List<StepOptionPOCO>();

                foreach (StepPOCO item in dto.Steps)
                {
                    var stepOptions = db.GetList<StepOptionPOCO>("StepID=@sid AND Status=1", new { sid = item.ID }).ToList();
                    //(Select COUNT(ID) from StepOptionValues where StepID= StepOptions.StepID )>0
                    dto.Options.AddRange(stepOptions);
                }
                return Json(dto);
            }
        }

        [HttpPost]
        public IHttpActionResult DomainControl(DTODomainControl domainPost)
        {
            bool result = false;
            using (BaseRepository db = new BaseRepository())
            {
                Guid uid = Guid.Parse(domainPost.WebSiteUniqueId);
                WebSitePOCO website = db.GetList<WebSitePOCO>("UniqueID=@uid AND WebSiteDomain=@domain AND Status=1", new { uid = uid, domain = domainPost.Domain }).FirstOrDefault();
                if (website != null) result = true;
            }
            return Json(result);
        }
        [HttpPost]
        public IHttpActionResult LastConnectionControl(DTOSession sessionPost)
        {
            ConnectionUserPOCO conn = null;
            using (BaseRepository db = new BaseRepository())
            {
                conn = db.GetList<ConnectionUserPOCO>("SessionKey=@session AND Username is NOT NULL", new { session = sessionPost.SessionKey }).FirstOrDefault();
            }
            return Json(conn);
        }
        #endregion
    }
}
