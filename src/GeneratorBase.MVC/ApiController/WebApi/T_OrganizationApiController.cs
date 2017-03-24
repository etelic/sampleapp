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
    public class T_OrganizationController : ApiBaseController
    {
        // GET api/T_OrganizationApi
        [EnableQuery]
        public HttpResponseMessage Get()
        {
            var _T_Organization = db.T_Organizations;
            var objList = _T_Organization as List<T_Organization> ?? _T_Organization.ToList();
            if (objList.Any())
                return Request.CreateResponse(HttpStatusCode.OK, objList);
            return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Organization not found");
        }
       
        // GET api/T_OrganizationApi/5
       
        public HttpResponseMessage Get(long id)
        {
            var obj = db.T_Organizations.Find(id);
            if (obj != null)
                return Request.CreateResponse(HttpStatusCode.OK, obj);
            return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No Organization found for this id");
        }

		public HttpResponseMessage Get(string searchKey)
        {
            var objList = searchRecords(db.T_Organizations, searchKey, true).ToList();
            if (objList.Any())
                return Request.CreateResponse(HttpStatusCode.OK, objList);
            return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No Organization found");
        }

        // POST api/T_OrganizationApi
        
        public HttpResponseMessage Post([FromBody] T_Organization t_organization)
        {
            db.T_Organizations.Add(t_organization);
            db.SaveChanges();
			return Request.CreateResponse(HttpStatusCode.OK, t_organization.Id);
        }

        // PUT api/T_OrganizationApi/5
       
        public bool Put(int id, [FromBody] T_Organization t_organization)
        {
            if (id > 0)
            {
   
                db.Entry(t_organization).State = EntityState.Modified;
				db.SaveChanges();
                return true;
            }
            return false;
        }

        // DELETE api/T_OrganizationApi/5
       
        public bool Delete(int id)
        {
            if (id > 0)
            {
                T_Organization t_organization = db.T_Organizations.Find(id);

				                db.Entry(t_organization).State = EntityState.Deleted;
                db.T_Organizations.Remove(t_organization);
                db.SaveChanges();
                return true;
            }
            return false;
        }
		private IQueryable<T_Organization> searchRecords(IQueryable<T_Organization> lstT_Organization, string searchString, bool? IsDeepSearch)
        {
		   if (string.IsNullOrEmpty(searchString)) return lstT_Organization;
		   searchString = searchString.Trim();
		if(Convert.ToBoolean(IsDeepSearch))
            lstT_Organization = lstT_Organization.Where(s => (!String.IsNullOrEmpty(s.T_Name) && s.T_Name.ToUpper().Contains(searchString)) ||(!String.IsNullOrEmpty(s.T_Description) && s.T_Description.ToUpper().Contains(searchString)) ||(!String.IsNullOrEmpty(s.DisplayValue) && s.DisplayValue.ToUpper().Contains(searchString)));
		else
			 lstT_Organization = lstT_Organization.Where(s => (!String.IsNullOrEmpty(s.T_Name) && s.T_Name.ToUpper().Contains(searchString)) ||(!String.IsNullOrEmpty(s.T_Description) && s.T_Description.ToUpper().Contains(searchString)) ||(!String.IsNullOrEmpty(s.DisplayValue) && s.DisplayValue.ToUpper().Contains(searchString)));
            return lstT_Organization;
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

