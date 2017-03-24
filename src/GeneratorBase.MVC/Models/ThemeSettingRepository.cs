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
    public class ThemeSettingRepository
    {
        private List<ThemeSettings> allThemes;
        private XDocument themeSettingsData;


        // constructor
        public ThemeSettingRepository()
        {
            try
            {
                themeSettingsData = XDocument.Load(HttpContext.Current.Server.MapPath("~/App_Data/ThemeSettings.xml"));
                allThemes = new List<ThemeSettings>();
                var Themes = from c in themeSettingsData.Descendants("Settings")
                             select new ThemeSettings(
                                  (long)c.Element("Id"),
                                  c.Element("Name").Value,
                                  c.Element("CssEditor").Value,
                                  (bool)c.Element("IsActive"),
                                  (bool)c.Element("IsDefault"),
                                  c.Element("DisplayValue").Value
                             );
                allThemes.AddRange(Themes.ToList());



            }
            catch
            {
            }
        }
        public int ThemeCount()
        {
            return allThemes.Count();
        }
        public List<ThemeSettings> GetThemeSettings()
        {
            return allThemes;
        }
        public ThemeSettings GetThemeSettingsById(long id)
        {
            return allThemes.Find(t => t.Id == id);
        }
        public long InsertThemeModel(ThemeSettings Themes)
        {
            if (ThemeCount() > 0)
                Themes.Id = (long)(from T in themeSettingsData.Descendants("Settings") orderby (long)T.Element("Id") descending select (long)T.Element("Id")).FirstOrDefault() + 1;
            else
                Themes.Id = 1;
            themeSettingsData.Root.Add(new XElement("Settings", new XElement("Id", Themes.Id),
                new XElement("Name", Themes.Name),
                new XElement("CssEditor", Themes.CssEditor),
                new XElement("IsActive", Themes.IsActive),
                new XElement("IsDefault", Themes.IsDefault),
                new XElement("DisplayValue", Themes.DisplayValue)));
            themeSettingsData.Save(HttpContext.Current.Server.MapPath("~/App_Data/ThemeSettings.xml"));
            return Themes.Id;
        }
        public void EditThemesModel(Theme Themes, string strCss)
        {
            try
            {
                XElement node = themeSettingsData.Root.Elements("Settings").Where(i => (long)i.Element("Id") == Themes.Id).FirstOrDefault();
                node.SetElementValue("Name", Themes.Name);
                node.SetElementValue("CssEditor", strCss);
                node.SetElementValue("IsActive", Themes.IsActive);
                node.SetElementValue("IsDefault", Themes.IsDefault);
                node.SetElementValue("DisplayValue", Themes.Name);
                themeSettingsData.Save(HttpContext.Current.Server.MapPath("~/App_Data/ThemeSettings.xml"));
            }
            catch
            {
            }
        }
        public void UpdateAll()
        {
            try
            {
                foreach (var element in themeSettingsData.Descendants("Settings"))
                {
                    element.SetElementValue("IsActive", false);
                }
                themeSettingsData.Save(HttpContext.Current.Server.MapPath("~/App_Data/ThemeSetting.xml"));
            }
            catch
            {
            }
        }
        public void UpdateSingle(long Id)
        {
            try
            {
                //themeSettingsData.Descendants("Settings").Where(t => (bool)t.Element("IsActive") == true)
                if (Id != 0)
                {
                    var ActiveElement = themeSettingsData.Descendants("Settings").Where(t => (bool)t.Element("IsActive") == true).FirstOrDefault();
                    if (ActiveElement != null)
                    {
                        ActiveElement.SetElementValue("IsActive", false);
                        themeSettingsData.Save(HttpContext.Current.Server.MapPath("~/App_Data/ThemeSettings.xml"));
                    }
                    var CurrentElement = themeSettingsData.Descendants("Settings").Where(t => (long)t.Element("Id") == Id).FirstOrDefault();
                    CurrentElement.SetElementValue("IsActive", true);
                    themeSettingsData.Save(HttpContext.Current.Server.MapPath("~/App_Data/ThemeSettings.xml"));
                }
                else
                {
                    if (ThemeCount() > 0)
                    {
                        var _defalutElement = themeSettingsData.Descendants("Settings").Where(t => (bool)t.Element("IsActive") == true).FirstOrDefault();
                        _defalutElement.SetElementValue("IsActive", false);
                        themeSettingsData.Save(HttpContext.Current.Server.MapPath("~/App_Data/ThemeSettings.xml"));
                    }
                }
            }
            catch
            {
            }
        }
        public void DeleteThemeModel(long id)
        {
            try
            {
                themeSettingsData.Root.Elements("Settings").Where(i => (long)i.Element("Id") == id).Remove();
                themeSettingsData.Save(HttpContext.Current.Server.MapPath("~/App_Data/ThemeSettings.xml"));
            }
            catch
            {
            }
        }
        //Mobile Theme
        public List<ThemeSetingMobile> GetThemeMobileSettings()
        {
            //mobile
            List<ThemeSetingMobile> allThemesMobile;
            XDocument themeSettingsMobileData;
            themeSettingsMobileData = XDocument.Load(HttpContext.Current.Server.MapPath("~/App_Data/MobileThemeSetting.xml"));
            allThemesMobile = new List<ThemeSetingMobile>();
            var ThemesMobile = from c in themeSettingsMobileData.Descendants("Settings")
                               select new ThemeSetingMobile(
                              (long)c.Element("Id"),
                              c.Element("Name").Value,
                              c.Element("CssName").Value,
                              (bool)c.Element("IsActive")
                         );
            allThemesMobile.AddRange(ThemesMobile.ToList());
            return allThemesMobile;
        }
        public void UpdateAllMobile(long Id)
        {
            try
            {
                XDocument MobileDataTheme = XDocument.Load(HttpContext.Current.Server.MapPath("~/App_Data/MobileThemeSetting.xml"));
                XElement node = MobileDataTheme.Root.Elements("Settings").Where(i => (long)i.Element("Id") == Id).FirstOrDefault();
                foreach (var element in MobileDataTheme.Descendants("Settings"))
                {
                    element.SetElementValue("IsActive", false);
                }
                node.SetElementValue("IsActive", true);
                MobileDataTheme.Save(HttpContext.Current.Server.MapPath("~/App_Data/MobileThemeSetting.xml"));
            }
            catch
            {
            }
        }
        //End Mobile Theme
    }
}