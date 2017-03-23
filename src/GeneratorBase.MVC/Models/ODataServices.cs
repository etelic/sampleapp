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
    public class ODataServices
    {
        public ODataServices()
        {
            this.EnableODataServices = false;
        }
        public ODataServices(bool enableodataservices)
        {
            this.EnableODataServices = enableodataservices;
        }
        [DisplayName("Enable OData Services")]
        public bool EnableODataServices { get; set; }
    }
    public interface IODataServicesRepository
    {
        void EditODataServices(ODataServices cp);
        ODataServices GetODataServices();
    }
}