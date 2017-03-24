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
            //master page icon
            this.Icon = "logo.gif";
            this.IconHeight = "28px";
            this.IconWidth = "28px";
            //
            //login logo page
            this.Logo = "logo_white.png";
            this.LogoWidth = "155px";
            this.LogoHeight = "29px";
            //
            this.AboutCompany = "About Company";
            //LegalInformation
            this.LegalInformation = "Legal Information";
            this.LegalInformationLink = "/PolicyAndService/Licensing.pdf";
            this.LegalInformationAttach = "Licensing.pdf";
            //end
            //PrivacyPolicy
            this.PrivacyPolicy = "Privacy Policy";
            this.PrivacyPolicyLink = "/PolicyAndService/PrivacyPolicy.pdf";
            this.PrivacyPolicyAttach = "PrivacyPolicy.pdf";
            //End
            //Terms Of Service
            this.TermsOfService = "Terms Of Service";
            this.TermsOfServiceLink = "/PolicyAndService/Terms_Of_Service.pdf";
            this.TermsOfServiceAttach = "Terms_Of_Service.pdf";
            //End
            //Emailto
            this.Emailto = "Email To";
            this.EmailtoAddress = "contact@turanto.com";
            //End
            //Create By
            this.CreatedBy = "Created By";
            this.CreatedByName = "Turanto";
            this.CreatedByLink = "http://www.turanto.com/";
        }
        public CompanyProfile(string name, string email, string address, string country, string state, string city,
            string zip, string contact1, string contact2, string smtpserver,
            string smtppassword, int smtpport, bool ssl,
             string aboutcompany,
            string _Iconwidth, string _Iconheight,
            string logowidth, string logoheight,
            string _Icon, string logo,
            string legalInformation,
            string legalInformationLink,
            string legalInformationAttach,
            string privacyPolicy,
            string privacyPolicyLink,
            string privacyPolicyAttach,
            string termsOfService,
            string termsOfServiceLink,
            string termsOfServiceAttach,
            string emailto,
            string emailtoAddress,
            string createdBy,
            string createdByLink,
            string createdByName)
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
            //master page icon
            this.Icon = _Icon;
            this.IconWidth = _Iconwidth;
            this.IconHeight = _Iconheight;
            //login page logo
            this.Logo = logo;
            this.LogoWidth = logowidth;
            this.LogoHeight = logoheight;
            //
            this.AboutCompany = aboutcompany;
            //Legal Information
            this.LegalInformation = legalInformation;
            this.LegalInformationLink = legalInformationLink;
            this.LegalInformationAttach = legalInformationAttach;
            //end
            //PrivacyPolicy
            this.PrivacyPolicy = privacyPolicy;
            this.PrivacyPolicyLink = privacyPolicyLink;
            this.PrivacyPolicyAttach = privacyPolicyAttach;
            //End
            //Terms Of Service
            this.TermsOfService = termsOfService;
            this.TermsOfServiceLink = termsOfServiceLink;
            this.TermsOfServiceAttach = termsOfServiceAttach;
            //End
            //Emailto
            this.Emailto = emailto;
            this.EmailtoAddress = emailtoAddress;
            //End
            //Create By
            this.CreatedBy = createdBy;
            this.CreatedByLink = createdByLink;
            this.CreatedByName = createdByName;
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
        //Icon Master Page
        [DisplayName("Company Icon")]
        public string Icon { get; set; }
        [DisplayName("Icon Width")]
        public string IconWidth { get; set; }
        [DisplayName("Icon Height")]
        public string IconHeight { get; set; }
        //login page logo
        [DisplayName("Company Logo")]
        public string Logo { get; set; }
        [DisplayName("Logo Width")]
        public string LogoWidth { get; set; }
        [DisplayName("Logo Height")]
        public string LogoHeight { get; set; }
        //
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
        //Legal Information
        [DisplayName("Legal Information")]
        public string LegalInformation { get; set; }
        [DisplayName("Legal Information Link")]
        public string LegalInformationLink { get; set; }
        [DisplayName("Legal Information Attach")]
        public string LegalInformationAttach { get; set; }
        //end
        //PrivacyPolicy
        [DisplayName("Privacy Policy")]
        public string PrivacyPolicy { get; set; }
        [DisplayName("Privacy Policy Link")]
        public string PrivacyPolicyLink { get; set; }
        [DisplayName("Privacy Policy Attach")]
        public string PrivacyPolicyAttach { get; set; }
        //End
        //Terms Of Service
        [DisplayName("Terms Of Service")]
        public string TermsOfService { get; set; }
        [DisplayName("Terms Of Service Link")]
        public string TermsOfServiceLink { get; set; }
        [DisplayName("Terms Of Service Attach")]
        public string TermsOfServiceAttach { get; set; }
        //End
        //Emailto
        [DisplayName("Email to")]
        public string Emailto { get; set; }
        [DisplayName("Email to Address")]
        public string EmailtoAddress { get; set; }
        //End
        //Create By
        [DisplayName("Created By")]
        public string CreatedBy { get; set; }
        [DisplayName("Created By Link")]
        public string CreatedByLink { get; set; }
        [DisplayName("Created By Name")]
        public string CreatedByName { get; set; }

    }

    public interface ICompanyProfileRepository
    {
        void EditCompanyProfile(CompanyProfile cp);
        CompanyProfile GetCompanyProfile();
    }
}