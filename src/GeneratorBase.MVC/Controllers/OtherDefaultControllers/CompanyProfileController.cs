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
using System.IO;
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
            if (((CustomPrincipal)User).CanEditAdminFeature("UserInterfaceSetting"))
            {
                CompanyProfile cp = _repository.GetCompanyProfile();
                if (cp == null)
                    return RedirectToAction("Index");
                return View(cp);
            }
            else return RedirectToAction("Index", "Home");
        }
        [HttpPost]
        public ActionResult Edit(CompanyProfile cp, HttpPostedFileBase Icon, HttpPostedFileBase Logo, HttpPostedFileBase LegalInformationAttach, HttpPostedFileBase PrivacyPolicyAttach, HttpPostedFileBase TermsOfServiceAttach)
        {
            if (((CustomPrincipal)User).IsAdmin)
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        string path = Server.MapPath("~/logo/");
                        string pathPolicyAndService = Server.MapPath("~/PolicyAndService/");
                        if (Icon != null)
                        {
                            Icon.SaveAs(path + "logo.gif");
                        }
                        if (Logo != null)
                        {
                            Logo.SaveAs(path + "logo_white.png");
                        }
                        if (LegalInformationAttach != null)
                        {
                            LegalInformationAttach.SaveAs(pathPolicyAndService + "Licensing.pdf");

                        }
                        if (PrivacyPolicyAttach != null)
                        {
                            LegalInformationAttach.SaveAs(pathPolicyAndService + "PrivacyPolicy.pdf");

                        }
                        if (TermsOfServiceAttach != null)
                        {
                            TermsOfServiceAttach.SaveAs(pathPolicyAndService + "Terms_Of_Service.pdf");

                        }
                        _repository.EditCompanyProfile(cp);
                        //journaling
                        DoAuditEntry.AddJournalEntryCommon(((CustomPrincipal)User),null,"Company Profile Data Modified","Company Profile");
                        return RedirectToAction("Index", "Admin");
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

