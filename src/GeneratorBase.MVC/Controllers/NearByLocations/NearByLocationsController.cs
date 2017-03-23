using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Collections;
using System.Data;
using System.IO;
using System.Web.Script.Serialization;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;
using System.Xml.Xsl;
using System.Net;
using System.Xml.Linq;
using System.Web.UI.WebControls;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using GeneratorBase.MVC.Models;
using PagedList;
namespace GeneratorBase.MVC.Controllers
{
    public class NearByLocationsController : BaseController
    {
        public double latitudeA1 { get; set; }
        public double longitudeA1 { get; set; }
        public double range { get; set; }
        public string imagePropertyName { get; set; }
        public string entityName { get; set; }
        public string address { get; set; }
		public string imageFileType { get; set; }
        //
        // GET: /NearByLocations/
         public ActionResult Index(string EntityName, string Latitude, string Longitude, string Range, string ImagePropertyName, string Address, string ImageFileType)
        {
            entityName = EntityName;
            latitudeA1 = Convert.ToDouble(Latitude);
            longitudeA1 = Convert.ToDouble(Longitude);
            range = Convert.ToDouble(Range);
            imagePropertyName = ImagePropertyName;
            address = Address;
            imageFileType = ImageFileType;
            var Result = GetInstanceData().OrderBy(P => P.Distance).ToPagedList(1, 10);
            return View(Result);
        }
        public List<NearByLocations> GetNearByLocationsData(IList list)
        {
            List<NearByLocations> lstNearByPlaces = new List<NearByLocations>();
            AddressDecoder _addressDecoder = new AddressDecoder();
            NearByLocations _addressCoordinates = null;
            double distance = 0.0;
            foreach (var obj in list)
            {
                string entityDisplayValue = (string)obj.GetType().GetProperty("DisplayValue").GetValue(obj, null);
                long ID = (long)obj.GetType().GetProperty("Id").GetValue(obj, null);
                var imageProperty = obj.GetType().GetProperty(imagePropertyName).GetValue(obj, null);
                if (!string.IsNullOrEmpty(imagePropertyName))
                {
                    var propLat = obj.GetType().GetProperty(imagePropertyName + "Latitude").GetValue(obj, null);
                    var propLng = obj.GetType().GetProperty(imagePropertyName + "Longitude").GetValue(obj, null);
                    if (address != "address" && !string.IsNullOrEmpty(address))
                    {
                        var coordinates = _addressDecoder.Geocode(address, entityDisplayValue);
                        latitudeA1 = coordinates.Latitude;
                        longitudeA1 = coordinates.Longitude;
                    }
                   distance = getDistance(Convert.ToDouble(propLat), Convert.ToDouble(propLng));
                    _addressCoordinates = new NearByLocations(Convert.ToDouble(propLat), Convert.ToDouble(propLng), "", entityDisplayValue, imageFileType);
                    if (distance > 0.0)
                        _addressCoordinates.Distance = Math.Round(distance, 2);
                    else
                        _addressCoordinates.Distance = distance;
                    _addressCoordinates.ID = ID;
                    _addressCoordinates.DisplayValue = entityDisplayValue;
                    _addressCoordinates.Picture = Convert.ToString(imageProperty);
                    _addressCoordinates.ImageFileType = imageFileType;
                    if (distance <= range)
                    {
                        lstNearByPlaces.Add(_addressCoordinates);
                    }
                }
            }
            return lstNearByPlaces;
        }
        public List<NearByLocations> GetInstanceData()
        {
            IList genericList;
            switch (entityName)
            {
           
			    default:
                    genericList = null;
                    break;
            }
            return GetNearByLocationsData(genericList);
        }
        public List<NearByLocations> GetNearByLocationsDataByAddress(IList list)
        {
            List<NearByLocations> lstNearByPlaces = new List<NearByLocations>();
            AddressDecoder _addressDecoder = new AddressDecoder();
            NearByLocations _addressCoordinates = null;
            foreach (var obj in list)
            {
                string entityDisplayValue = (string)obj.GetType().GetProperty("DisplayValue").GetValue(obj, null);
                long ID = (long)obj.GetType().GetProperty("Id").GetValue(obj, null);
                var imageProperty = obj.GetType().GetProperty(imagePropertyName).GetValue(obj, null);
                 var coordinates = _addressDecoder.Geocode(address, entityDisplayValue);
                double distance = getDistance(coordinates.Latitude, coordinates.Longitude);
                _addressCoordinates = new NearByLocations(coordinates.Latitude, coordinates.Longitude, "", entityDisplayValue, imageFileType);
                if (distance > 0.0)
                    _addressCoordinates.Distance = Math.Round(distance, 2);
                else
                    _addressCoordinates.Distance = distance;
                _addressCoordinates.ID = ID;
                _addressCoordinates.DisplayValue = entityDisplayValue;
                _addressCoordinates.Picture = Convert.ToString(imageProperty);
                _addressCoordinates.ImageFileType = imageFileType;
                if (distance <= range)
                {
                    lstNearByPlaces.Add(_addressCoordinates);
                }
            }
            return lstNearByPlaces;
        }
        public double getDistance(double latitudeB1, double longitudeB1)
        {
            double latitudeAInRadian = latitudeA1 * (Math.PI / 180.0);
            double longitudeAInRadian = longitudeA1 * (Math.PI / 180.0);
            double latitudeBInRadian = latitudeB1 * (Math.PI / 180.0);
            double longitudeBInRadian = longitudeB1 * (Math.PI / 180.0);
            //double earthRadius = 6371; //In K.M.
            double earthRadius = 3959; //In Miles
            double dlon = longitudeBInRadian - longitudeAInRadian;
            double dlat = latitudeBInRadian - latitudeAInRadian;
            double a = Math.Pow((Math.Sin(dlat / 2)), 2) + Math.Cos(latitudeAInRadian) * Math.Cos(latitudeBInRadian) * Math.Pow(Math.Sin(dlon / 2), 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            double distance = earthRadius * c;
            return distance;
        }
    }
    public class AddressDecoder
    {
        //private const string ServiceUri = "http://maps.googleapis.com/maps/api/geocode/xml?address={0}&region=be&sensor=false";
        private const string ServiceUri = "http://maps.google.com/maps/api/geocode/xml?address={0}&sensor=false";
        public Coordinates Geocode(string address, string displayValue)
        {
            if (string.IsNullOrEmpty(address))
                throw new ArgumentNullException("address");
            string requestUriString = string.Format(ServiceUri, Uri.EscapeDataString(address));
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(requestUriString);
            try
            {
                WebResponse response = request.GetResponse();
                XmlReader xmlReader = XmlReader.Create(response.GetResponseStream());
                XDocument xdoc = XDocument.Load(xmlReader);
                // Verify the GeocodeResponse status
                string status = xdoc.Element("GeocodeResponse").Element("status").Value;
                double latitude = 0.0;
                double longitude = 0.0;
                try
                {
                    ValidateGeocodeResponseStatus(status, address);
                    XElement locationElement = xdoc.Element("GeocodeResponse").Element("result").Element("geometry").Element("location");
                    latitude = (double)locationElement.Element("lat");
                    longitude = (double)locationElement.Element("lng");
                }
                catch { }
                return new Coordinates(latitude, longitude, address, displayValue);
            }
            catch (WebException ex)
            {
                switch (ex.Status)
                {
                    case WebExceptionStatus.NameResolutionFailure:
                        throw; //new ServiceOfflineException("The Google Maps geocoding service appears to be offline.", ex);
                    default:
                        throw;
                }
            }
        }
        private void ValidateGeocodeResponseStatus(string status, string address)
        {
            switch (status)
            {
                case "ZERO_RESULTS":
                    string message = string.Format("No coordinates found for address \"{0}\".", address);
                    throw new Exception(message);
                case "OVER_QUERY_LIMIT":
                    throw new Exception("Limit Exceeds");
                case "OK":
                    break;
                default:
                    throw new Exception("Unkown status code: " + status + ".");
            }
        }
    }
    public class Coordinates
    {
        public Coordinates(double latitude, double longitude, string address, string displayValue)
        {
            Latitude = latitude;
            Longitude = longitude;
            Address = address;
            DisplayValue = displayValue;
            Distance = 0;
            Picture = "";
            ID = 0;
        }
        public double Latitude { get; private set; }
        public double Longitude { get; private set; }
        public string Address { get; private set; }
        public string DisplayValue { get; set; }
        public double Distance { get; set; }
        public string Picture { get; set; }
        public long ID { get; set; }
    }
}
