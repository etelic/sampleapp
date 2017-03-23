using GeneratorBase.MVC.Models;
using System.Web;
using System.Xml.Linq;
using System.Linq;
using System.Collections.Generic;
using System.Data.Entity;
using System.Text;
using System;
public class ThirdPartyLoginRepository : IThirdPartyLoginRepository
{
    private XDocument thirdPartyLoginData;
    // constructor
    public ThirdPartyLoginRepository()
    {
        thirdPartyLoginData = XDocument.Load(HttpContext.Current.Server.MapPath("~/App_Data/AdminSettings.xml"));
    }
    public ThirdPartyLogin GetThirdPartyLogin()
    {
        IEnumerable<ThirdPartyLogin> thirdpartylogin = from cp in thirdPartyLoginData.Descendants("Settings")
                                                       select new ThirdPartyLogin((string)cp.Element("GooglePlusId"), (string)cp.Element("GooglePlusSecretKey"),
                                                       (string)cp.Element("FacebookId"), (string)cp.Element("FacebookSecretKey"));
        return thirdpartylogin.ToList()[0];
    }
    // Edit Record
    public void EditThirdPartyLogin(ThirdPartyLogin cp)
    {
        XElement node = thirdPartyLoginData.Root.Elements("Settings").FirstOrDefault();
        node.SetElementValue("GooglePlusId", string.IsNullOrEmpty(cp.GooglePlusId) ? "None" : cp.GooglePlusId);
        node.SetElementValue("GooglePlusSecretKey", string.IsNullOrEmpty(cp.GooglePlusSecretKey) ? "None" : cp.GooglePlusSecretKey);
        node.SetElementValue("FacebookId", string.IsNullOrEmpty(cp.FacebookId) ? "None" : cp.FacebookId);
        node.SetElementValue("FacebookSecretKey", string.IsNullOrEmpty(cp.FacebookSecretKey) ? "None" : cp.FacebookSecretKey);
        thirdPartyLoginData.Save(HttpContext.Current.Server.MapPath("~/App_Data/AdminSettings.xml"));
    }
}