using GeneratorBase.MVC.Models;
using System.Web;
using System.Xml.Linq;
using System.Linq;
using System.Collections.Generic;
using System.Data.Entity;
using System.Text;
using System;
public class CompanyProfileRepository : ICompanyProfileRepository
{
    private XDocument companyData;
    // constructor
    public CompanyProfileRepository()
    {
        companyData = XDocument.Load(HttpContext.Current.Server.MapPath("~/App_Data/CompanyProfile.xml"));
    }
    private string Encryptdata(string password)
    {
        string strmsg = string.Empty;
        byte[] encode = new byte[password.Length];
        encode = Encoding.UTF8.GetBytes(password);
        strmsg = Convert.ToBase64String(encode);
        return strmsg;
    }
    private string Decryptdata(string password)
    {
        string decryptpwd = string.Empty;
        UTF8Encoding encodepwd = new UTF8Encoding();
        Decoder Decode = encodepwd.GetDecoder();
        byte[] todecode_byte = Convert.FromBase64String(password);
        int charCount = Decode.GetCharCount(todecode_byte, 0, todecode_byte.Length);
        char[] decoded_char = new char[charCount];
        Decode.GetChars(todecode_byte, 0, todecode_byte.Length, decoded_char, 0);
        decryptpwd = new String(decoded_char);
        return decryptpwd;
    }
    public CompanyProfile GetCompanyProfile()
    {
        IEnumerable<CompanyProfile> companyprofile = from cp in companyData.Descendants("item")
                                                     select new CompanyProfile((string)cp.Element("Name"), (string)cp.Element("Email").Value,
                                                     (string)cp.Element("Address").Value,
                                                     (string)cp.Element("Country"),
                                                     (string)cp.Element("State").Value,
                                                     (string)cp.Element("City"),
                                                     (string)cp.Element("Zip"),
                                                     (string)cp.Element("ContactNumber1"),
                                                     (string)cp.Element("ContactNumber2"),
                                                     (string)cp.Element("SMTPServer"),
                                                     Decryptdata((string)cp.Element("SMTPPassword")),
                                                     (int)cp.Element("SMTPPort"),
                                                     (bool)cp.Element("SSL"),
                                                     (string)cp.Element("AboutCompany"),
                                                     (string)cp.Element("IconWidth"),
                                                     (string)cp.Element("IconHeight"),
                                                     (string)cp.Element("LogoWidth"),
                                                     (string)cp.Element("LogoHeight"),
                                                      (string)cp.Element("Icon"),
                                                     (string)cp.Element("Logo"),
                                                      (string)cp.Element("LegalInformation"),
                                                        (string)cp.Element("LegalInformationLink"),
                                                        (string)cp.Element("LegalInformationAttach"),
                                                        (string)cp.Element("PrivacyPolicy"),
                                                        (string)cp.Element("PrivacyPolicyLink"),
                                                        (string)cp.Element("PrivacyPolicyAttach"),
                                                        (string)cp.Element("TermsOfService"),
                                                        (string)cp.Element("TermsOfServiceLink"),
                                                        (string)cp.Element("TermsOfServiceAttach"),
                                                        (string)cp.Element("Emailto"),
                                                        (string)cp.Element("EmailtoAddress"),
                                                        (string)cp.Element("CreatedBy"),
                                                        (string)cp.Element("CreatedByLink"),
                                                        (string)cp.Element("CreatedByName"));
        return companyprofile.ToList()[0];
    }

