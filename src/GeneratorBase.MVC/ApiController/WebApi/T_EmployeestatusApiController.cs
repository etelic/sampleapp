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
    public class T_EmployeestatusController : ApiBaseController
    {
        // GET api/T_EmployeestatusApi
        [EnableQuery]
        public HttpResponseMessage Get()
        {
            var _T_Employeestatus = db.T_Employeestatuss;
            var objList = _T_Employeestatus as List<T_Employeestatus> ?? _T_Employeestatus.ToList();
            if (objList.Any())
                return Request.CreateResponse(HttpStatusCode.OK, objList);
            return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Employee Status not found");
        }
       
        // GET api/T_EmployeestatusApi/5
       
        public HttpResponseMessage Get(long id)
        {
            var obj = db.T_Employeestatuss.Find(id);
            if (obj != null)
                return Request.CreateResponse(HttpStatusCode.OK, obj);
            return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No Employee Status found for this id");
        }

		public HttpResponseMessage Get(string searchKey)
        {
            var objList = searchRecords(db.T_Employeestatuss, searchKey, true).ToList();
            if (objList.Any())
                return Request.CreateResponse(HttpStatusCode.OK, objList);
            return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No Employee Status found");
        }

        // POST api/T_EmployeestatusApi
        
        public HttpResponseMessage Post([FromBody] T_Employeestatus t_employeestatus)
        {
            db.T_Employeestatuss.Add(t_employeestatus);
            db.SaveChanges();
			return Request.CreateResponse(HttpStatusCode.OK, t_employeestatus.Id);
        }

        // PUT api/T_EmployeestatusApi/5
       
        public bool Put(int id, [FromBody] T_Employeestatus t_employeestatus)
        {
            if (id > 0)
            {
   
                db.Entry(t_employeestatus).State = EntityState.Modified;
				db.SaveChanges();
                return true;
            }
            return false;
        }

        // DELETE api/T_EmployeestatusApi/5
       
        public bool Delete(int id)
        {
            if (id > 0)
            {
                T_Employeestatus t_employeestatus = db.T_Employeestatuss.Find(id);

				                db.Entry(t_employeestatus).State = EntityState.Deleted;
                db.T_Employeestatuss.Remove(t_employeestatus);
                db.SaveChanges();
                return true;
            }
            return false;
        }
		private IQueryable<T_Employeestatus> searchRecords(IQueryable<T_Employeestatus> lstT_Employeestatus, string searchString, bool? IsDeepSearch)
        {
		   if (string.IsNullOrEmpty(searchString)) return lstT_Employeestatus;
		   searchString = searchString.Trim();
		if(Convert.ToBoolean(IsDeepSearch))
            lstT_Employeestatus = lstT_Employeestatus.Where(s => (!String.IsNullOrEmpty(s.T_Name) && s.T_Name.ToUpper().Contains(searchString)) ||(!String.IsNullOrEmpty(s.T_Description) && s.T_Description.ToUpper().Contains(searchString)) ||(!String.IsNullOrEmpty(s.DisplayValue) && s.DisplayValue.ToUpper().Contains(searchString)));
		else
			 lstT_Employeestatus = lstT_Employeestatus.Where(s => (!String.IsNullOrEmpty(s.T_Name) && s.T_Name.ToUpper().Contains(searchString)) ||(!String.IsNullOrEmpty(s.T_Description) && s.T_Description.ToUpper().Contains(searchString)) ||(!String.IsNullOrEmpty(s.DisplayValue) && s.DisplayValue.ToUpper().Contains(searchString)));
            return lstT_Employeestatus;
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

