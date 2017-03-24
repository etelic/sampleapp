using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace GeneratorBase.MVC
{
    public class HandleAntiforgeryTokenErrorAttribute : HandleErrorAttribute
    {
        public override void OnException(ExceptionContext filterContext)
        {
            if (filterContext.Exception is HttpAntiForgeryException)
            {
                filterContext.ExceptionHandled = true;
                HttpContext contxt = HttpContext.Current;
                var values = new RouteValueDictionary(new
                {
                    action = "Login",
                    controller = "Account",
                    returnUrl = contxt.Request.Url.PathAndQuery.ToString()
                });

                filterContext.Result = new RedirectToRouteResult(values);
            }
        }
    }
}