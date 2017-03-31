using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data.Entity;
using AttributeRouting.Web.Http;
using GeneratorBase.MVC.Models;
using GeneratorBase.MVC.Controllers;
using Newtonsoft.Json;
using System.Web.Http.Description;
using System;
using System.Web.Http.OData;
using System.Data.Entity.SqlServer;
namespace GeneratorBase.MVC.ApiControllers
{
	[AuthorizationRequired]
    public class T_EmployeetypeController : ApiBaseController
    {
        // GET api/T_EmployeetypeApi
        [EnableQuery]
        public HttpResponseMessage Get()
        {
            var _T_Employeetype = db.T_Employeetypes;
            var objList = _T_Employeetype as List<T_Employeetype> ?? _T_Employeetype.ToList();
            if (objList.Any())
                return Request.CreateResponse(HttpStatusCode.OK, objList);
            return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Employee Type not found");
        }
       
        // GET api/T_EmployeetypeApi/5
       
        public HttpResponseMessage Get(long id)
        {
            var obj = db.T_Employeetypes.Find(id);
            if (obj != null)
                return Request.CreateResponse(HttpStatusCode.OK, obj);
            return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No Employee Type found for this id");
        }

		public HttpResponseMessage Get(string searchKey)
        {
            var objList = searchRecords(db.T_Employeetypes, searchKey, true).ToList();
            if (objList.Any())
                return Request.CreateResponse(HttpStatusCode.OK, objList);
            return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No Employee Type found");
        }

        // POST api/T_EmployeetypeApi
        
        public HttpResponseMessage Post([FromBody] T_Employeetype t_employeetype)
        {
            db.T_Employeetypes.Add(t_employeetype);
            db.SaveChanges();
			return Request.CreateResponse(HttpStatusCode.OK, t_employeetype.Id);
        }

        // PUT api/T_EmployeetypeApi/5
       
        public bool Put(int id, [FromBody] T_Employeetype t_employeetype)
        {
            if (id > 0)
            {
   
                db.Entry(t_employeetype).State = EntityState.Modified;
				db.SaveChanges();
                return true;
            }
            return false;
        }

        // DELETE api/T_EmployeetypeApi/5
       
        public bool Delete(int id)
        {
            if (id > 0)
            {
                T_Employeetype t_employeetype = db.T_Employeetypes.Find(id);

				                db.Entry(t_employeetype).State = EntityState.Deleted;
                db.T_Employeetypes.Remove(t_employeetype);
                db.SaveChanges();
                return true;
            }
            return false;
        }
		private IQueryable<T_Employeetype> searchRecords(IQueryable<T_Employeetype> lstT_Employeetype, string searchString, bool? IsDeepSearch)
        {
		   if (string.IsNullOrEmpty(searchString)) return lstT_Employeetype;
		   searchString = searchString.Trim();
		if(Convert.ToBoolean(IsDeepSearch))
            lstT_Employeetype = lstT_Employeetype.Where(s => (!String.IsNullOrEmpty(s.T_Name) && s.T_Name.ToUpper().Contains(searchString)) ||(!String.IsNullOrEmpty(s.T_Description) && s.T_Description.ToUpper().Contains(searchString)) ||(!String.IsNullOrEmpty(s.DisplayValue) && s.DisplayValue.ToUpper().Contains(searchString)));
		else
			 lstT_Employeetype = lstT_Employeetype.Where(s => (!String.IsNullOrEmpty(s.T_Name) && s.T_Name.ToUpper().Contains(searchString)) ||(!String.IsNullOrEmpty(s.T_Description) && s.T_Description.ToUpper().Contains(searchString)) ||(!String.IsNullOrEmpty(s.DisplayValue) && s.DisplayValue.ToUpper().Contains(searchString)));
            return lstT_Employeetype;
        }
		protected void DeleteDocument(long? docID)
        {
            var document = db.Documents.Find(docID);
            db.Entry(document).State = EntityState.Deleted;
            db.Documents.Remove(document);
            db.SaveChanges();
        }
		protected void DeleteImageGalleryDocument(string docIDs)
        {
            if (!String.IsNullOrEmpty(docIDs))
            {
                string[] strDocIds = docIDs.Split(',');
                foreach (string strDocId in strDocIds)
                {
                    var document = db.Documents.Find(Convert.ToInt64(strDocId));
                    db.Entry(document).State = EntityState.Deleted;
                    db.Documents.Remove(document);
                    db.SaveChanges();
                }
            }
        }
    }
}

