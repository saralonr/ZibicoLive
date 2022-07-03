using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using ZibicoLive.Entity;
using ZibicoLive.Entity.DTO;
using ZibicoLive.Entity.Models;

namespace ZibicoLive.Script.Helpers
{
    public class LiveHelpers
    {
        #region Configuration
        private static readonly string apiUrl = ConfigurationManager.AppSettings["CoreApiUrl"];
        private static readonly string authKey = ConfigurationManager.AppSettings["CoreApiAuthKey"];
        private static readonly string authPass = ConfigurationManager.AppSettings["CoreApiAuthPass"];
        #endregion
        #region Variables

        #endregion
        public static DTOLive GetLive(string session, string websiteUniqueId)
        {
            try
            {
                var client = new RestClient(apiUrl);
                client.Authenticator = new HttpBasicAuthenticator(authKey, authPass);
                var request = new RestRequest("home/GetLiveSteps");
                DTOGetLive postData = new DTOGetLive() { SessionKey = session, WebSiteUniqueId = websiteUniqueId };
                request.AddObject(postData);
                var response = client.Post(request);
                var responseRaw = response.Content;
                var code = response.StatusCode;

                var dto = JsonConvert.DeserializeObject<DTOLive>(responseRaw);

                return dto;

            }
            catch (Exception ex)
            {
                return null;
                throw;
            }
        }
    }
}