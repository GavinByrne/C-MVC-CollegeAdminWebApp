using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(GradesWebApplication.Startup))]
namespace GradesWebApplication
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
