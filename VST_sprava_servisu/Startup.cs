using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(VST_sprava_servisu.Startup))]
[assembly: log4net.Config.XmlConfigurator(Watch = true)]
namespace VST_sprava_servisu
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
