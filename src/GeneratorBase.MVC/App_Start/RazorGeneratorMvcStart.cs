using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;
using RazorGenerator.Mvc;
[assembly: WebActivatorEx.PostApplicationStartMethod(typeof(GeneratorBase.MVC.App_Start.RazorGeneratorMvcStart), "Start")]
namespace GeneratorBase.MVC.App_Start {
    public static class RazorGeneratorMvcStart {
        public static void Start() {
            var engine = new PrecompiledMvcEngine(typeof(RazorGeneratorMvcStart).Assembly) {
                UsePhysicalViewsIfNewer = true
            };
            ViewEngines.Engines.Insert(0, engine);
            // StartPage lookups are done by WebPages. 
            VirtualPathFactoryManager.RegisterVirtualPathFactory(engine);
        }
    }
}
