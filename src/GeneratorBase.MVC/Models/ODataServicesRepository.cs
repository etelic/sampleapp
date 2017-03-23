using GeneratorBase.MVC.Models;
using System.Web;
using System.Xml.Linq;
using System.Linq;
using System.Collections.Generic;
using System.Data.Entity;
using System.Text;
using System;
public class ODataServicesRepository : IODataServicesRepository
{
    private XDocument ODataServicesData;
    // constructor
    public ODataServicesRepository()
    {
        ODataServicesData = XDocument.Load(HttpContext.Current.Server.MapPath("~/App_Data/AdminSettings.xml"));
    }
    public ODataServices GetODataServices()
    {
        IEnumerable<ODataServices> ODataServices = from cp in ODataServicesData.Descendants("Settings")
                                                   select new ODataServices((bool)cp.Element("EnableODataServices"));
        return ODataServices.ToList()[0];
    }
    // Edit Record
    public void EditODataServices(ODataServices cp)
    {
        XElement node = ODataServicesData.Root.Elements("Settings").FirstOrDefault();
        node.SetElementValue("EnableODataServices", cp.EnableODataServices == false ? false : cp.EnableODataServices);
        ODataServicesData.Save(HttpContext.Current.Server.MapPath("~/App_Data/AdminSettings.xml"));
    }
}