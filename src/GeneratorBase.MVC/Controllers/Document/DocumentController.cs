using System;
using System.Web.Helpers;
using System.Web.Mvc;
namespace GeneratorBase.MVC.Controllers
{
    public partial class DocumentController : BaseController
    {
        public FileResult Download(long id)
        {
            //check download permission;
            return ExportCode(id);
        }
        protected FileContentResult ExportCode(long id)
        {
            var doc = db.Documents.Find(id);
            if (doc == null)
                return null;
            byte[] fileBytes = doc.Byte;
            string fileName = doc.DisplayValue;
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetDocumentName(long? id)
        {

            var doc = db.Documents.Find(id);
            if (doc == null)
                return Json("NA", JsonRequestBehavior.AllowGet);
            else
                return Json(doc.DocumentName, JsonRequestBehavior.AllowGet);
        }
        public void DisplayImage(long id, int? maxSize, int? maxHeight, int? maxWidth)
        {
            //maxSize = 30;
            int height = Math.Min(maxSize ?? Int32.MaxValue, maxHeight ?? Int32.MaxValue);
            int width = Math.Min(maxSize ?? Int32.MaxValue, maxWidth ?? Int32.MaxValue);
            var doc = db.Documents.Find(id);
            if (doc == null)
                return;
            var wi = new WebImage(doc.Byte);
            wi.Resize(width, height, preventEnlarge: true);
            wi.Write();
        }
        public FileResult DisplayImage_old(long id)
        {
            var doc = db.Documents.Find(id);
            if (doc == null)
                return null;
            byte[] fileBytes = doc.Byte;
            string fileName = doc.DisplayValue;
            return File(fileBytes, doc.MIMEType.ToString());
        }
        public FileResult DisplayImageAfterhover(long id)
        {
            var doc = db.Documents.Find(id);
            if (doc == null)
                return null;
            byte[] fileBytes = doc.Byte;
            string fileName = doc.DisplayValue;
            return File(fileBytes, doc.MIMEType.ToString());
        }
        public FileResult DisplayImageAfterClick(long id)
        {
            var doc = db.Documents.Find(id);
            if (doc == null)
                return null;
            byte[] fileBytes = doc.Byte;
            string fileName = doc.DisplayValue;
            return File(fileBytes, doc.MIMEType.ToString());
        }
        public FileResult DisplayImageMobile(long id)
        {
            var doc = db.Documents.Find(id);
            if (doc == null)
                return null;
            byte[] fileBytes = doc.Byte;
            string fileName = doc.DisplayValue;
            return File(fileBytes, doc.MIMEType.ToString());
        }
        public void DisplayImageMobileList(long id, int? maxSize, int? maxHeight, int? maxWidth)
        {
            maxSize = 85;
            int height = Math.Min(maxSize ?? Int32.MaxValue, maxHeight ?? Int32.MaxValue);
            int width = Math.Min(maxSize ?? Int32.MaxValue, maxWidth ?? Int32.MaxValue);
            var doc = db.Documents.Find(id);
            if (doc == null)
                return;
            var wi = new WebImage(doc.Byte);
            wi.Resize(width, height, preventEnlarge: true);
            wi.Write();
        }
    }
}