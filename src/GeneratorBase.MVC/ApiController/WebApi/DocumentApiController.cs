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
namespace GeneratorBase.MVC.ApiControllers
{
	[AuthorizationRequired]
    public class DocumentController : ApiBaseController
    {
        // GET api/DocumentApi
        [EnableQuery]
        public HttpResponseMessage Get()
        {
            var _Document = db.Documents;
            var objList = _Document as List<Document> ?? _Document.ToList();
            if (objList.Any())
                return Request.CreateResponse(HttpStatusCode.OK, objList);
            return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Document not found");
        }
       
        // GET api/DocumentApi/5
       
        public HttpResponseMessage Get(long id)
        {
            var obj = db.Documents.Find(id);
            if (obj != null)
                return Request.CreateResponse(HttpStatusCode.OK, obj);
            return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No Document found for this id");
        }

        // POST api/DocumentApi

        public HttpResponseMessage Post([FromBody] Document Document)
        {
            Document.DateCreated = Document.DateLastUpdated = DateTime.Now;
            db.Documents.Add(Document);
            db.SaveChanges();
            return Request.CreateResponse(HttpStatusCode.OK, Document.Id);
        }

        // PUT api/DocumentApi/5
       
        public bool Put(int id, [FromBody] Document Document)
        {
            if (id > 0)
            {
                Document.DateLastUpdated = DateTime.Now;
                db.Entry(Document).State = EntityState.Modified;
				db.SaveChanges();
                return true;
            }
            return false;
        }

        // DELETE api/DocumentApi/5
       
        public bool Delete(int id)
        {
            if (id > 0)
            {
                Document Document = db.Documents.Find(id);
                db.Entry(Document).State = EntityState.Deleted;
                db.Documents.Remove(Document);
                db.SaveChanges();
                return true;
            }
            return false;
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
