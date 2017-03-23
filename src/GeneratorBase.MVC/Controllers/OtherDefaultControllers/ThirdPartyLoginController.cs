using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GeneratorBase.MVC.Models;
using PagedList;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.SqlServer;
using System.Linq.Expressions;
namespace GeneratorBase.MVC.Controllers
{
    public class ThirdPartyLoginController : Controller
    {
        private IThirdPartyLoginRepository _repository;
        public ThirdPartyLoginController()
            : this(new ThirdPartyLoginRepository())
        {
        }
        public ThirdPartyLoginController(IThirdPartyLoginRepository repository)
        {
            _repository = repository;
        }
        public ActionResult Edit()
        {
            if (((CustomPrincipal)User).IsAdmin())
            {
                ThirdPartyLogin cp = _repository.GetThirdPartyLogin();
                if (cp == null)
                    return RedirectToAction("Index");
                return View(cp);
            }
            else return RedirectToAction("Index", "Home");
        }
        [HttpPost]
        public ActionResult Edit(ThirdPartyLogin cp)
        {
            if (((CustomPrincipal)User).IsAdmin())
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        _repository.EditThirdPartyLogin(cp);
                        return RedirectToAction("Index","Admin");
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", "Error editing record. " + ex.Message);
                    }
                }
            }
             return RedirectToAction("Index", "Home");
        }
    }
}

