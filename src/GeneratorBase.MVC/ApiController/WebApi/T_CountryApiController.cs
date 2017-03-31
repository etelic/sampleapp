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
    public class T_CountryController : ApiBaseController
    {
        // GET api/T_CountryApi
        [EnableQuery]
        public HttpResponseMessage Get()
        {
            var _T_Country = db.T_Countrys;
            var objList = _T_Country as List<T_Country> ?? _T_Country.ToList();
            if (objList.Any())
                return Request.CreateResponse(HttpStatusCode.OK, objList);
            return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Country not found");
        }
       
        // GET api/T_CountryApi/5
       
        public HttpResponseMessage Get(long id)
        {
            var obj = db.T_Countrys.Find(id);
            if (obj != null)
                return Request.CreateResponse(HttpStatusCode.OK, obj);
            return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No Country found for this id");
        }

		public HttpResponseMessage Get(string searchKey)
        {
            var objList = searchRecords(db.T_Countrys, searchKey, true).ToList();
            if (objList.Any())
                return Request.CreateResponse(HttpStatusCode.OK, objList);
            return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No Country found");
        }

        // POST api/T_CountryApi
        
        public HttpResponseMessage Post([FromBody] T_Country t_country)
        {
            db.T_Countrys.Add(t_country);
            db.SaveChanges();
			return Request.CreateResponse(HttpStatusCode.OK, t_country.Id);
        }

        // PUT api/T_CountryApi/5
       
        public bool Put(int id, [FromBody] T_Country t_country)
        {
            if (id > 0)
            {
   
                db.Entry(t_country).State = EntityState.Modified;
				db.SaveChanges();
                return true;
            }
            return false;
        }

        // DELETE api/T_CountryApi/5
       
        public bool Delete(int id)
        {
            if (id > 0)
            {
                T_Country t_country = db.T_Countrys.Find(id);

				                db.Entry(t_country).State = EntityState.Deleted;
                db.T_Countrys.Remove(t_country);
                db.SaveChanges();
                return true;
            }
            return false;
        }
		private IQueryable<T_Country> searchRecords(IQueryable<T_Country> lstT_Country, string searchString, bool? IsDeepSearch)
        {
		   if (string.IsNullOrEmpty(searchString)) return lstT_Country;
		   searchString = searchString.Trim();
		if(Convert.ToBoolean(IsDeepSearch))
            lstT_Country = lstT_Country.Where(s => (!String.IsNullOrEmpty(s.T_Name) && s.T_Name.ToUpper().Contains(searchString)) ||(!String.IsNullOrEmpty(s.T_Description) && s.T_Description.ToUpper().Contains(searchString)) ||(!String.IsNullOrEmpty(s.DisplayValue) && s.DisplayValue.ToUpper().Contains(searchString)));
		else
			 lstT_Country = lstT_Country.Where(s => (!String.IsNullOrEmpty(s.T_Name) && s.T_Name.ToUpper().Contains(searchString)) ||(!String.IsNullOrEmpty(s.T_Description) && s.T_Description.ToUpper().Contains(searchString)) ||(!String.IsNullOrEmpty(s.DisplayValue) && s.DisplayValue.ToUpper().Contains(searchString)));
            return lstT_Country;
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

