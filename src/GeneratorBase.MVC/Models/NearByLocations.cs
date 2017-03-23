using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.ComponentModel;
using Microsoft.AspNet.Identity.EntityFramework;
using GeneratorBase.MVC.Controllers;
namespace GeneratorBase.MVC.Models
{
    public class NearByLocations
    {
        public NearByLocations(double latitude, double longitude, string address, string displayValue, string ImageFileType)
        {
            Latitude = latitude;
            Longitude = longitude;
            Address = address;
            DisplayValue = displayValue;
            Distance = 0;
            Picture = "";
            ID = 0;
            ImageFileType = "File";
        }
        public double Latitude { get; private set; }
        public double Longitude { get; private set; }
        public string Address { get; private set; }
        public string DisplayValue { get; set; }
        public double Distance { get; set; }
        public string Picture { get; set; }
        public long ID { get; set; }
        public string ImageFileType { get; set; }
    }
}