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
                       (string)cp.Element("Address").Value, (string)cp.Element("Country"),
                       (string)cp.Element("State").Value, (string)cp.Element("City"), (string)cp.Element("Zip"), (string)cp.Element("ContactNumber1"), (string)cp.Element("ContactNumber2"), (string)cp.Element("SMTPServer"), Decryptdata((string)cp.Element("SMTPPassword")), (int)cp.Element("SMTPPort"), (bool)cp.Element("SSL"), (string)cp.Element("logo"), (string)cp.Element("AboutCompany"));
        return companyprofile.ToList()[0];
    }
    // Insert Record
    //public void InsertBilling(CompanyProfile billing)
    //{
    //    billing.Name = (int)(from b in companyData.Descendants("item") orderby (int)b.Element("Name") descending select (string)b.Element("Name")).FirstOrDefault() + 1;
    //    companyData.Root.Add(new XElement("item", new XElement("id", billing.ID), new XElement("customer", billing.Customer),
    //        new XElement("type", billing.Type), new XElement("date", billing.Date.ToShortDateString()), new XElement("description", billing.Description),
    //        new XElement("hours", billing.Hours)));
    //    companyData.Save(HttpContext.Current.Server.MapPath("~/App_Data/Billings.xml"));
    //}
    // Delete Record
    //public void DeleteBilling(int id)
    //{
    //    companyData.Root.Elements("item").Where(i => (int)i.Element("id") == id).Remove();
    //    companyData.Save(HttpContext.Current.Server.MapPath("~/App_Data/Billings.xml"));
    //}
    // Edit Record
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
        node.SetElementValue("logo", string.IsNullOrEmpty(cp.logo) ? "" : cp.logo);
        node.SetElementValue("AboutCompany", string.IsNullOrEmpty(cp.AboutCompany) ? "" : cp.AboutCompany);
        companyData.Save(HttpContext.Current.Server.MapPath("~/App_Data/CompanyProfile.xml"));
    }
}