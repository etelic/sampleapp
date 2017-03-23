using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GeneratorBase.MVC.Models;
using PagedList;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.SqlServer;
using System.Linq.Expressions;
using RazorGenerator.Mvc;
using RazorEngine;
using System.Web.WebPages;
using System.Web.Mvc.Html;
using System.Text;
using System.IO;
using System.Data.OleDb;
using System.ComponentModel.DataAnnotations;
using System.Drawing.Imaging;
using System.Web.Helpers;
namespace GeneratorBase.MVC.Controllers
{
    public class ThemeController : Controller
    {
        ThemeSettingRepository _themeSettingsRepository = new ThemeSettingRepository();
        public ActionResult Index(string searchString, string sortBy, string isAsc, int? page, int? itemsPerPage, bool? RenderPartial, string BulkOperation)
        {
            if (string.IsNullOrEmpty(isAsc) && !string.IsNullOrEmpty(sortBy))
            {
                isAsc = "ASC";
            }
            ViewBag.isAsc = isAsc;
            ViewBag.CurrentSort = sortBy;
            if (searchString != null)
                page = null;
            ViewBag.CurrentFilter = searchString;
            var lstThemeBase = from s in _themeSettingsRepository.GetThemeSettings()
                               select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                lstThemeBase = searchRecords(lstThemeBase.AsQueryable(), searchString.ToUpper());
            }
            if (!String.IsNullOrEmpty(sortBy) && !String.IsNullOrEmpty(isAsc))
            {
                lstThemeBase = sortRecords(lstThemeBase.AsQueryable(), sortBy, isAsc);
            }
            else lstThemeBase = lstThemeBase.OrderByDescending(c => c.IsActive);
            int pageSize = 12;
            int pageNumber = (page ?? 1);
            ViewBag.Pages = page;
            if (itemsPerPage != null)
            {
                pageSize = (int)itemsPerPage;
                ViewBag.CurrentItemsPerPage = itemsPerPage;
            }
            pageSize = pageSize > 120 ? 120 : pageSize;
            var _Theme = lstThemeBase;
            if (!(RenderPartial == null ? false : RenderPartial.Value) && !Request.IsAjaxRequest())
                return View(_Theme.ToPagedList(pageNumber, pageSize));
            else
            {
                if (BulkOperation != null && (BulkOperation == "single" || BulkOperation == "multiple"))
                    return PartialView("BulkOperation", _Theme.OrderBy(q => q.DisplayValue).ToPagedList(pageNumber, pageSize));
                else
                    return PartialView("IndexPartial", _Theme.ToPagedList(pageNumber, pageSize));
            }
        }
        private IQueryable<ThemeSettings> searchRecords(IQueryable<ThemeSettings> lstTheme, string searchString)
        {
            searchString = searchString.Trim();
            lstTheme = lstTheme.Where(s => (!String.IsNullOrEmpty(s.Name) && s.Name.ToUpper().Contains(searchString)));
            return lstTheme;
        }
        private IQueryable<ThemeSettings> sortRecords(IQueryable<ThemeSettings> lstTheme, string sortBy, string isAsc)
        {
            string methodName = "";
            switch (isAsc.ToLower())
            {
                case "asc":
                    methodName = "OrderBy";
                    break;
                case "desc":
                    methodName = "OrderByDescending";
                    break;
            }
            ParameterExpression paramExpression = Expression.Parameter(typeof(ThemeSettings), "themebase");
            MemberExpression memExp = Expression.PropertyOrField(paramExpression, sortBy);
            LambdaExpression lambda = Expression.Lambda(memExp, paramExpression);
            return (IQueryable<ThemeSettings>)lstTheme.Provider.CreateQuery(
                Expression.Call(
                    typeof(Queryable),
                    methodName,
                    new Type[] { lstTheme.ElementType, lambda.Body.Type },
                    lstTheme.Expression,
                    lambda));
        }
        public string GetThemeCss()
        {
            string temp = "";
            // _themeSettingsRepository = new ThemeSettingRepository();
            var lsttheme = from s in _themeSettingsRepository.GetThemeSettings()
                           where (s.IsActive == true)
                           select s;
            if (lsttheme.Count() == 0)
                temp = System.IO.File.ReadAllText(Server.MapPath("~/Content/Site.css"));
            else
            {
                temp = lsttheme.ToList()[0].CssEditor;
            }
            Response.ContentType = "text/css";
            return Razor.Parse(temp.Replace("@media", "@@media").Replace("@font", "@@font"));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// 
        public ActionResult Create()
        {
            if (((CustomPrincipal)User).IsAdmin())
            {
                Theme theme = new Theme();
                theme = GetCssVariable(theme);
                return View(theme);
            }
            else return RedirectToAction("Index", "Home");
        }
        [HttpPost]
        public ActionResult Create(Theme theme)
        {
            if (((CustomPrincipal)User).IsAdmin())
            {
                if (ModelState.IsValid)
                {
                    string cssStr = EditCssVariable(theme);
                    if (theme.IsActive && _themeSettingsRepository.ThemeCount() > 0)
                    {
                        _themeSettingsRepository.UpdateAll();
                    }
                    ThemeSettings themeSetting = new ThemeSettings(0, theme.Name, cssStr, theme.IsActive, theme.IsDefault, theme.Name);
                    _themeSettingsRepository.InsertThemeModel(themeSetting);
                }
                return RedirectToAction("Create", "Theme");
            }
            else return RedirectToAction("Index", "Home");
        }
        public ActionResult Edit(bool? RenderPartial, long Id)
        {
            if (((CustomPrincipal)User).IsAdmin())
            {
                Theme theme = new Theme();
                theme = GetCssVariable(theme, Id);
                return View(theme);
            }
            else { return RedirectToAction("Index", "Home"); }
        }
        [HttpPost]
        public ActionResult Edit(Theme theme, bool? RenderPartial)
        {
            if (((CustomPrincipal)User).IsAdmin())
            {
                string eidtedTheme = EditCssVariable(theme);
                if (theme.IsActive)
                    _themeSettingsRepository.UpdateAll();
                _themeSettingsRepository.EditThemesModel(theme, eidtedTheme);
                return RedirectToAction("Create", "Theme");
            }
            else { return RedirectToAction("Index", "Home"); }
        }
        [HttpPost, ActionName("Delete")]
        public ActionResult Delete(long Id)
        {
            if (((CustomPrincipal)User).IsAdmin())
            {
                //_themeSettingsRepository = new ThemeSettingRepository();
                _themeSettingsRepository.DeleteThemeModel(Id);
                return RedirectToAction("Create", "Theme");
            }
            else { return RedirectToAction("Index", "Home"); }
        }
        //Css get Set methods
        public Theme GetCssVariable(Theme theme)
        {
            string CssFile = System.IO.File.ReadAllText(Server.MapPath("~/Content/Sitetemp.css"));
            theme.CssEditor = CssFile;
            Dictionary<string, string> varlist = new Dictionary<string, string>();
            if (System.IO.File.Exists(Server.MapPath("~/Content/Sitetemp.css")))
            {
                using (System.IO.StreamReader sr = System.IO.File.OpenText(Server.MapPath("~/Content/Sitetemp.css")))
                {
                    String input;
                    while ((input = sr.ReadLine()) != null)
                    {
                        if (input.Contains("var"))
                            varlist.Add(input.ToString().Trim().TrimStart("var".ToCharArray()).Split('=')[0], input.ToString().Trim().TrimStart("var".ToCharArray()).Split('=')[1]);
                    }
                }
            }
            theme.sidebarLinkColor = varlist.Where(p => p.Key.Trim() == "_sidebarLinkColor")
                        .First().Value.Replace("\"", "").Replace(";", "").Trim();
            theme.backgroundColorbody = varlist.Where(p => p.Key.Trim() == "_backgroundColorbody")
                        .First().Value.Replace("\"", "").Replace(";", "").Trim();
            theme.navlistBackground = varlist.Where(p => p.Key.Trim() == "_navlistBackground")
                        .First().Value.Replace("\"", "").Replace(";", "").Trim();
            theme.footerbgcolor = varlist.Where(p => p.Key.Trim() == "_footerbgcolor")
                       .First().Value.Replace("\"", "").Replace(";", "").Trim();
            theme.HeaderBackgroundColor = varlist.Where(p => p.Key.Trim() == "HeaderBackgroundColor")
                 .First().Value.Replace("\"", "").Replace(";", "").Trim();
            theme.HeaderAppLabel = varlist.Where(p => p.Key.Trim() == "HeaderAppLabel")
                 .First().Value.Replace("\"", "").Replace(";", "").Trim();
            theme.HeaderLinkReportBG = varlist.Where(p => p.Key.Trim() == "HeaderLinkReportBG")
                       .First().Value.Replace("\"", "").Replace(";", "").Trim();
            theme.HeaderLinkReportText = varlist.Where(p => p.Key.Trim() == "HeaderLinkReportText")
                 .First().Value.Replace("\"", "").Replace(";", "").Trim();
            theme.HeaderLinkReportTextHover = varlist.Where(p => p.Key.Trim() == "HeaderLinkReportTextHover")
                 .First().Value.Replace("\"", "").Replace(";", "").Trim();
            theme.HeaderLinkAdminBG = varlist.Where(p => p.Key.Trim() == "HeaderLinkAdminBG")
                       .First().Value.Replace("\"", "").Replace(";", "").Trim();
            theme.HeaderLinkAdminText = varlist.Where(p => p.Key.Trim() == "HeaderLinkAdminText")
                 .First().Value.Replace("\"", "").Replace(";", "").Trim();
            theme.HeaderLinkAdminTextHover = varlist.Where(p => p.Key.Trim() == "HeaderLinkAdminTextHover")
                 .First().Value.Replace("\"", "").Replace(";", "").Trim();
            theme.HeaderLinkUserBG = varlist.Where(p => p.Key.Trim() == "HeaderLinkUserBG")
                       .First().Value.Replace("\"", "").Replace(";", "").Trim();
            theme.HeaderLinkUserText = varlist.Where(p => p.Key.Trim() == "HeaderLinkUserText")
                 .First().Value.Replace("\"", "").Replace(";", "").Trim();
            theme.HeaderLinkUserTextHover = varlist.Where(p => p.Key.Trim() == "HeaderLinkUserTextHover")
                 .First().Value.Replace("\"", "").Replace(";", "").Trim();
            theme.DashboardBoxBG = varlist.Where(p => p.Key.Trim() == "DashboardBoxBG")
                .First().Value.Replace("\"", "").Replace(";", "").Trim();
            theme.DashboardBoxBorder = varlist.Where(p => p.Key.Trim() == "DashboardBoxBorder")
                 .First().Value.Replace("\"", "").Replace(";", "").Trim();
            theme.DashboardBoxHover = varlist.Where(p => p.Key.Trim() == "DashboardBoxHover")
                .First().Value.Replace("\"", "").Replace(";", "").Trim();
            theme.DashboardBoxLink = varlist.Where(p => p.Key.Trim() == "DashboardBoxLink")
                .First().Value.Replace("\"", "").Replace(";", "").Trim();
            theme.DashboardBoxLinkHover = varlist.Where(p => p.Key.Trim() == "DashboardBoxLinkHover")
                 .First().Value.Replace("\"", "").Replace(";", "").Trim();
            theme.sidebarLinkHoverBG = varlist.Where(p => p.Key.Trim() == "sidebarLinkHoverBG")
            .First().Value.Replace("\"", "").Replace(";", "").Trim();
            theme.sidebarLinkHoverColor = varlist.Where(p => p.Key.Trim() == "sidebarLinkHoverColor")
                 .First().Value.Replace("\"", "").Replace(";", "").Trim();
            theme.PageTitleBG = varlist.Where(p => p.Key.Trim() == "PageTitleBG")
            .First().Value.Replace("\"", "").Replace(";", "").Trim();
            theme.PageTitleBorder1 = varlist.Where(p => p.Key.Trim() == "PageTitleBorder1")
                 .First().Value.Replace("\"", "").Replace(";", "").Trim();
            theme.PanelHeadingBG = varlist.Where(p => p.Key.Trim() == "PanelHeadingBG")
                .First().Value.Replace("\"", "").Replace(";", "").Trim();
            theme.PanelHeadingForeColor1 = varlist.Where(p => p.Key.Trim() == "PanelHeadingForeColor1")
            .First().Value.Replace("\"", "").Replace(";", "").Trim();
            theme.PanelHeadingBorder = varlist.Where(p => p.Key.Trim() == "PanelHeadingBorder")
                 .First().Value.Replace("\"", "").Replace(";", "").Trim();
            theme.PageTitleForeColor = varlist.Where(p => p.Key.Trim() == "PageTitleForeColor")
                 .First().Value.Replace("\"", "").Replace(";", "").Trim();
            theme.BtnDefaultBG1 = varlist.Where(p => p.Key.Trim() == "BtnDefaultBG1")
           .First().Value.Replace("\"", "").Replace(";", "").Trim();
            theme.BtnDefaultForecolor = varlist.Where(p => p.Key.Trim() == "BtnDefaultForecolor")
                 .First().Value.Replace("\"", "").Replace(";", "").Trim();
            theme.BtnDefaultHoverBG = varlist.Where(p => p.Key.Trim() == "BtnDefaultHoverBG")
           .First().Value.Replace("\"", "").Replace(";", "").Trim();
            theme.BtnDefaultHoverForecolor = varlist.Where(p => p.Key.Trim() == "BtnDefaultHoverForecolor")
                 .First().Value.Replace("\"", "").Replace(";", "").Trim();
            theme.Borderbutton = varlist.Where(p => p.Key.Trim() == "Borderbutton")
           .First().Value.Replace("\"", "").Replace(";", "").Trim();
            theme.Borderbutton1 = varlist.Where(p => p.Key.Trim() == "Borderbutton1")
                 .First().Value.Replace("\"", "").Replace(";", "").Trim();
            theme.Btn2DefaultBG = varlist.Where(p => p.Key.Trim() == "Btn2DefaultBG")
           .First().Value.Replace("\"", "").Replace(";", "").Trim();
            theme.Btn2DefaultForecolor = varlist.Where(p => p.Key.Trim() == "Btn2DefaultForecolor")
                 .First().Value.Replace("\"", "").Replace(";", "").Trim();
            theme.Btn2DefaultHoverBG = varlist.Where(p => p.Key.Trim() == "Btn2DefaultHoverBG")
           .First().Value.Replace("\"", "").Replace(";", "").Trim();
            theme.Btn2DefaultHoverForecolor = varlist.Where(p => p.Key.Trim() == "Btn2DefaultHoverForecolor")
            .First().Value.Replace("\"", "").Replace(";", "").Trim();
            theme.Borderbutton2 = varlist.Where(p => p.Key.Trim() == "Borderbutton2")
           .First().Value.Replace("\"", "").Replace(";", "").Trim();
            theme.Borderbutton3 = varlist.Where(p => p.Key.Trim() == "Borderbutton3")
            .First().Value.Replace("\"", "").Replace(";", "").Trim();
            theme.TableHeaderBG = varlist.Where(p => p.Key.Trim() == "TableHeaderBG")
           .First().Value.Replace("\"", "").Replace(";", "").Trim();
            theme.TableForecolor = varlist.Where(p => p.Key.Trim() == "TableForecolor")
           .First().Value.Replace("\"", "").Replace(";", "").Trim();
            theme.TableBorder = varlist.Where(p => p.Key.Trim() == "TableBorder")
            .First().Value.Replace("\"", "").Replace(";", "").Trim();
            theme.TabsActiveforecolor = varlist.Where(p => p.Key.Trim() == "TabsActiveforecolor")
           .First().Value.Replace("\"", "").Replace(";", "").Trim();
            theme.TabsActiveBorder = varlist.Where(p => p.Key.Trim() == "TabsActiveBorder")
           .First().Value.Replace("\"", "").Replace(";", "").Trim();
            theme.TabsActiveTopborder = varlist.Where(p => p.Key.Trim() == "TabsActiveTopborder")
           .First().Value.Replace("\"", "").Replace(";", "").Trim();
            theme.TabsActiveBackground = varlist.Where(p => p.Key.Trim() == "TabsActiveBackground")
            .First().Value.Replace("\"", "").Replace(";", "").Trim();
            theme.TabsForecolor = varlist.Where(p => p.Key.Trim() == "TabsForecolor")
           .First().Value.Replace("\"", "").Replace(";", "").Trim();
            theme.TabsBackground = varlist.Where(p => p.Key.Trim() == "TabsBackground")
           .First().Value.Replace("\"", "").Replace(";", "").Trim();
            theme.TabsBorder = varlist.Where(p => p.Key.Trim() == "TabsBorder")
           .First().Value.Replace("\"", "").Replace(";", "").Trim();
            theme.TabsHeadingBG = varlist.Where(p => p.Key.Trim() == "TabsHeadingBG")
           .First().Value.Replace("\"", "").Replace(";", "").Trim();
            theme.TabsHeadingBorder = varlist.Where(p => p.Key.Trim() == "TabsHeadingBorder")
            .First().Value.Replace("\"", "").Replace(";", "").Trim();
            theme.TabsForecolorHover = varlist.Where(p => p.Key.Trim() == "TabsForecolorHover")
          .First().Value.Replace("\"", "").Replace(";", "").Trim();
            theme.TabsBackgroundHover = varlist.Where(p => p.Key.Trim() == "TabsBackgroundHover")
           .First().Value.Replace("\"", "").Replace(";", "").Trim();
            theme.TabsBorderHover = varlist.Where(p => p.Key.Trim() == "TabsBorderHover")
            .First().Value.Replace("\"", "").Replace(";", "").Trim();
            theme.SidebarActiveforcolor = varlist.Where(p => p.Key.Trim() == "SidebarActiveforcolor")
         .First().Value.Replace("\"", "").Replace(";", "").Trim();
            theme.SidebarActiveBG = varlist.Where(p => p.Key.Trim() == "SidebarActiveBG")
           .First().Value.Replace("\"", "").Replace(";", "").Trim();
            theme.SidebarHoverBorderLeft = varlist.Where(p => p.Key.Trim() == "SidebarHoverBorderLeft")
            .First().Value.Replace("\"", "").Replace(";", "").Trim();
            theme.FooterLinkcolor = varlist.Where(p => p.Key.Trim() == "FooterLinkcolor")
          .First().Value.Replace("\"", "").Replace(";", "").Trim();
            theme.FooterForecolor = varlist.Where(p => p.Key.Trim() == "FooterForecolor")
            .First().Value.Replace("\"", "").Replace(";", "").Trim();
            theme.rightwrapper = varlist.Where(p => p.Key.Trim() == "rightwrapper")
            .First().Value.Replace("\"", "").Replace(";", "").Trim();
            return theme;
        }
        public Theme GetCssVariable(Theme theme, long Id)
        {
            //var lsttheme = (from s in db.themes
            //                where s.Id == Id
            //                select s).ToList();
            //_themeSettingsRepository = new ThemeSettingRepository();
            var lsttheme = (from s in _themeSettingsRepository.GetThemeSettings()
                            where (s.Id == Id)
                            select s).ToList();
            theme.Id = Id;
            theme.Name = lsttheme[0].Name;
            string CssFile = lsttheme[0].CssEditor;
            theme.CssEditor = lsttheme[0].CssEditor;
            theme.IsActive = lsttheme[0].IsActive;
            Dictionary<string, string> varlist = new Dictionary<string, string>();
            if (System.IO.File.Exists(Server.MapPath("~/Content/Sitetemp.css")))
            {
                using (System.IO.StringReader sr = new StringReader(CssFile))
                {
                    String input;
                    while ((input = sr.ReadLine()) != null)
                    {
                        if (input.Contains("var"))
                            varlist.Add(input.ToString().Trim().TrimStart("var".ToCharArray()).Split('=')[0], input.ToString().Trim().TrimStart("var".ToCharArray()).Split('=')[1]);
                    }
                }
            }
            theme.sidebarLinkColor = varlist.Where(p => p.Key.Trim() == "_sidebarLinkColor")
                        .First().Value.Replace("\"", "").Replace(";", "").Trim();
            theme.backgroundColorbody = varlist.Where(p => p.Key.Trim() == "_backgroundColorbody")
                        .First().Value.Replace("\"", "").Replace(";", "").Trim();
            theme.navlistBackground = varlist.Where(p => p.Key.Trim() == "_navlistBackground")
                        .First().Value.Replace("\"", "").Replace(";", "").Trim();
            theme.footerbgcolor = varlist.Where(p => p.Key.Trim() == "_footerbgcolor")
                       .First().Value.Replace("\"", "").Replace(";", "").Trim();
            theme.HeaderBackgroundColor = varlist.Where(p => p.Key.Trim() == "HeaderBackgroundColor")
                 .First().Value.Replace("\"", "").Replace(";", "").Trim();
            theme.HeaderAppLabel = varlist.Where(p => p.Key.Trim() == "HeaderAppLabel")
                 .First().Value.Replace("\"", "").Replace(";", "").Trim();
            theme.HeaderLinkReportBG = varlist.Where(p => p.Key.Trim() == "HeaderLinkReportBG")
                       .First().Value.Replace("\"", "").Replace(";", "").Trim();
            theme.HeaderLinkReportText = varlist.Where(p => p.Key.Trim() == "HeaderLinkReportText")
                 .First().Value.Replace("\"", "").Replace(";", "").Trim();
            theme.HeaderLinkReportTextHover = varlist.Where(p => p.Key.Trim() == "HeaderLinkReportTextHover")
                 .First().Value.Replace("\"", "").Replace(";", "").Trim();
            theme.HeaderLinkAdminBG = varlist.Where(p => p.Key.Trim() == "HeaderLinkAdminBG")
                       .First().Value.Replace("\"", "").Replace(";", "").Trim();
            theme.HeaderLinkAdminText = varlist.Where(p => p.Key.Trim() == "HeaderLinkAdminText")
                 .First().Value.Replace("\"", "").Replace(";", "").Trim();
            theme.HeaderLinkAdminTextHover = varlist.Where(p => p.Key.Trim() == "HeaderLinkAdminTextHover")
                 .First().Value.Replace("\"", "").Replace(";", "").Trim();
            theme.HeaderLinkUserBG = varlist.Where(p => p.Key.Trim() == "HeaderLinkUserBG")
                       .First().Value.Replace("\"", "").Replace(";", "").Trim();
            theme.HeaderLinkUserText = varlist.Where(p => p.Key.Trim() == "HeaderLinkUserText")
                 .First().Value.Replace("\"", "").Replace(";", "").Trim();
            theme.HeaderLinkUserTextHover = varlist.Where(p => p.Key.Trim() == "HeaderLinkUserTextHover")
                 .First().Value.Replace("\"", "").Replace(";", "").Trim();
            theme.DashboardBoxBG = varlist.Where(p => p.Key.Trim() == "DashboardBoxBG")
                .First().Value.Replace("\"", "").Replace(";", "").Trim();
            theme.DashboardBoxBorder = varlist.Where(p => p.Key.Trim() == "DashboardBoxBorder")
                 .First().Value.Replace("\"", "").Replace(";", "").Trim();
            theme.DashboardBoxHover = varlist.Where(p => p.Key.Trim() == "DashboardBoxHover")
                .First().Value.Replace("\"", "").Replace(";", "").Trim();
            theme.DashboardBoxLink = varlist.Where(p => p.Key.Trim() == "DashboardBoxLink")
                .First().Value.Replace("\"", "").Replace(";", "").Trim();
            theme.DashboardBoxLinkHover = varlist.Where(p => p.Key.Trim() == "DashboardBoxLinkHover")
                 .First().Value.Replace("\"", "").Replace(";", "").Trim();
            theme.sidebarLinkHoverBG = varlist.Where(p => p.Key.Trim() == "sidebarLinkHoverBG")
            .First().Value.Replace("\"", "").Replace(";", "").Trim();
            theme.sidebarLinkHoverColor = varlist.Where(p => p.Key.Trim() == "sidebarLinkHoverColor")
                 .First().Value.Replace("\"", "").Replace(";", "").Trim();
            theme.PageTitleBG = varlist.Where(p => p.Key.Trim() == "PageTitleBG")
            .First().Value.Replace("\"", "").Replace(";", "").Trim();
            theme.PageTitleBorder1 = varlist.Where(p => p.Key.Trim() == "PageTitleBorder1")
                 .First().Value.Replace("\"", "").Replace(";", "").Trim();
            theme.PanelHeadingBG = varlist.Where(p => p.Key.Trim() == "PanelHeadingBG")
                .First().Value.Replace("\"", "").Replace(";", "").Trim();
            theme.PanelHeadingForeColor1 = varlist.Where(p => p.Key.Trim() == "PanelHeadingForeColor1")
            .First().Value.Replace("\"", "").Replace(";", "").Trim();
            theme.PanelHeadingBorder = varlist.Where(p => p.Key.Trim() == "PanelHeadingBorder")
                 .First().Value.Replace("\"", "").Replace(";", "").Trim();
            theme.PageTitleForeColor = varlist.Where(p => p.Key.Trim() == "PageTitleForeColor")
                 .First().Value.Replace("\"", "").Replace(";", "").Trim();
            theme.BtnDefaultBG1 = varlist.Where(p => p.Key.Trim() == "BtnDefaultBG1")
           .First().Value.Replace("\"", "").Replace(";", "").Trim();
            theme.BtnDefaultForecolor = varlist.Where(p => p.Key.Trim() == "BtnDefaultForecolor")
                 .First().Value.Replace("\"", "").Replace(";", "").Trim();
            theme.BtnDefaultHoverBG = varlist.Where(p => p.Key.Trim() == "BtnDefaultHoverBG")
           .First().Value.Replace("\"", "").Replace(";", "").Trim();
            theme.BtnDefaultHoverForecolor = varlist.Where(p => p.Key.Trim() == "BtnDefaultHoverForecolor")
                 .First().Value.Replace("\"", "").Replace(";", "").Trim();
            theme.Borderbutton = varlist.Where(p => p.Key.Trim() == "Borderbutton")
           .First().Value.Replace("\"", "").Replace(";", "").Trim();
            theme.Borderbutton1 = varlist.Where(p => p.Key.Trim() == "Borderbutton1")
                 .First().Value.Replace("\"", "").Replace(";", "").Trim();
            theme.Btn2DefaultBG = varlist.Where(p => p.Key.Trim() == "Btn2DefaultBG")
           .First().Value.Replace("\"", "").Replace(";", "").Trim();
            theme.Btn2DefaultForecolor = varlist.Where(p => p.Key.Trim() == "Btn2DefaultForecolor")
                 .First().Value.Replace("\"", "").Replace(";", "").Trim();
            theme.Btn2DefaultHoverBG = varlist.Where(p => p.Key.Trim() == "Btn2DefaultHoverBG")
           .First().Value.Replace("\"", "").Replace(";", "").Trim();
            theme.Btn2DefaultHoverForecolor = varlist.Where(p => p.Key.Trim() == "Btn2DefaultHoverForecolor")
            .First().Value.Replace("\"", "").Replace(";", "").Trim();
            theme.Borderbutton2 = varlist.Where(p => p.Key.Trim() == "Borderbutton2")
           .First().Value.Replace("\"", "").Replace(";", "").Trim();
            theme.Borderbutton3 = varlist.Where(p => p.Key.Trim() == "Borderbutton3")
            .First().Value.Replace("\"", "").Replace(";", "").Trim();
            theme.TableHeaderBG = varlist.Where(p => p.Key.Trim() == "TableHeaderBG")
           .First().Value.Replace("\"", "").Replace(";", "").Trim();
            theme.TableForecolor = varlist.Where(p => p.Key.Trim() == "TableForecolor")
           .First().Value.Replace("\"", "").Replace(";", "").Trim();
            theme.TableBorder = varlist.Where(p => p.Key.Trim() == "TableBorder")
            .First().Value.Replace("\"", "").Replace(";", "").Trim();
            theme.TabsActiveforecolor = varlist.Where(p => p.Key.Trim() == "TabsActiveforecolor")
           .First().Value.Replace("\"", "").Replace(";", "").Trim();
            theme.TabsActiveBorder = varlist.Where(p => p.Key.Trim() == "TabsActiveBorder")
           .First().Value.Replace("\"", "").Replace(";", "").Trim();
            theme.TabsActiveTopborder = varlist.Where(p => p.Key.Trim() == "TabsActiveTopborder")
           .First().Value.Replace("\"", "").Replace(";", "").Trim();
            theme.TabsActiveBackground = varlist.Where(p => p.Key.Trim() == "TabsActiveBackground")
            .First().Value.Replace("\"", "").Replace(";", "").Trim();
            theme.TabsForecolor = varlist.Where(p => p.Key.Trim() == "TabsForecolor")
           .First().Value.Replace("\"", "").Replace(";", "").Trim();
            theme.TabsBackground = varlist.Where(p => p.Key.Trim() == "TabsBackground")
           .First().Value.Replace("\"", "").Replace(";", "").Trim();
            theme.TabsBorder = varlist.Where(p => p.Key.Trim() == "TabsBorder")
           .First().Value.Replace("\"", "").Replace(";", "").Trim();
            theme.TabsHeadingBG = varlist.Where(p => p.Key.Trim() == "TabsHeadingBG")
           .First().Value.Replace("\"", "").Replace(";", "").Trim();
            theme.TabsHeadingBorder = varlist.Where(p => p.Key.Trim() == "TabsHeadingBorder")
            .First().Value.Replace("\"", "").Replace(";", "").Trim();
            theme.TabsForecolorHover = varlist.Where(p => p.Key.Trim() == "TabsForecolorHover")
          .First().Value.Replace("\"", "").Replace(";", "").Trim();
            theme.TabsBackgroundHover = varlist.Where(p => p.Key.Trim() == "TabsBackgroundHover")
           .First().Value.Replace("\"", "").Replace(";", "").Trim();
            theme.TabsBorderHover = varlist.Where(p => p.Key.Trim() == "TabsBorderHover")
            .First().Value.Replace("\"", "").Replace(";", "").Trim();
            theme.SidebarActiveforcolor = varlist.Where(p => p.Key.Trim() == "SidebarActiveforcolor")
         .First().Value.Replace("\"", "").Replace(";", "").Trim();
            theme.SidebarActiveBG = varlist.Where(p => p.Key.Trim() == "SidebarActiveBG")
           .First().Value.Replace("\"", "").Replace(";", "").Trim();
            theme.SidebarHoverBorderLeft = varlist.Where(p => p.Key.Trim() == "SidebarHoverBorderLeft")
            .First().Value.Replace("\"", "").Replace(";", "").Trim();
            theme.FooterLinkcolor = varlist.Where(p => p.Key.Trim() == "FooterLinkcolor")
          .First().Value.Replace("\"", "").Replace(";", "").Trim();
            theme.FooterForecolor = varlist.Where(p => p.Key.Trim() == "FooterForecolor")
            .First().Value.Replace("\"", "").Replace(";", "").Trim();
            theme.rightwrapper = varlist.Where(p => p.Key.Trim() == "rightwrapper")
            .First().Value.Replace("\"", "").Replace(";", "").Trim();
            return theme;
        }
        public string EditCssVariable(Theme theme)
        {
            string temp = System.IO.File.ReadAllText(Server.MapPath("~/Content/Sitetemp.css")); ;
            //temp = cssstring;
            temp = temp.Replace("_sidebarLinkColor = \"#ffffff\"", "_sidebarLinkColor = \"" + theme.sidebarLinkColor + "\"")
            .Replace("_backgroundColorbody = \"#62a8d1\"", "_backgroundColorbody = \"" + theme.backgroundColorbody + "\"")
            .Replace("_navlistBackground = \"#62a8d1\"", "_navlistBackground = \"" + theme.navlistBackground + "\"")
            .Replace("_footerbgcolor = \"#000000\"", "_footerbgcolor = \"" + theme.footerbgcolor + "\"")
            .Replace("HeaderBackgroundColor = \"#000000\"", "HeaderBackgroundColor = \"" + theme.HeaderBackgroundColor + "\"")
            .Replace("HeaderAppLabel = \"#ffffff\"", "HeaderAppLabel = \"" + theme.HeaderAppLabel + "\"")
            .Replace("HeaderLinkReportBG = \"#2e8965\"", "HeaderLinkReportBG = \"" + theme.HeaderLinkReportBG + "\"")
            .Replace("HeaderLinkReportText = \"#ffffff\"", "HeaderLinkReportText = \"" + theme.HeaderLinkReportText + "\"")
            .Replace("HeaderLinkReportTextHover = \"#2e8965\"", "HeaderLinkReportTextHover = \"" + theme.HeaderLinkReportTextHover + "\"")
            .Replace("HeaderLinkAdminBG = \"#892e65\"", "HeaderLinkAdminBG = \"" + theme.HeaderLinkAdminBG + "\"")
            .Replace("HeaderLinkAdminText = \"#ffffff\"", "HeaderLinkAdminText = \"" + theme.HeaderLinkAdminText + "\"")
            .Replace("HeaderLinkAdminTextHover = \"#892e65\"", "HeaderLinkAdminTextHover = \"" + theme.HeaderLinkAdminTextHover + "\"")
            .Replace("HeaderLinkUserBG = \"#62a8d1\"", "HeaderLinkUserBG = \"" + theme.HeaderLinkUserBG + "\"")
            .Replace("HeaderLinkUserText = \"#ffffff\"", "HeaderLinkUserText = \"" + theme.HeaderLinkUserText + "\"")
            .Replace("HeaderLinkUserTextHover = \"#62a8d1\"", "HeaderLinkUserTextHover = \"" + theme.HeaderLinkUserTextHover + "\"")
            .Replace("DashboardBoxBG = \"#ffffff\"", "DashboardBoxBG = \"" + theme.DashboardBoxBG + "\"")
            .Replace("DashboardBoxBorder = \"#d8d8d8\"", "DashboardBoxBorder = \"" + theme.DashboardBoxBorder + "\"")
            .Replace("DashboardBoxHover = \"#eeeeee\"", "DashboardBoxHover = \"" + theme.DashboardBoxHover + "\"")
            .Replace("DashboardBoxLink = \"#428bca\"", "DashboardBoxLink = \"" + theme.DashboardBoxLink + "\"")
            .Replace("DashboardBoxLinkHover = \"#777777\"", "DashboardBoxLinkHover = \"" + theme.DashboardBoxLinkHover + "\"")
            .Replace("sidebarLinkHoverBG = \"#ffffff\"", "sidebarLinkHoverBG = \"" + theme.sidebarLinkHoverBG + "\"")
            .Replace("sidebarLinkHoverColor = \"#1963aa\"", "sidebarLinkHoverColor = \"" + theme.sidebarLinkHoverColor + "\"")
            .Replace("PageTitleBG = \"#edf5fa\"", "PageTitleBG = \"" + theme.PageTitleBG + "\"")
            .Replace("PageTitleBorder1 = \"#c3ddec\"", "PageTitleBorder1 = \"" + theme.PageTitleBorder1 + "\"")
            .Replace("PanelHeadingBG = \"#edf5fa\"", "PanelHeadingBG = \"" + theme.PanelHeadingBG + "\"")
            .Replace("PanelHeadingForeColor1 = \"#3784b1\"", "PanelHeadingForeColor1 = \"" + theme.PanelHeadingForeColor1 + "\"")
            .Replace("PanelHeadingBorder = \"#c3ddec\"", "PanelHeadingBorder = \"" + theme.PanelHeadingBorder + "\"")
            .Replace("PageTitleForeColor = \"#333333\"", "PageTitleForeColor = \"" + theme.PageTitleForeColor + "\"")
            .Replace("BtnDefaultBG1 = \"#edf5fa\"", "BtnDefaultBG1 = \"" + theme.BtnDefaultBG1 + "\"")
            .Replace("BtnDefaultForecolor = \"#3784b1\"", "BtnDefaultForecolor = \"" + theme.BtnDefaultForecolor + "\"")
            .Replace("BtnDefaultHoverBG = \"#e6e6e6\"", "BtnDefaultHoverBG = \"" + theme.BtnDefaultHoverBG + "\"")
            .Replace("BtnDefaultHoverForecolor = \"#4d4d4d\"", "BtnDefaultHoverForecolor = \"" + theme.BtnDefaultHoverForecolor + "\"")
            .Replace("Borderbutton = \"#c3ddec\"", "Borderbutton = \"" + theme.Borderbutton + "\"")
            .Replace("Borderbutton1 = \"#dadada\"", "Borderbutton1 = \"" + theme.Borderbutton1 + "\"")
            .Replace("Btn2DefaultBG = \"#428bca\"", "Btn2DefaultBG = \"" + theme.Btn2DefaultBG + "\"")
            .Replace("Btn2DefaultForecolor = \"#ffffff\"", "Btn2DefaultForecolor = \"" + theme.Btn2DefaultForecolor + "\"")
            .Replace("Btn2DefaultHoverBG = \"#3276b1\"", "Btn2DefaultHoverBG = \"" + theme.Btn2DefaultHoverBG + "\"")
            .Replace("Btn2DefaultHoverForecolor = \"#ffffff\"", "Btn2DefaultHoverForecolor = \"" + theme.Btn2DefaultHoverForecolor + "\"")
            .Replace("Borderbutton2 = \"#357ebd\"", "Borderbutton2 = \"" + theme.Borderbutton2 + "\"")
            .Replace("Borderbutton3 = \"#285e8e\"", "Borderbutton3 = \"" + theme.Borderbutton3 + "\"")
            .Replace("TableHeaderBG = \"#edf5fa\"", "TableHeaderBG = \"" + theme.TableHeaderBG + "\"")
            .Replace("TableForecolor = \"#3784b1\"", "TableForecolor = \"" + theme.TableForecolor + "\"")
            .Replace("TableBorder = \"#c3ddec\"", "TableBorder = \"" + theme.TableBorder + "\"")
            .Replace("TabsActiveforecolor = \"#5b98c2\"", "TabsActiveforecolor = \"" + theme.TabsActiveforecolor + "\"")
            .Replace("TabsActiveBorder = \"#c5d0dc\"", "TabsActiveBorder = \"" + theme.TabsActiveBorder + "\"")
            .Replace("TabsActiveTopborder = \"#4c8fbd\"", "TabsActiveTopborder = \"" + theme.TabsActiveTopborder + "\"")
            .Replace("TabsActiveBackground = \"#ffffff\"", "TabsActiveBackground = \"" + theme.TabsActiveBackground + "\"")
            .Replace("TabsForecolor = \"#999999\"", "TabsForecolor = \"" + theme.TabsForecolor + "\"")
            .Replace("TabsBackground = \"#f9f9f9\"", "TabsBackground = \"" + theme.TabsBackground + "\"")
            .Replace("TabsBorder = \"#c5d0dc\"", "TabsBorder = \"" + theme.TabsBorder + "\"")
            .Replace("TabsHeadingBG = \"#edf5fa\"", "TabsHeadingBG = \"" + theme.TabsHeadingBG + "\"")
            .Replace("TabsHeadingBorder = \"#c5d0dc\"", "TabsHeadingBorder = \"" + theme.TabsHeadingBorder + "\"")
            .Replace("TabsForecolorHover = \"#4c8fbd\"", "TabsForecolorHover = \"" + theme.TabsForecolorHover + "\"")
            .Replace("TabsBackgroundHover = \"#ffffff\"", "TabsBackgroundHover = \"" + theme.TabsBackgroundHover + "\"")
            .Replace("TabsBorderHover = \"#c5d0dc\"", "TabsBorderHover = \"" + theme.TabsBorderHover + "\"")
            .Replace("SidebarActiveforcolor = \"#2b7dbc\"", "SidebarActiveforcolor = \"" + theme.SidebarActiveforcolor + "\"")
            .Replace("SidebarActiveBG = \"#ffffff\"", "SidebarActiveBG = \"" + theme.SidebarActiveBG + "\"")
            .Replace("SidebarHoverBorderLeft = \"#3382af\"", "SidebarHoverBorderLeft = \"" + theme.SidebarHoverBorderLeft + "\"")
            .Replace("FooterLinkcolor = \"#999999\"", "FooterLinkcolor = \"" + theme.FooterLinkcolor + "\"")
            .Replace("FooterForecolor = \"#2a6496\"", "FooterForecolor = \"" + theme.FooterForecolor + "\"")
            .Replace("rightwrapper = \"#ffffff\"", "rightwrapper = \"" + theme.rightwrapper + "\"");
            return temp;
        }
        public ActionResult SetCurrentTheme(long Id)
        {
            if (((CustomPrincipal)User).IsAdmin())
            {
                //_themeSettingsRepository = new ThemeSettingRepository();
                _themeSettingsRepository.UpdateSingle(Id);
                return RedirectToAction("Create", "Theme");
            }
            else return RedirectToAction("Index", "Home");
        }
        //

        //Mobile Theme
        public void EditMobile(long Id)
        {
            _themeSettingsRepository.UpdateAllMobile(Id);

        }
        public string GetThemeCssForMobile()
        {
            string temp = "";
            // _themeSettingsRepository = new ThemeSettingRepository();
            var lstthemeMobile = from s in _themeSettingsRepository.GetThemeMobileSettings()
                                 where (s.IsActive == true)
                                 select s;
            if (lstthemeMobile.Count() == 0)
                temp = "red";
            else
            {
                temp = lstthemeMobile.ToList()[0].CssName;
            }
            return temp;
            //Response.ContentType = "text/css";
            // return Razor.Parse(temp.Replace("@media", "@@media").Replace("@font", "@@font"));
        }
        //end Mobile Theme
    }
}