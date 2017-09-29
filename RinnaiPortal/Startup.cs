using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(RinnaiPortal.Startup))]
namespace RinnaiPortal
{
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            ConfigureAuth(app);
        }
    }
}
