using GeneratorBase.MVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
namespace GeneratorBase.MVC.Controllers
{
    public class ODataServicesController : Controller
    {
        private IODataServicesRepository _repository;
        public ODataServicesController()
            : this(new ODataServicesRepository())
        {
        }
        public ODataServicesController(IODataServicesRepository repository)
        {
            _repository = repository;
        }
        [HttpPost]
        public ActionResult Edit(bool? EnableODataServices)
        {
            if (((CustomPrincipal)User).IsAdmin)
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        ODataServices oDataServices = new ODataServices();
                        oDataServices.EnableODataServices = EnableODataServices == true ? true : false;
                        _repository.EditODataServices(oDataServices);
                        return RedirectToAction("Index", "Admin");
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", "Error editing record. " + ex.Message);
                    }
                }
            }
            return RedirectToAction("Index", "Home");
        }
        // GET: ODataServices
        public ActionResult Index()
        {
            if (((CustomPrincipal)User).IsAdmin)
            {
                ODataServices cp = _repository.GetODataServices();
                if (cp == null)
                    return RedirectToAction("Index");
                return View(cp);
            }
            else return RedirectToAction("Index", "Home");
        }
    }
}