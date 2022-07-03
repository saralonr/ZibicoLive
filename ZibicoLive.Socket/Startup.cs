using Owin;
using Microsoft.Owin;
using Microsoft.AspNet.SignalR;

[assembly: OwinStartup(typeof(ZibicoLive.Socket.Startup))]
namespace ZibicoLive.Socket
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // Any connection or hub wire up and configuration should go here
            app.MapSignalR("/signalr",new HubConfiguration() { EnableJSONP=true});
        }
    }
}