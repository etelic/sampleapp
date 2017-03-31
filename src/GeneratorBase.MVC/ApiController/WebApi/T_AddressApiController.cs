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
    public class T_AddressController : ApiBaseController
    {
        // GET api/T_AddressApi
        [EnableQuery]
        public HttpResponseMessage Get()
        {
            var _T_Address = db.T_Addresss;
            var objList = _T_Address as List<T_Address> ?? _T_Address.ToList();
            if (objList.Any())
                return Request.CreateResponse(HttpStatusCode.OK, objList);
            return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Address not found");
        }
       
        // GET api/T_AddressApi/5
       
        public HttpResponseMessage Get(long id)
        {
            var obj = db.T_Addresss.Find(id);
            if (obj != null)
                return Request.CreateResponse(HttpStatusCode.OK, obj);
            return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No Address found for this id");
        }

		public HttpResponseMessage Get(string searchKey)
        {
            var objList = searchRecords(db.T_Addresss, searchKey, true).ToList();
            if (objList.Any())
                return Request.CreateResponse(HttpStatusCode.OK, objList);
            return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No Address found");
        }

        // POST api/T_AddressApi
        
        public HttpResponseMessage Post([FromBody] T_Address t_address)
        {
            db.T_Addresss.Add(t_address);
            db.SaveChanges();
			return Request.CreateResponse(HttpStatusCode.OK, t_address.Id);
        }

        // PUT api/T_AddressApi/5
       
        public bool Put(int id, [FromBody] T_Address t_address)
        {
            if (id > 0)
            {
   
                db.Entry(t_address).State = EntityState.Modified;
				db.SaveChanges();
                return true;
            }
            return false;
        }

        // DELETE api/T_AddressApi/5
       
        public bool Delete(int id)
        {
            if (id > 0)
            {
                T_Address t_address = db.T_Addresss.Find(id);

				                db.Entry(t_address).State = EntityState.Deleted;
                db.T_Addresss.Remove(t_address);
                db.SaveChanges();
                return true;
            }
            return false;
        }
		private IQueryable<T_Address> searchRecords(IQueryable<T_Address> lstT_Address, string searchString, bool? IsDeepSearch)
        {
		   if (string.IsNullOrEmpty(searchString)) return lstT_Address;
		   searchString = searchString.Trim();
		if(Convert.ToBoolean(IsDeepSearch))
            lstT_Address = lstT_Address.Where(s => (!String.IsNullOrEmpty(s.T_AddressLine1) && s.T_AddressLine1.ToUpper().Contains(searchString)) ||(!String.IsNullOrEmpty(s.T_AddressLine2) && s.T_AddressLine2.ToUpper().Contains(searchString)) ||(!String.IsNullOrEmpty(s.T_ZipCode) && s.T_ZipCode.ToUpper().Contains(searchString)) ||(s.t_addresscountry!= null && (s.t_addresscountry.DisplayValue.ToUpper().Contains(searchString) )) ||(s.t_addressstate!= null && (s.t_addressstate.DisplayValue.ToUpper().Contains(searchString) )) ||(s.t_addresscity!= null && (s.t_addresscity.DisplayValue.ToUpper().Contains(searchString) )) ||(!String.IsNullOrEmpty(s.DisplayValue) && s.DisplayValue.ToUpper().Contains(searchString)));
		else
			 lstT_Address = lstT_Address.Where(s => (!String.IsNullOrEmpty(s.T_AddressLine1) && s.T_AddressLine1.ToUpper().Contains(searchString)) ||(!String.IsNullOrEmpty(s.T_AddressLine2) && s.T_AddressLine2.ToUpper().Contains(searchString)) ||(!String.IsNullOrEmpty(s.T_ZipCode) && s.T_ZipCode.ToUpper().Contains(searchString)) ||(s.t_addresscountry!= null && (s.t_addresscountry.DisplayValue.ToUpper().Contains(searchString) )) ||(s.t_addressstate!= null && (s.t_addressstate.DisplayValue.ToUpper().Contains(searchString) )) ||(s.t_addresscity!= null && (s.t_addresscity.DisplayValue.ToUpper().Contains(searchString) )) ||(!String.IsNullOrEmpty(s.DisplayValue) && s.DisplayValue.ToUpper().Contains(searchString)));
            return lstT_Address;
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

