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
    public class CompanyProfile
    {
        public CompanyProfile()
        {
            this.Name = "Turanto";
            this.Email = "Contact@turanto.com";
            this.Address = "";
            this.Country = "USA";
            this.State = "VA";
            this.City = "";
            this.Zip = "";
            this.ContactNumber1 = "";
            this.ContactNumber2 = "";
            this.SMTPServer = "";
            this.SMTPPassword = "";
            this.SMTPPort = 786;
            this.SSL = false;
            this.logo = "logo.gif";
            this.AboutCompany = "About Company";
        }
        public CompanyProfile(string name, string email, string address, string country, string state, string city, string zip, string contact1, string contact2, string smtpserver, string smtppassword, int smtpport, bool ssl, string _logo, string aboutcompany)
        {
            this.Name = name;
            this.Email = email;
            this.Address = address;
            this.Country = country;
            this.State = state;
            this.City = city;
            this.Zip = zip;
            this.ContactNumber1 = contact1;
            this.ContactNumber2 = contact2;
            this.SMTPServer = smtpserver;
            this.SMTPPassword = smtppassword;
            this.SMTPPort = smtpport;
            this.SSL = ssl;
            this.logo = _logo;
            this.AboutCompany = aboutcompany;
        }
        [DisplayName("Company Name")]
        [Required]
        public string Name { get; set; }
        [DisplayName("Company Email")]
        [Required]
        public string Email { get; set; }
        [DisplayName("Company Address")]
        public string Address { get; set; }
        [DisplayName("Company Country")]
        public string Country { get; set; }
        [DisplayName("Company State")]
        public string State { get; set; }
        [DisplayName("Company City")]
        public string City { get; set; }
        [DisplayName("Company Zip-Code")]
        public string Zip { get; set; }
        [DisplayName("Contact Number 1")]
        public string ContactNumber1 { get; set; }
        [DisplayName("Contact Number 2")]
        public string ContactNumber2 { get; set; }
        [DisplayName("Company Logo")]
        public string logo { get; set; }
        //SMTP SERVER DETAILS
        [DisplayName("SMTP Server")]
        [Required]
        public string SMTPServer { get; set; }
        [DisplayName("SMTP Password")]
        [Required]
        public string SMTPPassword { get; set; }
        [DisplayName("SMTP Port")]
        [Required]
        public int SMTPPort { get; set; }
        [DisplayName("SSL Authentication")]
        public bool? SSL { get; set; }
        //
        //About Company
        [DisplayName("About Company")]
        [System.Web.Mvc.AllowHtml]
        public string AboutCompany { get; set; }
        //
    }
    public interface ICompanyProfileRepository
    {
        void EditCompanyProfile(CompanyProfile cp);
       CompanyProfile GetCompanyProfile();
    }
}