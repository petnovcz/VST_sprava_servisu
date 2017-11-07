using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(VST_sprava_servisu.Startup))]
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
