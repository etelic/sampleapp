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
    public class T_EmployeeOrganizationAssociationController : ApiBaseController
    {
        // GET api/T_EmployeeOrganizationAssociationApi
        [EnableQuery]
        public HttpResponseMessage Get()
        {
            var _T_EmployeeOrganizationAssociation = db.T_EmployeeOrganizationAssociations;
            var objList = _T_EmployeeOrganizationAssociation as List<T_EmployeeOrganizationAssociation> ?? _T_EmployeeOrganizationAssociation.ToList();
            if (objList.Any())
                return Request.CreateResponse(HttpStatusCode.OK, objList);
            return Request.CreateErrorResponse(HttpStatusCode.NotFound, "EmployeeOrganizationAssociation not found");
        }
       
        // GET api/T_EmployeeOrganizationAssociationApi/5
       
        public HttpResponseMessage Get(long id)
        {
            var obj = db.T_EmployeeOrganizationAssociations.Find(id);
            if (obj != null)
                return Request.CreateResponse(HttpStatusCode.OK, obj);
            return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No EmployeeOrganizationAssociation found for this id");
        }

		public HttpResponseMessage Get(string searchKey)
        {
            var objList = searchRecords(db.T_EmployeeOrganizationAssociations, searchKey, true).ToList();
            if (objList.Any())
                return Request.CreateResponse(HttpStatusCode.OK, objList);
            return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No EmployeeOrganizationAssociation found");
        }

        // POST api/T_EmployeeOrganizationAssociationApi
        
        public HttpResponseMessage Post([FromBody] T_EmployeeOrganizationAssociation t_employeeorganizationassociation)
        {
            db.T_EmployeeOrganizationAssociations.Add(t_employeeorganizationassociation);
            db.SaveChanges();
			return Request.CreateResponse(HttpStatusCode.OK, t_employeeorganizationassociation.Id);
        }

        // PUT api/T_EmployeeOrganizationAssociationApi/5
       
        public bool Put(int id, [FromBody] T_EmployeeOrganizationAssociation t_employeeorganizationassociation)
        {
            if (id > 0)
            {
   
                db.Entry(t_employeeorganizationassociation).State = EntityState.Modified;
				db.SaveChanges();
                return true;
            }
            return false;
        }

        // DELETE api/T_EmployeeOrganizationAssociationApi/5
       
        public bool Delete(int id)
        {
            if (id > 0)
            {
                T_EmployeeOrganizationAssociation t_employeeorganizationassociation = db.T_EmployeeOrganizationAssociations.Find(id);

				                db.Entry(t_employeeorganizationassociation).State = EntityState.Deleted;
                db.T_EmployeeOrganizationAssociations.Remove(t_employeeorganizationassociation);
                db.SaveChanges();
                return true;
            }
            return false;
        }
		private IQueryable<T_EmployeeOrganizationAssociation> searchRecords(IQueryable<T_EmployeeOrganizationAssociation> lstT_EmployeeOrganizationAssociation, string searchString, bool? IsDeepSearch)
        {
		   if (string.IsNullOrEmpty(searchString)) return lstT_EmployeeOrganizationAssociation;
		   searchString = searchString.Trim();
		if(Convert.ToBoolean(IsDeepSearch))
            lstT_EmployeeOrganizationAssociation = lstT_EmployeeOrganizationAssociation.Where(s => (s.t_employee!= null && (s.t_employee.DisplayValue.ToUpper().Contains(searchString) )) ||(s.t_organization!= null && (s.t_organization.DisplayValue.ToUpper().Contains(searchString) )) ||(!String.IsNullOrEmpty(s.DisplayValue) && s.DisplayValue.ToUpper().Contains(searchString)));
		else
			 lstT_EmployeeOrganizationAssociation = lstT_EmployeeOrganizationAssociation.Where(s => (s.t_employee!= null && (s.t_employee.DisplayValue.ToUpper().Contains(searchString) )) ||(s.t_organization!= null && (s.t_organization.DisplayValue.ToUpper().Contains(searchString) )) ||(!String.IsNullOrEmpty(s.DisplayValue) && s.DisplayValue.ToUpper().Contains(searchString)));
            return lstT_EmployeeOrganizationAssociation;
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

