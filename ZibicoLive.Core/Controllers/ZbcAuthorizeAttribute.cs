using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using ZibicoLive.Core.Helpers;

namespace ZibicoLive.Core.Controllers
{
    internal class ZbcAuthorizeAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            try
            {
                var authorize = actionContext.Request.Headers.Authorization?.Parameter;
                if (authorize == null) { actionContext.Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Incorrect Api Authorize Parameters"); return; }
                byte[] data = Convert.FromBase64String(authorize);
                string decodedString = Encoding.UTF8.GetString(data);

                bool result = AuthHelpers.Control(decodedString);
                if (result) return;
                else actionContext.Response = actionContext.Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Incorrect Api Authorize Parameters");
            }
            catch (Exception ex)
            {
                actionContext.Response = actionContext.Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Connection Error");
            }

        }
    }
}