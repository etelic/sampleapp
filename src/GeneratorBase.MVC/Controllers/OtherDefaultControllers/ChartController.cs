using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GeneratorBase.MVC.Models;
using System.Net;
using System.Data.Entity;

namespace GeneratorBase.MVC.Controllers
{
    public class ChartController : BaseController
    {
        //
        // GET: /Chart/
        public ActionResult Index()
        {
            var lstchart = from s in db.Charts
                           select s;
            if (!Request.IsAjaxRequest())
                return View(lstchart);
            else
                return PartialView("IndexPartial", lstchart);

            return View();
        }

        //
        // GET: /Chart/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        public ActionResult CreateQuick(string UrlReferrer, bool? IsAddPop)
        {
            if (!User.CanAdd("T_Chart"))
            {
                return RedirectToAction("Index", "Error");
            }
            ViewBag.IsAddPop = IsAddPop;
            ViewData["T_ChartgParentUrl"] = UrlReferrer;

            ViewBag.EntityName = new SelectList(ModelReflector.Entities.Where(ent => ent.IsDefault == false), "Name", "DisplayName");
            var ChartType = from System.Web.UI.DataVisualization.Charting.SeriesChartType s in Enum.GetValues(typeof(System.Web.UI.DataVisualization.Charting.SeriesChartType))
                            select new { ID = s, Name = s.ToString() };
            ViewBag.ChartType = new SelectList(ChartType, "Name", "Name");

            Dictionary<string, string> objXAxis = new Dictionary<string, string>();
            objXAxis.Add("0", "--Select Property--");
            ViewBag.XAxis = new SelectList(objXAxis, "key", "value");

            Dictionary<string, string> objYAxis = new Dictionary<string, string>();
            objYAxis.Add("0", "--Select Property--");
            ViewBag.YAxis = new SelectList(objYAxis, "key", "value");
            
            return View();
        }

        [HttpPost]
        public ActionResult CreateQuick([Bind(Include = "Id,ConcurrencyKey,EntityName,ChartTitle,ChartType,XAxis,YAxis,ShowInDashBoard")] T_Chart chart, string UrlReferrer, bool? IsAddPop)
        {
            if (ModelState.IsValid)
            {
                db.Charts.Add(chart);
                db.SaveChanges();

                return Json("FROMPOPUP", "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var errors = "";
                foreach (ModelState modelState in ViewData.ModelState.Values)
                {
                    foreach (ModelError error in modelState.Errors)
                    {
                        errors += error.ErrorMessage + ".  ";
                    }
                }
                return Json(errors, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
            }
            return View();
        }
        
        //
        // GET: /Chart/Edit/5
        public ActionResult Edit(long? id, string UrlReferrer, bool? IsAddPop)
        {
             if (!User.CanEdit("T_Chart"))
            {
                return RedirectToAction("Index", "Error");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            T_Chart chart = db.Charts.Find(id);
            if (chart == null)
            {
                return HttpNotFound();
            }

            ViewBag.EntityName = new SelectList(ModelReflector.Entities.Where(ent => ent.IsDefault == false), "Name", "DisplayName", chart.EntityName);
            var ChartType = from System.Web.UI.DataVisualization.Charting.SeriesChartType s in Enum.GetValues(typeof(System.Web.UI.DataVisualization.Charting.SeriesChartType))
                            select new { ID = s, Name = s.ToString() };
            ViewBag.ChartType = new SelectList(ChartType, "Name", "Name", chart.ChartType);

            var Properties = ModelReflector.Entities.FirstOrDefault(ent => ent.IsDefault == false && ent.Name == chart.EntityName).Properties.Select(prp => new { prp.Name, prp.DisplayName });
            Dictionary<string, string> objXAxis = new Dictionary<string, string>();
            
            objXAxis.Add("0", "--Select Property--");
            foreach (var prop in Properties)
                objXAxis.Add(prop.Name, prop.DisplayName);
            ViewBag.XAxis = new SelectList(objXAxis, "key", "value", chart.XAxis);

            Dictionary<string, string> objYAxis = new Dictionary<string, string>();
            objYAxis.Add("0", "--Select Property--");
            foreach (var prop in Properties)
                objYAxis.Add(prop.Name, prop.DisplayName);
            ViewBag.YAxis = new SelectList(objYAxis, "key", "value", chart.YAxis);
            ViewBag.IsAddPop = IsAddPop;
            ViewData["ChartParentUrl"] = UrlReferrer;
            return View(chart);
        }

        //
        // POST: /Chart/Edit/5
        [HttpPost]
        public ActionResult EditQuick([Bind(Include = "Id,ConcurrencyKey,EntityName,ChartTitle,ChartType,XAxis,YAxis,ShowInDashBoard")] T_Chart chart, string UrlReferrer, bool? IsAddPop)
        {
            if (ModelState.IsValid)
            {
                //string command = Request.Form["hdncommand"];
                db.Entry(chart).State = EntityState.Modified;
                db.SaveChanges();

                return Json("FROMPOPUP", "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var errors = "";
                foreach (ModelState modelState in ViewData.ModelState.Values)
                {
                    foreach (ModelError error in modelState.Errors)
                    {
                        errors += error.ErrorMessage + ".  ";
                    }
                }
                return Json(errors, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
            }
            return View(chart);
        }

        public ActionResult Delete(long? id, string UrlReferrer, bool? IsAddPop)
        {
            if (!User.CanDelete("T_Chart"))
            {
                return RedirectToAction("Index", "Error");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            T_Chart chart = db.Charts.Find(id);

            if (chart == null)
            {
                throw (new Exception("Deleted"));
            }
            ViewBag.IsAddPop = IsAddPop;
            ViewData["T_ChartParentUrl"] = UrlReferrer;
            return View(chart);
        }

        [HttpPost]
        public ActionResult DeleteConfirmed(T_Chart chart, string UrlReferrer)
        {
            if (!User.CanDelete("T_Chart"))
            {
                return RedirectToAction("Index", "Error");
            }

            db.Entry(chart).State = EntityState.Deleted;
            db.Charts.Remove(chart);
            db.SaveChanges();

            return Json("FROMPOPUP", "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
        }
    }
}
