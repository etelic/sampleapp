using GeneratorBase.MVC.Models;
using System.Web;
using System.Xml.Linq;
using System.Linq;
using System.Collections.Generic;
using System.Data.Entity;
using System.Text;
using System;
namespace GeneratorBase.MVC.Models
{
    public class AdminSettingsRepository : IAdminSettingsRepository
    {
        private XDocument adminSettingsData;
        // constructor
        public AdminSettingsRepository()
        {
            adminSettingsData = XDocument.Load(HttpContext.Current.Server.MapPath("~/App_Data/AdminSettings.xml"));
        }
        public AdminSettings GetAdminSettings()
        {
            IEnumerable<AdminSettings> adminsettings = from admset in adminSettingsData.Descendants("Settings")
                                                       select new AdminSettings((string)admset.Element("DefaultRoleForNewUser"));
            return adminsettings.ToList()[0];
        }
        public string GetDefaultRoleForNewUser()
        {
            IEnumerable<AdminSettings> adminsettings = from admset in adminSettingsData.Descendants("Settings")
                                                       select new AdminSettings((string)admset.Element("DefaultRoleForNewUser"));
            return adminsettings.ToList()[0].DefaultRoleForNewUser;
        }
        public void EditAdminSettings(AdminSettings admSet)
        {
            XElement node = adminSettingsData.Root.Elements("Settings").FirstOrDefault();
            node.SetElementValue("DefaultRoleForNewUser", admSet.DefaultRoleForNewUser);
            adminSettingsData.Save(HttpContext.Current.Server.MapPath("~/App_Data/AdminSettings.xml"));
        }
    }
}