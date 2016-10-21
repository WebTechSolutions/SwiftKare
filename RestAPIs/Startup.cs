using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(RestAPIs.Startup))]
namespace RestAPIs
{
        public partial class Startup
        {
            public void Configuration(IAppBuilder app)
            {
                ConfigureAuth(app);
            }
        }
}