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
    public class SessionHelpers
    {
        #region Configuration
        private static readonly string apiUrl = ConfigurationManager.AppSettings["CoreApiUrl"];
        private static readonly string authKey = ConfigurationManager.AppSettings["CoreApiAuthKey"];
        private static readonly string authPass = ConfigurationManager.AppSettings["CoreApiAuthPass"];
        #endregion
        #region Variables

        #endregion
        public static bool Control(string id, string domain)
        {
            try
            {
                var client = new RestClient(apiUrl);
                client.Authenticator = new HttpBasicAuthenticator(authKey, authPass);
                var request = new RestRequest("home/DomainControl");
                DTODomainControl postData = new DTODomainControl() {Domain=domain,WebSiteUniqueId=id };
                request.AddObject(postData);
                var response = client.Post(request);
                var responseRaw = response.Content;
                var code = response.StatusCode;

                var dto = JsonConvert.DeserializeObject<bool>(responseRaw);

                return dto;
            }
            catch (Exception ex)
            {
                return false;
                throw;
            }
        }

        public static ConnectionUserPOCO LastConnectionControl(string sessionId)
        {
            try
            {
                var client = new RestClient(apiUrl);
                client.Authenticator = new HttpBasicAuthenticator(authKey, authPass);
                var request = new RestRequest("home/LastConnectionControl");
                DTOSession dtoPost = new DTOSession() { SessionKey = sessionId };
                request.AddObject(dtoPost);
                var response = client.Post(request);
                var responseRaw = response.Content;
                var code = response.StatusCode;

                var dto = JsonConvert.DeserializeObject<ConnectionUserPOCO>(responseRaw);

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