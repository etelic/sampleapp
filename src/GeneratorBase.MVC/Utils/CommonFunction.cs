using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.ComponentModel;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Configuration;
namespace GeneratorBase.MVC.Models
{
    public class CommonFunction
    {
        private static CommonFunction instance = null;
		private string reportPath { get; set; }
        private string reportUser { get; set; }
        private string reportPass { get; set; }
        private string reportFolder { get; set; }
        private string administratorRoles { get; set; }
        private string useActiveDirectory { get; set; }
        private string domainName { get; set; }
        private string needSharedUserSystem { get; set; }
        private string server { get; set; }
        private string appURL { get; set; }
        private string appName { get; set; }
        private string multipleRoleSelection { get; set; }
        private string enablePrototypingTool { get; set; }
		private string gpsenabled { get; set; }
        private CommonFunction()
        {
            var appsettinglist = (new ApplicationContext(new SystemUser())).AppSettings;
            reportPath = appsettinglist.FirstOrDefault(p => p.Key.ToLower() == "ReportPath".ToLower()).Value;
            reportUser = appsettinglist.FirstOrDefault(p => p.Key.ToLower() == "ReportUser".ToLower()).Value;
            reportPass = appsettinglist.FirstOrDefault(p => p.Key.ToLower() == "ReportPass".ToLower()).Value;
            reportFolder = appsettinglist.FirstOrDefault(p => p.Key.ToLower() == "ReportFolder".ToLower()).Value;
            administratorRoles = appsettinglist.FirstOrDefault(p => p.Key.ToLower() == "AdministratorRoles".ToLower()).Value;
            useActiveDirectory = appsettinglist.FirstOrDefault(p => p.Key.ToLower() == "UseActiveDirectory".ToLower()).Value;
            domainName = appsettinglist.FirstOrDefault(p => p.Key.ToLower() == "DomainName".ToLower()).Value;
            needSharedUserSystem = appsettinglist.FirstOrDefault(p => p.Key.ToLower() == "NeedSharedUserSystem".ToLower()).Value;
            server = appsettinglist.FirstOrDefault(p => p.Key.ToLower() == "server".ToLower()).Value;
            appURL = appsettinglist.FirstOrDefault(p => p.Key.ToLower() == "AppURL".ToLower()).Value;
            appName = appsettinglist.FirstOrDefault(p => p.Key.ToLower() == "AppName".ToLower()).Value;
            multipleRoleSelection = appsettinglist.FirstOrDefault(p => p.Key.ToLower() == "MultipleRoleSelection".ToLower()).Value;
            enablePrototypingTool = appsettinglist.FirstOrDefault(p => p.Key.ToLower() == "EnablePrototypingTool".ToLower()).Value;
			gpsenabled = appsettinglist.FirstOrDefault(p => p.Key.ToLower() == "GPSEnabled".ToLower()).Value;
        }
        public static CommonFunction Instance
        {
            get
            {
                if (CommonFunction.instance == null)
                    CommonFunction.instance = new CommonFunction();
                return CommonFunction.instance;
            }
            set { }
        }
        public static void ResetInstance()
        {
            CommonFunction.instance = null;
        }
        public string ReportPath()
        {
            return reportPath; 
        }
        public string ReportUser()
        {
            return reportUser;
        }
        public string ReportPass()
        {
            return reportPass;
        }
        public string ReportFolder()
        {
            if (string.IsNullOrEmpty(reportFolder) || reportFolder!= ConfigurationManager.AppSettings["ReportFolder"])
                reportFolder = ConfigurationManager.AppSettings["ReportFolder"];
            return reportFolder;
        }
        public string AdministratorRoles()
        {
            return administratorRoles;
        }
        public string UseActiveDirectory()
        {
            return useActiveDirectory;
        }
        public string DomainName()
        {
            return domainName;
        }
        public string NeedSharedUserSystem()
        {
			if (string.IsNullOrEmpty(needSharedUserSystem))
				needSharedUserSystem ="no";
            return needSharedUserSystem;
        }
        public string Server()
        {
            return server;
        }

		public string AppURL()
        {
            if (string.IsNullOrEmpty(appURL) || appURL!= ConfigurationManager.AppSettings["AppURL"])
                appURL = ConfigurationManager.AppSettings["AppURL"];
            return appURL;
        }
        public string AppName()
        {
            if (string.IsNullOrEmpty(appName) || appName != ConfigurationManager.AppSettings["AppName"])
                appName = ConfigurationManager.AppSettings["AppName"];
            return appName;
        }
        
        public string MultipleRoleSelection()
        {
            return multipleRoleSelection;
        }
        public string EnablePrototypingTool()
        {
            return enablePrototypingTool;
        }
		public string GPSEnabled()
        {
            if (string.IsNullOrEmpty(gpsenabled))
                gpsenabled = "false";
            return gpsenabled;
        }
    }
	public class ModelConversion
    {
        public static string GetDisplayNameOfEntity(string InternalName)
        {
            string result = InternalName;
            var entity = ModelReflector.Entities.FirstOrDefault(p => p.Name == InternalName);
            if (entity != null && !string.IsNullOrEmpty(entity.DisplayName))
                result = entity.DisplayName;
            return result;
        }
    }
}


