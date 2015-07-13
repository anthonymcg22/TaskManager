using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(BirchmierConstruction.Startup))]
namespace BirchmierConstruction
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