    public void EditCompanyProfile(CompanyProfile cp)
    {
        XElement node = companyData.Root.Elements("item").FirstOrDefault();
        node.SetElementValue("Name", string.IsNullOrEmpty(cp.Name) ? "" : cp.Name);
        node.SetElementValue("Email", string.IsNullOrEmpty(cp.Email) ? "" : cp.Email);
        node.SetElementValue("Address", string.IsNullOrEmpty(cp.Address) ? "" : cp.Address);
        node.SetElementValue("Country", string.IsNullOrEmpty(cp.Country) ? "" : cp.Country);
        node.SetElementValue("State", string.IsNullOrEmpty(cp.State) ? "" : cp.State);
        node.SetElementValue("City", string.IsNullOrEmpty(cp.City) ? "" : cp.City);
        node.SetElementValue("Zip", string.IsNullOrEmpty(cp.Zip) ? "" : cp.Zip);
        node.SetElementValue("ContactNumber1", string.IsNullOrEmpty(cp.ContactNumber1) ? "" : cp.ContactNumber1);
        node.SetElementValue("ContactNumber2", string.IsNullOrEmpty(cp.ContactNumber2) ? "" : cp.ContactNumber2);
        node.SetElementValue("SMTPServer", string.IsNullOrEmpty(cp.SMTPServer) ? "" : cp.SMTPServer);
        node.SetElementValue("SMTPPassword", string.IsNullOrEmpty(cp.SMTPPassword) ? "" : Encryptdata(cp.SMTPPassword));
        node.SetElementValue("SMTPPort", string.IsNullOrEmpty(Convert.ToString(cp.SMTPPort)) ? "" : Convert.ToString(cp.SMTPPort));
        node.SetElementValue("SSL", string.IsNullOrEmpty(Convert.ToString(cp.SSL)) ? "" : Convert.ToString(cp.SSL));
        //master page icon
        node.SetElementValue("IconWidth", string.IsNullOrEmpty(cp.IconWidth) ? "28px" : cp.IconWidth);
        node.SetElementValue("IconHeight", string.IsNullOrEmpty(cp.IconHeight) ? "28px" : cp.IconHeight);
        node.SetElementValue("Icon", string.IsNullOrEmpty(cp.Icon) ? "logo.gif" : cp.Icon);
        //login logo page 
        node.SetElementValue("LogoWidth", string.IsNullOrEmpty(cp.LogoWidth) ? "155px" : cp.LogoWidth);
        node.SetElementValue("LogoHeight", string.IsNullOrEmpty(cp.LogoHeight) ? "29px" : cp.LogoHeight);
        node.SetElementValue("Logo", string.IsNullOrEmpty(cp.Logo) ? "logo_white.png" : cp.Logo);

        //
        node.SetElementValue("AboutCompany", string.IsNullOrEmpty(cp.AboutCompany) ? "" : cp.AboutCompany);
        //Legal Information
        node.SetElementValue("LegalInformation", string.IsNullOrEmpty(cp.LegalInformation) ? "Legal Information" : cp.LegalInformation);
        node.SetElementValue("LegalInformationLink", string.IsNullOrEmpty(cp.LegalInformationLink) ? "/PolicyAndService/Licensing.pdf" : cp.LegalInformationLink);
        node.SetElementValue("LegalInformationAttach", string.IsNullOrEmpty(cp.LegalInformationAttach) ? "Licensing.pdf" : cp.LegalInformationAttach);
        node.SetElementValue("PrivacyPolicy", string.IsNullOrEmpty(cp.PrivacyPolicy) ? "Privacy Policy" : cp.PrivacyPolicy);
        node.SetElementValue("PrivacyPolicyLink", string.IsNullOrEmpty(cp.PrivacyPolicyLink) ? "/PolicyAndService/PrivacyPolicy.pdf" : cp.PrivacyPolicyLink);
        node.SetElementValue("PrivacyPolicyAttach", string.IsNullOrEmpty(cp.PrivacyPolicyAttach) ? "PrivacyPolicy.pdf" : cp.PrivacyPolicyAttach);
        node.SetElementValue("TermsOfService", string.IsNullOrEmpty(cp.TermsOfService) ? "Terms Of Service" : cp.TermsOfService);
        node.SetElementValue("TermsOfServiceLink", string.IsNullOrEmpty(cp.TermsOfServiceLink) ? "/PolicyAndService/Terms_Of_Service.pdf" : cp.TermsOfServiceLink);
        node.SetElementValue("TermsOfServiceAttach", string.IsNullOrEmpty(cp.TermsOfServiceAttach) ? "Terms_Of_Service.pdf" : cp.TermsOfServiceAttach);
        node.SetElementValue("Emailto", string.IsNullOrEmpty(cp.Emailto) ? "Email To" : cp.Emailto);
        node.SetElementValue("EmailtoAddress", string.IsNullOrEmpty(cp.EmailtoAddress) ? "contact@turanto.com" : cp.EmailtoAddress);
        node.SetElementValue("CreatedBy", string.IsNullOrEmpty(cp.CreatedBy) ? "Created By" : cp.CreatedBy);
        node.SetElementValue("CreatedByLink", string.IsNullOrEmpty(cp.CreatedByLink) ? "http://www.turanto.com/" : cp.CreatedByLink);
        node.SetElementValue("CreatedByName", string.IsNullOrEmpty(cp.CreatedByName) ? "Turanto" : cp.CreatedByName);
        //
        companyData.Save(HttpContext.Current.Server.MapPath("~/App_Data/CompanyProfile.xml"));
    }
}