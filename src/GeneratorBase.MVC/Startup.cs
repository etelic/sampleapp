using Microsoft.Owin;
using Owin;
[assembly: OwinStartupAttribute(typeof(GeneratorBase.MVC.Startup))]
namespace GeneratorBase.MVC
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
