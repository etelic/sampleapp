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
    public class AdminSettings
    {
        public AdminSettings()
        {
            this.DefaultRoleForNewUser = "None";
        }
        public AdminSettings(string defaultrolefornewuser)
        {
            this.DefaultRoleForNewUser = defaultrolefornewuser;
        }
        [DisplayName("Set Default Role For New User")]
        public string DefaultRoleForNewUser { get; set; }
    }
    public interface IAdminSettingsRepository
    {
        void EditAdminSettings(AdminSettings adminSettings);
        AdminSettings GetAdminSettings();
    }
}