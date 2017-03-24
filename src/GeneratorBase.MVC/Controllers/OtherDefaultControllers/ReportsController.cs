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
using System.Configuration;
using GeneratorBase.MVC.WebReference;
using System.Text;
using System.Security.Principal;
namespace GeneratorBase.MVC.Controllers
{
    public class ReportsController : BaseController
    {
        public ActionResult ResultShow(string ReportName, string id)
        {
            ViewBag.Name = ReportName.Split('&')[0];
            int param = ReportName.Split('&').Length;
            if (param == 1)
                ViewBag.ReportName = ReportName;
            else
                ViewBag.ReportName = ConfigurationManager.AppSettings["ReportFolder"] + "/" + ReportName;
            return View();
        }
        public ActionResult Index(int? page, string searchString, int? itemsPerPage)
        {

            var items = new HashSet<CatalogItem>();
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            try
            {
                ReportingService2005 rs = new ReportingService2005();
                rs.Credentials = new System.Net.NetworkCredential(CommonFunction.Instance.ReportUser(), CommonFunction.Instance.ReportPass(), CommonFunction.Instance.ReportPath());
                var children = rs.ListChildren(CommonFunction.Instance.ReportFolder(), true);
                if (!String.IsNullOrEmpty(searchString))
                {
                    var matches = children.Where(m => m.Type == ItemTypeEnum.Report && m.Hidden == false);
                    foreach (var match in matches.Where(m => (!String.IsNullOrEmpty(m.Name) && m.Name.ToUpper().Contains(searchString.ToUpper())) || (!String.IsNullOrEmpty(m.Description) && m.Description.ToUpper().Contains(searchString.ToUpper()))))
                    {
                        items.Add(match);
                    }
                    ViewBag.CurrentFilter = searchString;
                }
                else
                {
                    var matches = children.Where(m => m.Type == ItemTypeEnum.Report && m.Hidden == false);
                    foreach (var match in matches)
                    {
                        items.Add(match);
                    }
                }
                if (itemsPerPage != null)
                {
                    pageSize = (int)itemsPerPage;
                    ViewBag.CurrentItemsPerPage = itemsPerPage;
                }
            }
            catch { }
            var _Reports = items.OrderBy(o => o.Name);
            return View(_Reports.ToPagedList(pageNumber, pageSize));
        }
       
    }
}

