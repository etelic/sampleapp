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
    public class T_CityController : ApiBaseController
    {
        // GET api/T_CityApi
        [EnableQuery]
        public HttpResponseMessage Get()
        {
            var _T_City = db.T_Citys;
            var objList = _T_City as List<T_City> ?? _T_City.ToList();
            if (objList.Any())
                return Request.CreateResponse(HttpStatusCode.OK, objList);
            return Request.CreateErrorResponse(HttpStatusCode.NotFound, "City not found");
        }
       
        // GET api/T_CityApi/5
       
        public HttpResponseMessage Get(long id)
        {
            var obj = db.T_Citys.Find(id);
            if (obj != null)
                return Request.CreateResponse(HttpStatusCode.OK, obj);
            return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No City found for this id");
        }

		public HttpResponseMessage Get(string searchKey)
        {
            var objList = searchRecords(db.T_Citys, searchKey, true).ToList();
            if (objList.Any())
                return Request.CreateResponse(HttpStatusCode.OK, objList);
            return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No City found");
        }

        // POST api/T_CityApi
        
        public HttpResponseMessage Post([FromBody] T_City t_city)
        {
            db.T_Citys.Add(t_city);
            db.SaveChanges();
			return Request.CreateResponse(HttpStatusCode.OK, t_city.Id);
        }

        // PUT api/T_CityApi/5
       
        public bool Put(int id, [FromBody] T_City t_city)
        {
            if (id > 0)
            {
   
                db.Entry(t_city).State = EntityState.Modified;
				db.SaveChanges();
                return true;
            }
            return false;
        }

        // DELETE api/T_CityApi/5
       
        public bool Delete(int id)
        {
            if (id > 0)
            {
                T_City t_city = db.T_Citys.Find(id);

				                db.Entry(t_city).State = EntityState.Deleted;
                db.T_Citys.Remove(t_city);
                db.SaveChanges();
                return true;
            }
            return false;
        }
		private IQueryable<T_City> searchRecords(IQueryable<T_City> lstT_City, string searchString, bool? IsDeepSearch)
        {
		   if (string.IsNullOrEmpty(searchString)) return lstT_City;
		   searchString = searchString.Trim();
		if(Convert.ToBoolean(IsDeepSearch))
            lstT_City = lstT_City.Where(s => (!String.IsNullOrEmpty(s.T_Name) && s.T_Name.ToUpper().Contains(searchString)) ||(!String.IsNullOrEmpty(s.T_Description) && s.T_Description.ToUpper().Contains(searchString)) ||(s.t_citycountry!= null && (s.t_citycountry.DisplayValue.ToUpper().Contains(searchString) )) ||(s.t_citystate!= null && (s.t_citystate.DisplayValue.ToUpper().Contains(searchString) )) ||(!String.IsNullOrEmpty(s.DisplayValue) && s.DisplayValue.ToUpper().Contains(searchString)));
		else
			 lstT_City = lstT_City.Where(s => (!String.IsNullOrEmpty(s.T_Name) && s.T_Name.ToUpper().Contains(searchString)) ||(!String.IsNullOrEmpty(s.T_Description) && s.T_Description.ToUpper().Contains(searchString)) ||(s.t_citycountry!= null && (s.t_citycountry.DisplayValue.ToUpper().Contains(searchString) )) ||(s.t_citystate!= null && (s.t_citystate.DisplayValue.ToUpper().Contains(searchString) )) ||(!String.IsNullOrEmpty(s.DisplayValue) && s.DisplayValue.ToUpper().Contains(searchString)));
            return lstT_City;
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

