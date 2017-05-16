using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WebApp.Startup))]
namespace WebApp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var hubConfiguration = new HubConfiguration();
            hubConfiguration.EnableDetailedErrors = true;

            // Make long polling connections wait a maximum of 110 seconds for a
            // response. When that time expires, trigger a timeout command and
            // make the client reconnect.
            GlobalHost.Configuration.ConnectionTimeout = System.TimeSpan.FromSeconds(40);
            // Wait a maximum of 30 seconds after a transport connection is lost
            // before raising the Disconnected event to terminate the SignalR connection.
            GlobalHost.Configuration.DisconnectTimeout = System.TimeSpan.FromSeconds(30);
            // For transports other than long polling, send a keepalive packet every
            // 10 seconds. 
            // This value must be no more than 1/3 of the DisconnectTimeout value.
            GlobalHost.Configuration.KeepAlive = System.TimeSpan.FromSeconds(10);
            //Setting up the message buffer size
            GlobalHost.Configuration.DefaultMessageBufferSize = 500;
            app.MapSignalR(hubConfiguration);
            // RouteTable.Routes.MapHubs("/myhubs", new HubConfiguration());
            ConfigureAuth(app);
        }
    }
}
