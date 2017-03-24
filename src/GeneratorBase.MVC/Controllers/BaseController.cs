using GeneratorBase.MVC.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
namespace GeneratorBase.MVC.Controllers
{
    [NoCache]
    public class BaseController : Controller
    {
        public ApplicationContext db { get; private set; } //removed static for race condition
        public new IUser User { get; private set; }//removed static for race condition
        public static string FavoriteUrl { get; private set; }
        public static string FavoriteUrlEntityName { get; private set; }
        public static bool IsFavorite = false;
        public static string EntityNameJournal = null;
        public static FavoriteItem objFavorite = null;
        protected override void OnAuthorization(AuthorizationContext filterContext)
        {
            User = base.User as IUser;
            db = new ApplicationContext(base.User as IUser);
        }
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!Request.IsAuthenticated)
            {
                // filterContext.Result = new RedirectResult("~/Account/Login");
                var values = new RouteValueDictionary(new
                {
                    action = "Login",
                    controller = "Account",
                    returnUrl = HttpContext.Request.Url.PathAndQuery
                });
                var result = new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest, "Bad Request");
                if (Request.IsAjaxRequest())
                    filterContext.Result = result;
                else
                    filterContext.Result = new RedirectToRouteResult(values);
                return;
            }
            if (Request.UrlReferrer != null && Request.UrlReferrer.AbsolutePath.EndsWith("/Account/Login"))
            {
                ApplicationDbContext usercontext = new ApplicationDbContext();
                var userid = usercontext.Users.FirstOrDefault(p => p.UserName == User.Name).Id;
                LoginAttempts history = new LoginAttempts();
                history.UserId = userid;
                history.Date = DateTime.UtcNow;
                history.IsSuccessfull = true;
                history.IPAddress = Request.UserHostAddress;
                usercontext.LoginAttempts.Add(history);
                usercontext.SaveChanges();

                string applySecurityPolicy = db.AppSettings.Where(p => p.Key == "ApplySecurityPolicy").FirstOrDefault().Value;
                int duration = Convert.ToInt32(db.AppSettings.Where(p => p.Key == "PasswordExpirationInDays").FirstOrDefault().Value);
                if ((applySecurityPolicy.ToLower() == "yes") && !(((CustomPrincipal)User).Identity is System.Security.Principal.WindowsIdentity))
                    if (IsPasswordExpired(duration, userid))
                        filterContext.Result = new RedirectResult("~/Account/Manage");
            }
            if (Request.Url.PathAndQuery.ToUpper().Contains("/HOME?ISTHIRDPARTY=TRUE") || (Request.UrlReferrer != null && Request.UrlReferrer.AbsolutePath.EndsWith("/Account/Login") && !Request.Url.PathAndQuery.Contains("/Home?RegistrationEntity")))
            {
                ApplicationDbContext usercontext = new ApplicationDbContext();
                var userid = usercontext.Users.FirstOrDefault(p => p.UserName == User.Name).Id;
                if (IsAutoRegistration(userid))
                {
                    filterContext.Result = Redirect(Url.Action("Index", "Home", new { RegistrationEntity = string.Join(",", User.permissions.Where(p => p.SelfRegistration.Value).Select(p => p.EntityName)), TokenId = userid }));
                }
                if (Request.Url.PathAndQuery.ToUpper().Contains("/BULKUPDATE"))
                {
                    filterContext.Result = Redirect(Url.Action("Index", "Home"));
                }
            }
            
            objFavorite = db.FavoriteItems.Where(p => p.LastUpdatedByUser == User.Name && p.LinkAddress == HttpContext.Request.Url.PathAndQuery).FirstOrDefault();
            string entity = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
            if (User.CanView(entity))
            {
                base.OnActionExecuting(filterContext);
            }
            else
            {
                filterContext.Result = new RedirectResult("~/Error");
            }
            base.OnActionExecuting(filterContext);
        }
        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var EntityName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
            if (!(Request.IsAjaxRequest() || EntityName.ToLower() == "document"))
            {
                FavoriteUrlEntityName = EntityName;
                FavoriteUrl = getfavoriteUrl();
            }

            base.OnActionExecuted(filterContext);
        }
        protected long SaveDocument(HttpPostedFileBase file)
        {
            string fileExt = "";
            string filename = "";
            long fileSize = 0;
            Document document = new Document();
            document.DocumentName = file.FileName;
            filename = System.IO.Path.GetFileName(file.FileName);
            fileExt = System.IO.Path.GetExtension(file.FileName);
            fileSize = file.ContentLength;
            byte[] fileData = null;
            using (var binaryReader = new BinaryReader(file.InputStream))
            {
                fileData = binaryReader.ReadBytes(file.ContentLength);
            }
            document.DocumentName = filename;
            document.DateCreated = System.DateTime.Now.Date;
            document.DateLastUpdated = System.DateTime.Now.Date;
            document.FileExtension = fileExt;
            document.DisplayValue = System.IO.Path.GetFileName(file.FileName);
            document.FileName = System.IO.Path.GetFileName(file.FileName);
            document.FileSize = fileSize;
            document.MIMEType = file.ContentType;
            document.Byte = fileData;
            db.Documents.Add(document);
            db.SaveChanges();
            return document.Id;
        }
        protected long UpdateDocument(HttpPostedFileBase file, long? docId)
        {
            var document = db.Documents.Find(docId);
            if (document == null)
                return SaveDocument(file);
            string fileExt = "";
            string filename = "";
            long fileSize = 0;
            //Document document = new Document();
            document.DocumentName = file.FileName;
            filename = System.IO.Path.GetFileName(file.FileName);
            fileExt = System.IO.Path.GetExtension(file.FileName);
            fileSize = file.ContentLength;
            byte[] fileData = null;
            using (var binaryReader = new BinaryReader(file.InputStream))
            {
                fileData = binaryReader.ReadBytes(file.ContentLength);
            }
            document.DocumentName = filename;
            document.DateCreated = System.DateTime.Now.Date;
            document.DateLastUpdated = System.DateTime.Now.Date;
            document.FileExtension = fileExt;
            document.DisplayValue = filename;
            document.FileName = filename;
            document.FileSize = fileSize;
            document.MIMEType = file.ContentType;
            document.Byte = fileData;
            //db.Documents.Add(document);
            db.Entry(document).State = EntityState.Modified;
            db.SaveChanges();
            return document.Id;
        }
        //add/upadte for camera
        protected long SaveDocumentCameraImage(string ext, string camfilename, byte[] filebyte, long ContentLength, int len)
        {
            string fileExt = "";
            string filename = "";
            long fileSize = 0;
            Document document = new Document();
            document.DocumentName = filename;
            filename = camfilename;
            fileExt = ext;
            fileSize = ContentLength;
            byte[] fileData = null;
            using (var binaryReader = new BinaryReader(new MemoryStream(filebyte)))
            {
                fileData = binaryReader.ReadBytes(len);
            }
            document.DocumentName = filename;
            document.DateCreated = System.DateTime.Now.Date;
            document.DateLastUpdated = System.DateTime.Now.Date;
            document.FileExtension = fileExt;
            document.DisplayValue = filename;
            document.FileName = filename;
            document.FileSize = fileSize;
            document.MIMEType = "image/png";
            document.Byte = fileData;
            db.Documents.Add(document);
            db.SaveChanges();
            return document.Id;
        }
        protected long UpdateDocumentCamera(string ext, string camfilename, byte[] filebyte, long ContentLength, int len, long? docId)
        {
            var document = db.Documents.Find(docId);
            if (document == null)
                return SaveDocumentCameraImage(ext, camfilename, filebyte, ContentLength, len);
            string fileExt = "";
            string filename = "";
            long fileSize = 0;
            document.DocumentName = camfilename;
            filename = camfilename;
            fileExt = ext;
            fileSize = ContentLength;
            byte[] fileData = null;
            using (var binaryReader = new BinaryReader(new MemoryStream(filebyte)))
            {
                fileData = binaryReader.ReadBytes(len);
            }
            document.DocumentName = filename;
            document.DateCreated = System.DateTime.Now.Date;
            document.DateLastUpdated = System.DateTime.Now.Date;
            document.FileExtension = fileExt;
            document.DisplayValue = filename;
            document.FileName = filename;
            document.FileSize = fileSize;
            document.MIMEType = "image/png";
            document.Byte = fileData;
            db.Entry(document).State = EntityState.Modified;
            db.SaveChanges();
            return document.Id;
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
        protected bool IsFileTypeAndSizeAllowed(HttpPostedFileBase file)
        {
            var fileType = db.AppSettings.Where(p => p.Key == "FileTypes").FirstOrDefault();
            var fileSize = db.AppSettings.Where(p => p.Key == "FileSize").FirstOrDefault();
            var fileExt = System.IO.Path.GetExtension(file.FileName).Replace(".", "").Trim();
            bool isFileValid = true;
            string alertMsg = "Validation Alert - ";
            if (fileType != null)
                if (!(fileType.Value.ToLower().Contains(fileExt.ToLower())))
                {
                    alertMsg = alertMsg + "File type is not proper. Accepts only [" + fileType.Value + "] file types)";
                    isFileValid = false;
                }
            if (fileSize != null)
                if (!(Convert.ToInt32(fileSize.Value) * 1024 * 1024 >= file.ContentLength))
                {
                    alertMsg = alertMsg + " File size is not proper. Accepts file of size less than [ " + fileSize.Value + " MB ])";
                    isFileValid = false;
                }
            if (!isFileValid)
            {
                ModelState.AddModelError("CustomError", alertMsg);
                ViewBag.ApplicationError = alertMsg;
            }
            return isFileValid;

        }
        private bool IsPasswordExpired(int duration, string userId)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            var lstLastPasswordChangedDate = db.PasswordHistorys.Where(p => p.UserId == userId).OrderBy(p => p.Date).ToList();
            if (lstLastPasswordChangedDate != null && lstLastPasswordChangedDate.Count() > 0)
            {
                var LastPasswordChangedDate = lstLastPasswordChangedDate.LastOrDefault();
                if (LastPasswordChangedDate.Date.AddDays(duration) < DateTime.Now)
                    return true;
                else
                    return false;
            }
            return false;
        }
        private bool IsAutoRegistration(string userid)
        {
            var result = false;
            var Permission = User.permissions.Where(p => p.SelfRegistration != null && p.SelfRegistration.Value);
            foreach (var ent in Permission)
            {
                result = true;
                var EntityName = ent.EntityName;
                Type controller = Type.GetType("GeneratorBase.MVC.Controllers." + EntityName + "Controller");
                object objController = Activator.CreateInstance(controller, null);
                System.Reflection.MethodInfo mc = controller.GetMethod("IsAlreadyRegistred");
                object[] MethodParams = new object[] { userid, db };
                var result1 = mc.Invoke(objController, MethodParams);
                if (Convert.ToBoolean(result1))
                    return false;
            }

            return result;
        }
        public static string GetBaseUrl()
        {
            var request = System.Web.HttpContext.Current.Request;
            var appUrl = HttpRuntime.AppDomainAppVirtualPath;

            if (appUrl != "/")
                appUrl = appUrl + "/";

            var baseUrl = string.Format("{0}://{1}{2}", request.Url.Scheme, request.Url.Authority, appUrl);

            return baseUrl;
        }
        public string getfavoriteUrl()
        {
            string baseUri = "";
            Uri uri = new Uri(Request.Url.AbsoluteUri);
            string pathQuery = uri.PathAndQuery;
            string hostName = uri.ToString().Replace(pathQuery, "");
            var virtualurl = VirtualPathUtility.ToAbsolute("~/");
            if (virtualurl == "/" || string.IsNullOrEmpty(virtualurl))
                baseUri = pathQuery.ToString().Replace(hostName, "/");
            else
                baseUri = pathQuery.ToString().Replace(virtualurl, ""); ;
            return baseUri;
        }
    }
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class NoCacheAttribute : ActionFilterAttribute { public override void OnResultExecuting(ResultExecutingContext filterContext) { base.OnResultExecuting(filterContext); if (filterContext.IsChildAction) return; var response = filterContext.HttpContext.Response; var cache = response.Cache; cache.SetCacheability(HttpCacheability.NoCache); cache.SetExpires(DateTime.Today.AddDays(-1)); cache.SetMaxAge(TimeSpan.FromSeconds(0)); cache.SetNoStore(); cache.SetRevalidation(HttpCacheRevalidation.AllCaches); response.AppendHeader("Pragma", "no-cache"); } }
}