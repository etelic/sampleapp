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
    public class CompanyProfileController : Controller
    {
        private ICompanyProfileRepository _repository;
        public CompanyProfileController()
            : this(new CompanyProfileRepository())
        {
        }
        public CompanyProfileController(ICompanyProfileRepository repository)
        {
            _repository = repository;
        }
        public ActionResult Edit()
        {
            if (((CustomPrincipal)User).IsAdmin())
            {
                CompanyProfile cp = _repository.GetCompanyProfile();
                if (cp == null)
                    return RedirectToAction("Index");
                return View(cp);
            }
            else return RedirectToAction("Index", "Home");
        }
        [HttpPost]
        public ActionResult Edit(CompanyProfile cp, HttpPostedFileBase logo)
        {
            if (((CustomPrincipal)User).IsAdmin())
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        string path = Server.MapPath("~/logo/");
                        if (logo != null)
                        {
                            logo.SaveAs(path + "logo.gif");// System.IO.Path.GetFileName(logo.FileName));
                           // cp.logo = System.IO.Path.GetFileName(logo.FileName);
                        }
                        _repository.EditCompanyProfile(cp);
                        return RedirectToAction("Index","Admin");
                    }
                    catch (Exception ex)
                    {
                        //error msg for failed edit in XML file
                        ModelState.AddModelError("", "Error editing record. " + ex.Message);
                    }
                }
            }
             return RedirectToAction("Index", "Home");
        }
    }
}

