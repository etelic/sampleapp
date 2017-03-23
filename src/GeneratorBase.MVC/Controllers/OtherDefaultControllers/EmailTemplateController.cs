using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GeneratorBase.MVC.Models;
using System.Linq.Expressions;
using System.Reflection;
namespace GeneratorBase.MVC.Controllers.OtherDefaultControllers
{
    public class EmailTemplateController : BaseController
    {
        public ActionResult Index()
        {
            var lstEmailTemplate = from s in db.EmailTemplates
                                select s;
            var _EmailTemplate = lstEmailTemplate.Include(t => t.associatedemailtemplatetype);
            ViewBag.EmailTemplateType = db.EmailTemplateTypes.ToList();
            if (!Request.IsAjaxRequest())
                return View(_EmailTemplate);
            else
                return PartialView("IndexPartial", _EmailTemplate);
        }
        public ActionResult Create(string UrlReferrer, bool? IsAddPop)
        {
            if (!User.CanAdd("EmailTemplate"))
            {
                return RedirectToAction("Index", "Error");
            }
            ViewBag.IsAddPop = IsAddPop;
            ViewData["EmailTemplateParentUrl"] = UrlReferrer;
            var objAssociatedEmailTemplateType = new List<EmailTemplateType>();
            ViewBag.AssociatedEmailTemplateTypeID = new SelectList(objAssociatedEmailTemplateType, "ID", "DisplayValue");
            return View();
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create([Bind(Include = "Id,ConcurrencyKey,AssociatedEmailTemplateTypeID,EmailContent")] EmailTemplate emailtemplate, string UrlReferrer, bool? IsAddPop)
        {
            if (ModelState.IsValid)
            {
                db.EmailTemplates.Add(emailtemplate);
                db.SaveChanges();
            }
            return RedirectToAction("Index", "EmailTemplate");
        }
        public ActionResult Edit(long? id, string UrlReferrer, bool? IsAddPop)
        {
            if (!User.CanAdd("EmailTemplate"))
            {
                return RedirectToAction("Index", "Error");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EmailTemplate emailtemplate = db.EmailTemplates.Find(id);
            if (emailtemplate == null)
            {
                return HttpNotFound();
            }
            ViewBag.IsAddPop = IsAddPop;
            ViewData["EmailTemplateParentUrl"] = UrlReferrer;
            var objAssociatedEmailTemplateType = new List<EmailTemplateType>();
            objAssociatedEmailTemplateType.Add(emailtemplate.associatedemailtemplatetype);
            ViewBag.AssociatedEmailTemplateTypeID = new SelectList(objAssociatedEmailTemplateType, "ID", "DisplayValue", emailtemplate.AssociatedEmailTemplateTypeID);
            return View(emailtemplate);
        }
        [HttpPost]
        public ActionResult Edit([Bind(Include = "Id,ConcurrencyKey,AssociatedEmailTemplateTypeID,EmailContent")] EmailTemplate emailtemplate, string UrlReferrer, bool? IsAddPop)
        {
            if (ModelState.IsValid)
            {
                db.Entry(emailtemplate).State = EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("Index", "EmailTemplate");
        }
        public ActionResult DeleteEmailTemplate(long? id, string UrlReferrer, bool? IsAddPop)
        {
            if (!User.CanDelete("EmailTemplate"))
            {
                return RedirectToAction("Index", "Error");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EmailTemplate emailtemplate = db.EmailTemplates.Find(id);
            if (emailtemplate == null)
            {
                throw (new Exception("Deleted"));
            }
            ViewBag.IsAddPop = IsAddPop;
            ViewData["EmailTemplateParentUrl"] = UrlReferrer;
            return View(emailtemplate);
        }
        [HttpPost]
        public ActionResult DeleteEmailTemplateConfirmed(EmailTemplate emailtemplate, string UrlReferrer)
        {
            if (!User.CanDelete("EmailTemplate"))
            {
                return RedirectToAction("Index", "Error");
            }
            db.Entry(emailtemplate).State = EntityState.Deleted;
            db.EmailTemplates.Remove(emailtemplate);
            db.SaveChanges();
            return Json("FROMPOPUP", "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
        }
        public ActionResult CreateEmailTemplateType(string UrlReferrer, bool? IsAddPop)
        {
            if (!User.CanAdd("EmailTemplateType"))
            {
                return RedirectToAction("Index", "Error");
            }
            ViewBag.IsAddPop = IsAddPop;
            ViewData["EmailTemplateTypeParentUrl"] = UrlReferrer;
            return View();
        }
        [HttpPost]
        public ActionResult CreateEmailTemplateType([Bind(Include = "Id,ConcurrencyKey,Name,IsDefault")] EmailTemplateType emailtemplatetype, string UrlReferrer, bool? IsAddPop)
        {
            if (ModelState.IsValid)
            {
                db.EmailTemplateTypes.Add(emailtemplatetype);
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
        public ActionResult EditEmailTemplateType(long? id, string UrlReferrer, bool? IsAddPop)
        {
            if (!User.CanAdd("EmailTemplateType"))
            {
                return RedirectToAction("Index", "Error");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EmailTemplateType emailtemplatetype = db.EmailTemplateTypes.Find(id);
            if (emailtemplatetype == null)
            {
                return HttpNotFound();
            }
            ViewBag.IsAddPop = IsAddPop;
            ViewData["EmailTemplateTypeParentUrl"] = UrlReferrer;
            return View(emailtemplatetype);
        }
        [HttpPost]
        public ActionResult EditEmailTemplateType([Bind(Include = "Id,ConcurrencyKey,Name,IsDefault")] EmailTemplateType emailtemplatetype, string UrlReferrer, bool? IsAddPop)
        {
            if (ModelState.IsValid)
            {
                db.Entry(emailtemplatetype).State = EntityState.Modified;
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
        public ActionResult DeleteEmailTemplateType(long? id, string UrlReferrer, bool? IsAddPop)
        {
            if (!User.CanDelete("EmailTemplateType"))
            {
                return RedirectToAction("Index", "Error");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EmailTemplateType emailtemplatetype = db.EmailTemplateTypes.Find(id);
            if (emailtemplatetype == null)
            {
                throw (new Exception("Deleted"));
            }
            ViewBag.IsAddPop = IsAddPop;
            ViewData["EmailTemplateTypeParentUrl"] = UrlReferrer;
            return View(emailtemplatetype);
        }
        [HttpPost]
        public ActionResult DeleteEmailTemplateTypeConfirmed(EmailTemplateType emailtemplatetype, string UrlReferrer)
        {
            if (!User.CanDelete("EmailTemplateType"))
            {
                return RedirectToAction("Index", "Error");
            }
            db.Entry(emailtemplatetype).State = EntityState.Deleted;
            db.EmailTemplateTypes.Remove(emailtemplatetype);
            db.SaveChanges();
            return Json("FROMPOPUP", "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Cancel(string UrlReferrer)
        {
            if (!string.IsNullOrEmpty(UrlReferrer))
            {
                var uri = new Uri(UrlReferrer);
                var query = HttpUtility.ParseQueryString(uri.Query);
                if (Convert.ToBoolean(query.Get("IsFilter")) == true)
                    return RedirectToAction("Index");
                else
                    return Redirect(UrlReferrer);
            }
            else
                return RedirectToAction("Index");
        }
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetAllValue(string caller, string key, string AssoNameWithParent, string AssociationID, string ExtraVal)
        {
            IQueryable<EmailTemplateType> list = db.EmailTemplateTypes;
            if (AssoNameWithParent != null && !string.IsNullOrEmpty(AssociationID))
            {
                Nullable<long> AssoID = Convert.ToInt64(AssociationID);
                if (AssoID != null && AssoID > 0)
                {
                    IQueryable query = db.EmailTemplateTypes;
                    Type[] exprArgTypes = { query.ElementType };
                    string propToWhere = AssoNameWithParent;
                    ParameterExpression p = Expression.Parameter(typeof(EmailTemplateType), "p");
                    MemberExpression member = Expression.PropertyOrField(p, propToWhere);
                    LambdaExpression lambda = Expression.Lambda<Func<EmailTemplateType, bool>>(Expression.Equal(member, Expression.Convert(Expression.Constant(AssoID), member.Type)), p);
                    MethodCallExpression methodCall = Expression.Call(typeof(Queryable), "Where", exprArgTypes, query.Expression, lambda);
                    IQueryable q = query.Provider.CreateQuery(methodCall);
                    list = ((IQueryable<EmailTemplateType>)q);
                }
            }
            if (list.Count() > 0)
                list = list.Where(p => !(db.EmailTemplates.Select(et => et.AssociatedEmailTemplateTypeID).Contains(p.Id)));
            if (key != null && key.Length > 0)
            {
                if (ExtraVal != null && ExtraVal.Length > 0)
                {
                    long? val = Convert.ToInt64(ExtraVal);
                    var data = from x in list.Where(p => p.DisplayValue.Contains(key) && p.Id != val).Take(9).Union(list.Where(p => p.Id == val)).OrderBy(q => q.DisplayValue).ToList()
                               select new { Id = x.Id, Name = x.DisplayValue };
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var data = from x in list.Where(p => p.DisplayValue.Contains(key)).Take(10).OrderBy(q => q.DisplayValue).ToList()
                               select new { Id = x.Id, Name = x.DisplayValue };
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                if (ExtraVal != null && ExtraVal.Length > 0)
                {
                    long? val = Convert.ToInt64(ExtraVal);
                    var data = from x in list.Where(p => p.Id != val).Take(9).Union(list.Where(p => p.Id == val)).Distinct().OrderBy(q => q.DisplayValue).ToList()
                               select new { Id = x.Id, Name = x.DisplayValue };
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var data = from x in list.Take(10).OrderBy(q => q.DisplayValue).ToList()
                               select new { Id = x.Id, Name = x.DisplayValue };
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
            }
        }
    }
}
