using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(RinnaiPortalMvc.Startup))]
namespace RinnaiPortalMvc
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
