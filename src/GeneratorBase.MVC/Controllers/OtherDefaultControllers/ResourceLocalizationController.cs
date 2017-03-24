using GeneratorBase.MVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
namespace GeneratorBase.MVC.Controllers
{
    public class ResourceLocalizationController : BaseController
    {
        public ActionResult Index()
        {
            if (!((CustomPrincipal)User).CanViewAdminFeature("UserInterfaceSetting"))
                return RedirectToAction("Index", "Home");
            var model = GeneratorBase.MVC.ModelReflector.Entities.Where(p => !p.IsAdminEntity && !p.IsDefault && p.Name.ToLower() != "filedocument" && (!Enum.IsDefined(typeof(GeneratorBase.MVC.ModelReflector.IgnoreEntities), p.Name))).ToList();
            
            return View(model);
        }
        public ActionResult Reset(string name)
        {
            var filepath = System.Web.HttpContext.Current.Server.MapPath("~/ResourceFiles/");
            var fileexist = System.IO.File.Exists(filepath + name + ".resx");
            if (fileexist)
            {
                System.IO.File.Delete(filepath + name + ".resx");
                var myConfiguration = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~");
                myConfiguration.AppSettings.Settings["ResourceLocalizationChange"].Value = Convert.ToString(DateTime.Now.Ticks);
                myConfiguration.Save();
            }
            return RedirectToAction("Index", "ResourceLocalization");
        }
        public ActionResult Create(string name)
        {
            ResourceLocalization model = new ResourceLocalization();
            model.Name = name;
            var Entity = GeneratorBase.MVC.ModelReflector.Entities.Where(p => !p.IsAdminEntity && !p.IsDefault).FirstOrDefault(p => p.Name == name);
            var filepath = System.Web.HttpContext.Current.Server.MapPath("~/ResourceFiles/");
            var fileexist = System.IO.File.Exists(filepath + name + ".resx");
            if (fileexist)
            {
                using (System.Resources.ResourceSet resxSet = new System.Resources.ResourceSet(filepath + name + ".resx"))
                {
                    ColumnMapping entobj = new ColumnMapping();
                    entobj.source = name;
                    entobj.target = resxSet.GetString(name);
                    model.columnmapping.Add(entobj);

                    foreach (var item in Entity.Properties.OrderBy(p => p.DisplayName))
                    {
                        if (item.Name == "DisplayValue") continue;
                        var result = resxSet.GetString(item.Name);
                        ColumnMapping obj = new ColumnMapping();
                        obj.source = item.Name;
                        obj.target = result;
                        model.columnmapping.Add(obj);
                    }
                }
            }
            else
            {
                ColumnMapping entobj = new ColumnMapping();
                entobj.source = name;
                entobj.target = Entity.DisplayName;
                model.columnmapping.Add(entobj);
                foreach (var item in Entity.Properties.OrderBy(p => p.DisplayName))
                {
                    if (item.Name == "DisplayValue") continue;
                    ColumnMapping obj = new ColumnMapping();
                    obj.source = item.Name;
                    obj.target = item.DisplayName;
                    model.columnmapping.Add(obj);
                }
            }

            return View(model);
        }
        [HttpPost]
        public ActionResult Create(ResourceLocalization model)
        {
            if (!((CustomPrincipal)User).CanAddAdminFeature("UserInterfaceSetting"))
                return RedirectToAction("Index", "Home");
            if (ModelState.IsValid)
            {
                var filepath = System.Web.HttpContext.Current.Server.MapPath("~/ResourceFiles/");
                var fileexist = System.IO.File.Exists(filepath + model.Name + ".resx");
                if (fileexist)
                {
                    System.IO.File.Delete(filepath + model.Name + ".resx");
                    //remove old file
                }
                using (System.Resources.ResourceWriter resxSet = new System.Resources.ResourceWriter(filepath + model.Name + ".resx"))
                {
                    foreach (var _model in model.columnmapping)
                    {
                        resxSet.AddResource(_model.source, _model.target);
                        //Create new file   
                    }
                }
                var myConfiguration = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~");
                myConfiguration.AppSettings.Settings["ResourceLocalizationChange"].Value = Convert.ToString(DateTime.Now.Ticks);
                myConfiguration.Save();
            }
            return RedirectToAction("Index", "ResourceLocalization");
        }
        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        db.Dispose();
        //    }
        //    base.Dispose(disposing);
        //}
    }

    public class ResourceLocalization
    {
        public ResourceLocalization()
        {
            this.columnmapping = new List<ColumnMapping>();
        }
        public ResourceLocalization(List<ColumnMapping> list)
            : this()
        {
            foreach (var lst in list)
                this.columnmapping.Add(lst);
        }

        public List<ColumnMapping> columnmapping { get; set; }
        public string Name { get; set; }
    }
    public class ColumnMapping
    {
        public string source { get; set; }
        public string target { get; set; }
    }
}
