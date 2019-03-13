using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MineSweeper.WebHost.Startup))]
namespace MineSweeper.WebHost
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
			var hubConfiguration = new HubConfiguration
			{
				EnableJSONP = true,
				EnableDetailedErrors = true
			};
			app.MapSignalR(hubConfiguration);
		}
	}

}
