using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using ZibicoLive.Entity.DTO;
using ZibicoLive.Entity.Models;

namespace ZibicoLive.Socket
{
    [HubName("LiveHub")]
    public class LiveHub : Hub
    {
        #region Configuration
        string apiUrl = ConfigurationManager.AppSettings["CoreApiUrl"];
        string authKey = ConfigurationManager.AppSettings["CoreApiAuthKey"];
        string authPass = ConfigurationManager.AppSettings["CoreApiAuthPass"];
        #endregion
        #region Variables

        #endregion

        [HubMethodName("ChatLogin")]
        public void ChatLogin(string name, string connectionUniqueId, string ipAddress, string session)
        {
            var connectionId = Context.ConnectionId;
            var websiteUniqueId = connectionUniqueId;

            var client = new RestClient(apiUrl);
            client.Authenticator = new HttpBasicAuthenticator(authKey, authPass);
            var request = new RestRequest("home/ChatLogin");
            DTOChatLogin postData = new DTOChatLogin() { SessionKey = session, WebSiteUniqueId = websiteUniqueId, IpAddress = ipAddress, Username = name, ConnectionId = connectionId };
            request.AddObject(postData);
            var response = client.Post(request);
            var responseRaw = response.Content;
            var code = response.StatusCode;

            var dto = JsonConvert.DeserializeObject<DTOLive>(responseRaw);

            Clients.Clients(new List<string>() { connectionId }).chatLogin(true, name, dto);
        }
        [HubMethodName("SetStepOption")]
        public void SetStepOption(string optionId, string websiteUniqueId, string session)
        {
            var connectionId = Context.ConnectionId;

            var client = new RestClient(apiUrl);
            client.Authenticator = new HttpBasicAuthenticator(authKey, authPass);
            var request = new RestRequest("home/SetStepOption");
            DTOSetOption postData = new DTOSetOption() { SessionKey = session, WebSiteUniqueId = websiteUniqueId, ConnectionId = connectionId ,OptionId=optionId};
            request.AddObject(postData);
            var response = client.Post(request);
            var responseRaw = response.Content;
            var code = response.StatusCode;

            var dto = JsonConvert.DeserializeObject<DTOLiveSingle>(responseRaw);

            Clients.Clients(new List<string>() { connectionId }).setStepOption(dto);
        }
        [HubMethodName("FinishChatStep")]
        public void FinishChatStep(string connectionUniqueId, string session)
        {
            var connectionId = Context.ConnectionId;
            var websiteUniqueId = connectionUniqueId;

            var client = new RestClient(apiUrl);
            client.Authenticator = new HttpBasicAuthenticator(authKey, authPass);
            var request = new RestRequest("home/FinishChatStep");
            DTOFinishChatStep postData = new DTOFinishChatStep() { SessionKey = session, WebSiteUniqueId = websiteUniqueId, ConnectionId = connectionId };
            request.AddObject(postData);
            var response = client.Post(request);
            var responseRaw = response.Content;
            var code = response.StatusCode;

            var dto = JsonConvert.DeserializeObject<bool>(responseRaw);

            Clients.Clients(new List<string>() { connectionId }).finishChatStep(dto);
        }
        [HubMethodName("OnConnectSession")]
        public void OnConnectSession(string session)
        {
            var connectionId = Context.ConnectionId;

            var client = new RestClient(apiUrl);
            client.Authenticator = new HttpBasicAuthenticator(authKey, authPass);
            var request = new RestRequest("home/OnConnectSession");
            ConnectionUserPOCO conn = new ConnectionUserPOCO() { ConnectionId = connectionId, SessionKey = session };
            request.AddObject(conn);
            var response = client.Post(request);
            var code = response.StatusCode;
        }
        public override Task OnConnected()
        {
            return base.OnConnected();
        }
        public override Task OnDisconnected(bool stopCalled)
        {
            var connectionId = Context.ConnectionId;

            var client = new RestClient(apiUrl);
            client.Authenticator = new HttpBasicAuthenticator(authKey, authPass);
            var request = new RestRequest("home/OnDisconnected");
            DTOConnection postData = new DTOConnection() { ConnectionId = connectionId };
            request.AddObject(postData);
            var response = client.Post(request);
            var responseRaw = response.Content;
            var code = response.StatusCode;

            return base.OnDisconnected(stopCalled);
        }
        public override Task OnReconnected()
        {
            return base.OnReconnected();
        }
    }
}