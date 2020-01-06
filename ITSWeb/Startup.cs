using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ITSWeb.Startup))]
namespace ITSWeb
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
