using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.ComponentModel;
using Microsoft.AspNet.Identity.EntityFramework;
namespace GeneratorBase.MVC.Models
{
    public class Theme
    {
        public Theme()
        {
            this.Id = 0;
            this.IsActive = false;
            this.IsDefault = false;
            this.Name = "";
            this.sidebarLinkColor = "#ffffff";
            this.backgroundColorbody = "#62a8d1";
            this.navlistBackground = "#62a8d1";
            this.footerbgcolor = "#000000";
            this.HeaderBackgroundColor = "#000000";
            this.HeaderAppLabel = "#ffffff";
            this.HeaderLinkReportBG = "#2e8965";
            this.HeaderLinkReportText = "#fffffff";
            this.HeaderLinkReportTextHover = "#2e8965";
            this.DashboardBoxBG = "#ffffff";
            this.DashboardBoxBorder = "#d8d8d8";
            this.DashboardBoxHover = "#eeeeee";
            this.DashboardBoxLink = "#428bca";
            this.DashboardBoxLinkHover = "#777777";
            this.sidebarLinkHoverBG = "#ffffff";
            this.sidebarLinkHoverColor = "#1963aa";
            this.PageTitleBG = "#edf5fa";
            this.PageTitleBorder1 = "#c3ddec";
            this.PanelHeadingBG = "#edf5fa";
            this.PanelHeadingForeColor1 = "#3784b1";
            this.PanelHeadingBorder = "#c3ddec";
            this.PageTitleForeColor = "#333333";
            this.BtnDefaultBG1 = "#edf5fa";
            this.BtnDefaultForecolor = "#3784b1";
            this.BtnDefaultHoverBG = "#e6e6e6";
            this.BtnDefaultHoverForecolor = "#4d4d4d";
            this.Borderbutton = "#c3ddec";
            this.Borderbutton1 = "#dadada";
            this.Btn2DefaultBG = "#428bca";
            this.Btn2DefaultForecolor = "#ffffff";
            this.Btn2DefaultHoverBG = "#3276b1";
            this.Btn2DefaultHoverForecolor = "#ffffff";
            this.Borderbutton2 = "#357ebd";
            this.Borderbutton3 = "#285e8e";
            this.TableHeaderBG = "#edf5fa";
            this.TableForecolor = "#3784b1";
            this.TableBorder = "#c3ddec";
            this.TabsActiveforecolor = "#5b98c2";
            this.TabsActiveBorder = "#c5d0dc";
            this.TabsActiveTopborder = "#4c8fbd";
            this.TabsActiveBackground = "#ffffff";
            this.TabsForecolor = "#999999";
            this.TabsBackground = "#f9f9f9";
            this.TabsBorder = "#c5d0dc";
            this.TabsHeadingBG = "#edf5fa";
            this.TabsHeadingBorder = "#c5d0dc";
            this.TabsForecolorHover = "#4c8fbd";
            this.TabsBackgroundHover = "#ffffff";
            this.TabsBorderHover = "#c5d0dc";
            this.SidebarActiveforcolor = "#2b7dbc";
            this.SidebarActiveBG = "#ffffff";
            this.SidebarHoverBorderLeft = "#3382af";
            this.FooterLinkcolor = "#999999";
            this.FooterForecolor = "#2a6496";
            this.rightwrapper = "#ffffff";
        }
        public Theme(int id, string name, string CssEditor, bool IsActive,bool IsDefault, string sidebarLinkColor, string backgroundColorbody, string navlistBackground,
            string footerbgcolor, string HeaderBackgroundColor, string HeaderAppLabel,
            string HeaderLinkReportBG, string HeaderLinkReportText, string HeaderLinkReportTextHover,
            string HeaderLinkAdminBG, string HeaderLinkAdminText, string HeaderLinkAdminTextHover,
            string HeaderLinkUserBG, string HeaderLinkUserText, string HeaderLinkUserTextHover,
            string DashboardBoxBG, string DashboardBoxBorder, string DashboardBoxHover, string DashboardBoxLink, string DashboardBoxLinkHover, string sidebarLinkHoverBG, string sidebarLinkHoverColor,
            string PageTitleBG, string PageTitleBorder1, string PanelHeadingBG, string PanelHeadingForeColor1, string PanelHeadingBorder, string PageTitleForeColor, string BtnDefaultBG1, string BtnDefaultForecolor,
            string BtnDefaultHoverBG, string BtnDefaultHoverForecolor, string Borderbutton, string Borderbutton1,
            string Btn2DefaultBG, string Btn2DefaultForecolor, string Btn2DefaultHoverBG, string Btn2DefaultHoverForecolor, string Borderbutton2, string Borderbutton3,
            string TableHeaderBG, string TableForecolor, string TableBorder, string TabsActiveforecolor, string TabsActiveBorder, string TabsActiveTopborder,
            string TabsActiveBackground, string TabsForecolor, string TabsBackground, string TabsBorder, string TabsHeadingBG, string TabsHeadingBorder,
            string TabsForecolorHover, string TabsBackgroundHover, string TabsBorderHover, string SidebarActiveforcolor, string SidebarActiveBG, string SidebarHoverBorderLeft,
            string FooterLinkcolor, string FooterForecolor, string rightwrapper)
        {
            this.Id = id;
            this.Name = name;
            this.CssEditor = CssEditor;
            this.sidebarLinkColor = sidebarLinkColor;
            this.backgroundColorbody = backgroundColorbody;
            this.navlistBackground = navlistBackground;
            this.footerbgcolor = footerbgcolor;
            this.HeaderBackgroundColor = HeaderBackgroundColor;
            this.HeaderAppLabel = HeaderAppLabel;
            this.HeaderLinkReportBG = HeaderLinkReportBG;
            this.HeaderLinkReportText = HeaderLinkReportText;
            this.HeaderLinkReportTextHover = HeaderLinkReportTextHover;
            this.HeaderLinkAdminBG = HeaderLinkAdminBG;
            this.HeaderLinkAdminText = HeaderLinkAdminText;
            this.HeaderLinkAdminTextHover = HeaderLinkAdminTextHover;
            this.HeaderLinkUserBG = HeaderLinkUserBG;
            this.HeaderLinkUserText = HeaderLinkUserText;
            this.HeaderLinkUserTextHover = HeaderLinkUserTextHover;
            this.DashboardBoxBG = DashboardBoxBG;
            this.DashboardBoxBorder = DashboardBoxBorder;
            this.DashboardBoxHover = DashboardBoxHover;
            this.DashboardBoxLink = DashboardBoxLink;
            this.DashboardBoxLinkHover = DashboardBoxLinkHover;
            this.sidebarLinkHoverBG = sidebarLinkHoverBG;
            this.sidebarLinkHoverColor = sidebarLinkHoverColor;
            this.PageTitleBG = PageTitleBG;
            this.PageTitleBorder1 = PageTitleBorder1;
            this.PanelHeadingBG = PanelHeadingBG;
            this.PanelHeadingForeColor1 = PanelHeadingForeColor1;
            this.PanelHeadingBorder = PanelHeadingBorder;
            this.PageTitleForeColor = PageTitleForeColor;
            this.BtnDefaultBG1 = BtnDefaultBG1;
            this.BtnDefaultForecolor = BtnDefaultForecolor;
            this.BtnDefaultHoverBG = BtnDefaultHoverBG;
            this.BtnDefaultHoverForecolor = BtnDefaultHoverForecolor;
            this.Borderbutton = Borderbutton;
            this.Borderbutton1 = Borderbutton1;
            this.Btn2DefaultBG = Btn2DefaultBG;
            this.Btn2DefaultForecolor = Btn2DefaultForecolor;
            this.Btn2DefaultHoverBG = Btn2DefaultHoverBG;
            this.Btn2DefaultHoverForecolor = Btn2DefaultHoverForecolor;
            this.Borderbutton2 = Borderbutton2;
            this.Borderbutton3 = Borderbutton3;
            this.TableHeaderBG = TableHeaderBG;
            this.TableForecolor = TableForecolor;
            this.TableBorder = TableBorder;
            this.TabsActiveforecolor = TabsActiveforecolor;
            this.TabsActiveBorder = TabsActiveBorder;
            this.TabsActiveTopborder = TabsActiveTopborder;
            this.TabsActiveBackground = TabsActiveBackground;
            this.TabsForecolor = TabsForecolor;
            this.TabsBackground = TabsBackground;
            this.TabsBorder = TabsBorder;
            this.TabsHeadingBG = TabsHeadingBG;
            this.TabsHeadingBorder = TabsHeadingBorder;
            this.TabsForecolorHover = TabsForecolorHover;
            this.TabsBackgroundHover = TabsBackgroundHover;
            this.TabsBorderHover = TabsBorderHover;
            this.SidebarActiveforcolor = SidebarActiveforcolor;
            this.SidebarActiveBG = SidebarActiveBG;
            this.SidebarHoverBorderLeft = SidebarHoverBorderLeft;
            this.FooterLinkcolor = FooterLinkcolor;
            this.FooterForecolor = FooterForecolor;
            this.rightwrapper = rightwrapper;
        }
        //[RegularExpression(@"^\d{3}\-?\d{3}\-?\d{4}$", ErrorMessage = "Invalid PhoneNo")]
        [DisplayName("Id")]
        public long Id { get; set; }
        [DisplayName("Theme Name")]
        [Required]
        public string Name { get; set; }
        [DisplayName("CSS Editor")]
        public string CssEditor { get; set; }
        [DisplayName("IsActive")]
        public bool IsActive { get; set; }

