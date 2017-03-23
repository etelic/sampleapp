using GeneratorBase.MVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
namespace GeneratorBase.MVC
{
    public abstract class CustomWebViewPage<TModel> : WebViewPage<TModel>
    {
        public new CustomPrincipal User
        {
            get
            {
                return base.User as CustomPrincipal;
            }
        }
    }
}