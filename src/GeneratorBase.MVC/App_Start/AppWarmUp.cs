using GeneratorBase.MVC.Models;
using InteractivePreGeneratedViews;
using System.Collections.Generic;
using System.Linq;
namespace GeneratorBase.MVC.App_Start
{
    public class AppWarmUp : System.Web.Hosting.IProcessHostPreloadClient 
    {
        public void Preload(string[] parameters)
        {
            //var counts = new List<int>();
			//try{
			//using (var ctx = new  ApplicationDbContext()) {InteractiveViews.SetViewCacheFactory(ctx,new SqlServerViewCacheFactory(ctx.Database.Connection.ConnectionString));};
			//using (var ctx = new  JournalEntryContext()) {InteractiveViews.SetViewCacheFactory(ctx,new SqlServerViewCacheFactory(ctx.Database.Connection.ConnectionString));};
			//using (var ctx = new ApplicationContext(new SystemUser())) { InteractiveViews.SetViewCacheFactory(ctx, new SqlServerViewCacheFactory(ctx.Database.Connection.ConnectionString)); };
			//}catch{}
        }
    }
}
