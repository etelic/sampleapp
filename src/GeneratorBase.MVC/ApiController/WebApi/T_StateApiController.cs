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
    public class T_StateController : ApiBaseController
    {
        // GET api/T_StateApi
        [EnableQuery]
        public HttpResponseMessage Get()
        {
            var _T_State = db.T_States;
            var objList = _T_State as List<T_State> ?? _T_State.ToList();
            if (objList.Any())
                return Request.CreateResponse(HttpStatusCode.OK, objList);
            return Request.CreateErrorResponse(HttpStatusCode.NotFound, "State not found");
        }
       
        // GET api/T_StateApi/5
       
        public HttpResponseMessage Get(long id)
        {
            var obj = db.T_States.Find(id);
            if (obj != null)
                return Request.CreateResponse(HttpStatusCode.OK, obj);
            return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No State found for this id");
        }

		public HttpResponseMessage Get(string searchKey)
        {
            var objList = searchRecords(db.T_States, searchKey, true).ToList();
            if (objList.Any())
                return Request.CreateResponse(HttpStatusCode.OK, objList);
            return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No State found");
        }

        // POST api/T_StateApi
        
        public HttpResponseMessage Post([FromBody] T_State t_state)
        {
            db.T_States.Add(t_state);
            db.SaveChanges();
			return Request.CreateResponse(HttpStatusCode.OK, t_state.Id);
        }

        // PUT api/T_StateApi/5
       
        public bool Put(int id, [FromBody] T_State t_state)
        {
            if (id > 0)
            {
   
                db.Entry(t_state).State = EntityState.Modified;
				db.SaveChanges();
                return true;
            }
            return false;
        }

        // DELETE api/T_StateApi/5
       
        public bool Delete(int id)
        {
            if (id > 0)
            {
                T_State t_state = db.T_States.Find(id);

				                db.Entry(t_state).State = EntityState.Deleted;
                db.T_States.Remove(t_state);
                db.SaveChanges();
                return true;
            }
            return false;
        }
		private IQueryable<T_State> searchRecords(IQueryable<T_State> lstT_State, string searchString, bool? IsDeepSearch)
        {
		   if (string.IsNullOrEmpty(searchString)) return lstT_State;
		   searchString = searchString.Trim();
		if(Convert.ToBoolean(IsDeepSearch))
            lstT_State = lstT_State.Where(s => (!String.IsNullOrEmpty(s.T_Name) && s.T_Name.ToUpper().Contains(searchString)) ||(!String.IsNullOrEmpty(s.T_Description) && s.T_Description.ToUpper().Contains(searchString)) ||(s.t_statecountry!= null && (s.t_statecountry.DisplayValue.ToUpper().Contains(searchString) )) ||(!String.IsNullOrEmpty(s.DisplayValue) && s.DisplayValue.ToUpper().Contains(searchString)));
		else
			 lstT_State = lstT_State.Where(s => (!String.IsNullOrEmpty(s.T_Name) && s.T_Name.ToUpper().Contains(searchString)) ||(!String.IsNullOrEmpty(s.T_Description) && s.T_Description.ToUpper().Contains(searchString)) ||(s.t_statecountry!= null && (s.t_statecountry.DisplayValue.ToUpper().Contains(searchString) )) ||(!String.IsNullOrEmpty(s.DisplayValue) && s.DisplayValue.ToUpper().Contains(searchString)));
            return lstT_State;
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

