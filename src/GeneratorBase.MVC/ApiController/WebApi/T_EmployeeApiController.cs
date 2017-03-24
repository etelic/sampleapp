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
    public class T_EmployeeController : ApiBaseController
    {
        // GET api/T_EmployeeApi
        [EnableQuery]
        public HttpResponseMessage Get()
        {
            var _T_Employee = db.T_Employees;
            var objList = _T_Employee as List<T_Employee> ?? _T_Employee.ToList();
            if (objList.Any())
                return Request.CreateResponse(HttpStatusCode.OK, objList);
            return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Employee not found");
        }
       
        // GET api/T_EmployeeApi/5
       
        public HttpResponseMessage Get(long id)
        {
            var obj = db.T_Employees.Find(id);
            if (obj != null)
                return Request.CreateResponse(HttpStatusCode.OK, obj);
            return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No Employee found for this id");
        }

		public HttpResponseMessage Get(string searchKey)
        {
            var objList = searchRecords(db.T_Employees, searchKey, true).ToList();
            if (objList.Any())
                return Request.CreateResponse(HttpStatusCode.OK, objList);
            return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No Employee found");
        }

        // POST api/T_EmployeeApi
        
        public HttpResponseMessage Post([FromBody] T_Employee t_employee)
        {
            db.T_Employees.Add(t_employee);
            db.SaveChanges();
			return Request.CreateResponse(HttpStatusCode.OK, t_employee.Id);
        }

        // PUT api/T_EmployeeApi/5
       
        public bool Put(int id, [FromBody] T_Employee t_employee)
        {
            if (id > 0)
            {
   
			 if(t_employee.t_employeeaddress != null && t_employee.t_employeeaddress.Id == 0)
                   db.Entry(t_employee.t_employeeaddress).State = EntityState.Added;
             else if(t_employee.t_employeeaddress != null && t_employee.t_employeeaddress.Id > 0)
                  db.Entry(t_employee.t_employeeaddress).State = EntityState.Modified;
                db.Entry(t_employee).State = EntityState.Modified;
				db.SaveChanges();
                return true;
            }
            return false;
        }

        // DELETE api/T_EmployeeApi/5
       
        public bool Delete(int id)
        {
            if (id > 0)
            {
                T_Employee t_employee = db.T_Employees.Find(id);

							if (t_employee.T_Picture != null)
            {
                DeleteDocument(t_employee.T_Picture);
            }
                db.Entry(t_employee).State = EntityState.Deleted;
                db.T_Employees.Remove(t_employee);
                db.SaveChanges();
                return true;
            }
            return false;
        }
		private IQueryable<T_Employee> searchRecords(IQueryable<T_Employee> lstT_Employee, string searchString, bool? IsDeepSearch)
        {
		   if (string.IsNullOrEmpty(searchString)) return lstT_Employee;
		   searchString = searchString.Trim();
		if(Convert.ToBoolean(IsDeepSearch))
            lstT_Employee = lstT_Employee.Where(s => (!String.IsNullOrEmpty(s.T_Title) && s.T_Title.ToUpper().Contains(searchString)) ||(!String.IsNullOrEmpty(s.T_FirstName) && s.T_FirstName.ToUpper().Contains(searchString)) ||(!String.IsNullOrEmpty(s.T_MiddleName) && s.T_MiddleName.ToUpper().Contains(searchString)) ||(!String.IsNullOrEmpty(s.T_LastName) && s.T_LastName.ToUpper().Contains(searchString)) ||(!String.IsNullOrEmpty(s.T_Email) && s.T_Email.ToUpper().Contains(searchString)) ||(!String.IsNullOrEmpty(s.T_PhoneNo) && s.T_PhoneNo.ToUpper().Contains(searchString)) ||(!String.IsNullOrEmpty(s.T_IdentificationNo) && s.T_IdentificationNo.ToUpper().Contains(searchString)) ||(s.t_associatedemployeetype!= null && (s.t_associatedemployeetype.DisplayValue.ToUpper().Contains(searchString) )) ||(s.t_associatedemployeestatus!= null && (s.t_associatedemployeestatus.DisplayValue.ToUpper().Contains(searchString) )) ||(s.t_employeeuserlogin!= null && (s.t_employeeuserlogin.UserName.ToUpper().Contains(searchString) )) ||(s.t_employeeaddress!= null && (s.t_employeeaddress.DisplayValue.ToUpper().Contains(searchString) )) ||(!String.IsNullOrEmpty(s.DisplayValue) && s.DisplayValue.ToUpper().Contains(searchString)));
		else
			 lstT_Employee = lstT_Employee.Where(s => (!String.IsNullOrEmpty(s.T_Title) && s.T_Title.ToUpper().Contains(searchString)) ||(!String.IsNullOrEmpty(s.T_FirstName) && s.T_FirstName.ToUpper().Contains(searchString)) ||(!String.IsNullOrEmpty(s.T_MiddleName) && s.T_MiddleName.ToUpper().Contains(searchString)) ||(!String.IsNullOrEmpty(s.T_LastName) && s.T_LastName.ToUpper().Contains(searchString)) ||(!String.IsNullOrEmpty(s.T_Email) && s.T_Email.ToUpper().Contains(searchString)) ||(!String.IsNullOrEmpty(s.T_PhoneNo) && s.T_PhoneNo.ToUpper().Contains(searchString)) ||(!String.IsNullOrEmpty(s.T_IdentificationNo) && s.T_IdentificationNo.ToUpper().Contains(searchString)) ||(s.t_associatedemployeetype!= null && (s.t_associatedemployeetype.DisplayValue.ToUpper().Contains(searchString) )) ||(s.t_associatedemployeestatus!= null && (s.t_associatedemployeestatus.DisplayValue.ToUpper().Contains(searchString) )) ||(s.t_employeeuserlogin!= null && (s.t_employeeuserlogin.UserName.ToUpper().Contains(searchString) )) ||(s.t_employeeaddress!= null && (s.t_employeeaddress.DisplayValue.ToUpper().Contains(searchString) )) ||(!String.IsNullOrEmpty(s.DisplayValue) && s.DisplayValue.ToUpper().Contains(searchString)));
            return lstT_Employee;
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

