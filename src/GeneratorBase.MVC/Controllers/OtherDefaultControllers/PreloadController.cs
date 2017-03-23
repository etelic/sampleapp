using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
namespace GeneratorBase.MVC.Controllers
{
    public class PreloadController : Controller
    {
        //
        // GET: /Preload/
        [AllowAnonymous]
        public ActionResult Index()
        {
            return View();
        }
        //
        // GET: /Preload/Start
        [AllowAnonymous]
        public ActionResult Start()
        {
            var warmup = new GeneratorBase.MVC.App_Start.AppWarmUp();
            warmup.Preload(new string[]{});
            return Content("OK!");
        }
    }
}
