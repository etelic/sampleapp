using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;
namespace GeneratorBase.MVC.Models
{
    /// <summary>
    /// Testing Handler to restrict file downloading based on if user session doesn't exist.
    /// for instance my User session will be Session["User"]
    /// </summary>
    public class FileHandler : IHttpHandler, IReadOnlySessionState
    {
        bool IHttpHandler.IsReusable
        {
            get { return false; }
        }
        void IHttpHandler.ProcessRequest(HttpContext context)
        {
            if (!context.User.Identity.IsAuthenticated)
                return;
            var url = context.Request.RawUrl;
            if (!GetAuthorizedFiles((IUser)context.User).Contains(url.Substring(url.LastIndexOf("/") + 1)))
                return;
            //var filExtension = GettingExtension(context.Request.RawUrl);
            context.Response.ClearContent();
            context.Response.ClearHeaders();
            //context.Response.ContentType = MIMEType.Get(filExtension);
            context.Response.AddHeader("Content-Disposition", "attachment");
            context.Response.WriteFile(context.Request.RawUrl);
            context.Response.Flush();
        }
        public string GettingExtension(string rawUrl)
        {
            return rawUrl.Substring(rawUrl.LastIndexOf(".", System.StringComparison.Ordinal));
        }
        private List<string> GetAuthorizedFiles(IUser user)
        {
            List<string> list = new List<string>();
            ApplicationContext db = new ApplicationContext(user);
			list.AddRange(db.FileDocuments.Select(p => p.AttachDocument));	
            return list;
        }
    }
    /// <summary>
    /// Summary description for MIMEType,
    /// This code is the organized version of the answer posted by Sameul Neff
    /// on stack over flow Check here http://stackoverflow.com/a/4259616/1142645 
    /// </summary>
    public class MIMEType
    {
        #region MIME type list
        private static readonly Dictionary<String, String> MimeTypeDict = new Dictionary<String, String>()
		{
			{      "bin", "application/octet-stream" },
			{      "png", "image/png" },
            {      "pdf", "application/pdf" },
            {     "xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" },
            {     "docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document" }
		};
        #endregion
        #region Get
        /// <summary>
        /// Returns the mime type for the requested file extension. Returns
        /// the default application/octet-stream if the extension is not found.
        /// </summary>
        /// <param name="extension"></param>
        /// <returns></returns>
        public static String Get(String extension)
        {
            return Get(extension, MimeTypeDict["bin"]);
        }
        /// <summary>
        /// Returns the mime type for the requested file extension. Returns the
        /// specified defaultMimeType if the extension is not found.
        /// </summary>
        /// <param name="extension"></param>
        /// <param name="defaultMimeType"></param>
        /// <returns></returns>
        public static String Get(String extension, String defaultMimeType)
        {
            if (extension.StartsWith("."))
                extension = extension.Remove(0, 1);
            if (MimeTypeDict.ContainsKey(extension))
                return MimeTypeDict[extension];
            else
                return defaultMimeType;
        }
        #endregion Get
    }
}

