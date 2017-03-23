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
    public class ThemeSettings
    {
        public ThemeSettings(long Id, string Name, string CssEditor, bool IsActive,bool IsDefault, string DisplayValue)
        {
            // TODO: Complete member initialization
            this.Id = Id;
            this.Name = Name;
            this.CssEditor = CssEditor;
            this.IsActive = IsActive;
            this.IsDefault = IsDefault;
            this.DisplayValue = DisplayValue;
        }
      
        [DisplayName("Id")]
        public long Id { get; set; }
        [DisplayName("Theme Name")]
        public string Name { get; set; }
        [DisplayName("CSS Editor")]
        [Required]
        public string CssEditor { get; set; }
        [DisplayName("IsActive")]
        public bool IsActive { get; set; }
        [DisplayName("IsDefault")]
        public bool IsDefault { get; set; }
        [DisplayName("Display Value")]
        public string DisplayValue { get; set; }
    }
    // Class For Mobile Theme
    public class ThemeSetingMobile
    {
        public ThemeSetingMobile(long Id, string Name, string CssName, bool IsActive)
        {
            // TODO: Complete member initialization
            this.Id = Id;
            this.Name = Name;
            this.CssName = CssName;
            this.IsActive = IsActive;
            
        }
      
        [DisplayName("Id")]
        public long Id { get; set; }
        [DisplayName("Theme Name")]
        public string Name { get; set; }
        [DisplayName("CSS Name")]
        [Required]
        public string CssName { get; set; }
        [DisplayName("IsActive")]
        public bool IsActive { get; set; }
       
    }
    //public interface IThemeSettingsRepository
    //{
    //    //void EditThemeSettings(ThemeSettings themeSettings);
    //    // void AddThemeSettings();
    //    void List<ThemeSettings> GetThemeSettings();
    //}
}