        [DisplayName("IsDefault")]
        public bool IsDefault { get; set; }

        [DisplayName("Side Bar link")]
        [Required]
        public string sidebarLinkColor { get; set; }
        [DisplayName("Back Ground Color")]
        [Required]
        public string backgroundColorbody { get; set; }
        [DisplayName("Side Bar Background")]
        [Required]
        public string navlistBackground { get; set; }
        [DisplayName("Footer Color")]
        [Required]
        public string footerbgcolor { get; set; }
        [DisplayName("Header Background Color")]
        [Required]
        public string HeaderBackgroundColor { get; set; }
        [DisplayName("HeaderAppLabel")]
        [Required]
        public string HeaderAppLabel { get; set; }
        [DisplayName("HeaderLinkReportBG")]
        [Required]
        public string HeaderLinkReportBG { get; set; }
        [DisplayName("HeaderLinkReportText")]
        [Required]
        public string HeaderLinkReportText { get; set; }
        [DisplayName("HeaderLinkReportTextHover")]
        [Required]
        public string HeaderLinkReportTextHover { get; set; }
        [DisplayName("HeaderLinkAdminBG")]
        [Required]
        public string HeaderLinkAdminBG { get; set; }
        [DisplayName("HeaderLinkAdminText")]
        [Required]
        public string HeaderLinkAdminText { get; set; }
        [DisplayName("HeaderLinkAdminTextHover")]
        [Required]
        public string HeaderLinkAdminTextHover { get; set; }
        [DisplayName("HeaderLinkUserBG")]
        [Required]
        public string HeaderLinkUserBG { get; set; }
        [DisplayName("HeaderLinkUserText")]
        [Required]
        public string HeaderLinkUserText { get; set; }
        [DisplayName("HeaderLinkUserTextHover")]
        [Required]
        public string HeaderLinkUserTextHover { get; set; }
        [DisplayName("DashboardBoxBG")]
        [Required]
        public string DashboardBoxBG { get; set; }
        [DisplayName("DashboardBoxBorder")]
        [Required]
        public string DashboardBoxBorder { get; set; }
        [DisplayName("DashboardBoxHover")]
        [Required]
        public string DashboardBoxHover { get; set; }
        [DisplayName("DashboardBoxLink")]
        [Required]
        public string DashboardBoxLink { get; set; }
        [DisplayName("DashboardBoxLinkHover")]
        [Required]
        public string DashboardBoxLinkHover { get; set; }
        [DisplayName("sidebarLinkHoverBG")]
        [Required]
        public string sidebarLinkHoverBG { get; set; }
        [DisplayName("sidebarLinkHoverColor")]
        [Required]
        public string sidebarLinkHoverColor { get; set; }
        [DisplayName("PageTitleBG")]
        [Required]
        public string PageTitleBG { get; set; }
        [DisplayName("PageTitleBorder1")]
        [Required]
        public string PageTitleBorder1 { get; set; }
        [DisplayName("PanelHeadingBG")]
        [Required]
        public string PanelHeadingBG { get; set; }
        [DisplayName("PanelHeadingForeColor1")]
        [Required]
        public string PanelHeadingForeColor1 { get; set; }
        [DisplayName("PanelHeadingBorder")]
        [Required]
        public string PanelHeadingBorder { get; set; }
        [DisplayName("PageTitleForeColor")]
        [Required]
        public string PageTitleForeColor { get; set; }
        [DisplayName("BtnDefaultBG1")]
        [Required]
        public string BtnDefaultBG1 { get; set; }
        [DisplayName("BtnDefaultForecolor")]
        [Required]
        public string BtnDefaultForecolor { get; set; }
        [DisplayName("BtnDefaultHoverBG")]
        [Required]
        public string BtnDefaultHoverBG { get; set; }
        [DisplayName("BtnDefaultHoverForecolor")]
        [Required]
        public string BtnDefaultHoverForecolor { get; set; }
        [DisplayName("Borderbutton")]
        [Required]
        public string Borderbutton { get; set; }
        [DisplayName("Borderbutton1")]
        [Required]
        public string Borderbutton1 { get; set; }
        [DisplayName("Btn2DefaultBG")]
        [Required]
        public string Btn2DefaultBG { get; set; }
        [DisplayName("Btn2DefaultForecolor")]
        [Required]
        public string Btn2DefaultForecolor { get; set; }
        [DisplayName("Btn2DefaultHoverBG")]
        [Required]
        public string Btn2DefaultHoverBG { get; set; }
        [DisplayName("Btn2DefaultHoverForecolor")]
        [Required]
        public string Btn2DefaultHoverForecolor { get; set; }
        [DisplayName("Borderbutton2")]
        [Required]
        public string Borderbutton2 { get; set; }
        [DisplayName("Borderbutton3")]
        [Required]
        public string Borderbutton3 { get; set; }
        [DisplayName("TableHeaderBG")]
        [Required]
        public string TableHeaderBG { get; set; }
        [DisplayName("TableForecolor")]
        [Required]
        public string TableForecolor { get; set; }
        [DisplayName("TableBorder")]
        [Required]
        public string TableBorder { get; set; }
        [DisplayName("TabsActiveforecolor")]
        [Required]
        public string TabsActiveforecolor { get; set; }
        [DisplayName("TabsActiveBorder")]
        [Required]
        public string TabsActiveBorder { get; set; }
        [DisplayName("TabsActiveTopborder")]
        [Required]
        public string TabsActiveTopborder { get; set; }
        [DisplayName("TabsActiveBackground")]
        [Required]
        public string TabsActiveBackground { get; set; }
        [DisplayName("TabsForecolor")]
        [Required]
        public string TabsForecolor { get; set; }
        [DisplayName("TabsBackground")]
        public string TabsBackground { get; set; }
        [DisplayName("TabsBorder")]
        [Required]
        public string TabsBorder { get; set; }
        [DisplayName("TabsHeadingBG")]
        [Required]
        public string TabsHeadingBG { get; set; }
        [DisplayName("TabsHeadingBorder")]
        [Required]
        public string TabsHeadingBorder { get; set; }
        [DisplayName("TabsForecolorHover")]
        [Required]
        public string TabsForecolorHover { get; set; }
        [DisplayName("TabsBackgroundHover")]
        [Required]
        public string TabsBackgroundHover { get; set; }
        [DisplayName("TabsBorderHover")]
        [Required]
        public string TabsBorderHover { get; set; }
        [DisplayName("SidebarActiveforcolor")]
        [Required]
        public string SidebarActiveforcolor { get; set; }
        [DisplayName("SidebarActiveBG")]
        [Required]
        public string SidebarActiveBG { get; set; }
        [DisplayName("SidebarHoverBorderLeft")]
        [Required]
        public string SidebarHoverBorderLeft { get; set; }
        [DisplayName("FooterLinkcolor")]
        [Required]
        public string FooterLinkcolor { get; set; }
        [DisplayName("FooterForecolor")]
        [Required]
        public string FooterForecolor { get; set; }
        [DisplayName("rightwrapper")]
        [Required]
        public string rightwrapper { get; set; }
    }
    public interface IThemeRepository
    {
        void EditTheme(Theme theme);
        Theme GetTheme();
    }
}