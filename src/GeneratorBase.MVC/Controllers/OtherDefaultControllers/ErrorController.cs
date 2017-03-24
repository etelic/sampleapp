using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
namespace GeneratorBase.MVC.Controllers
{
    public class ErrorController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult ActivationLink()
        {
            return View();
        }
        public ActionResult ConcurrencyError(string UrlReferrer, string Message)
        {
            ViewData["ConcurrencyReferrer"] = UrlReferrer;
            ViewData["ConcurrencyMessage"] = Message;
            return View();
        }
        public ActionResult ConcurrencyButton(string UrlReferrer)
        {
            return Redirect(UrlReferrer);
           // return View();
        }
        public ActionResult NotFound404()
        {
            return View("NotFound404");
        }
    }
}