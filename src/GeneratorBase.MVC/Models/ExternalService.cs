using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Json;
using System.Net.Http;
using System.Threading.Tasks;
namespace GeneratorBase.MVC.Models
{
    public class SchemaGeneratorForJson
    {
        public void Generate(string jsonStr, List<string> roots, string tablename)
        {
            JsonValue parsedJsonObject = JsonObject.Parse(jsonStr);
            switch (parsedJsonObject.JsonType)
            {
                case JsonType.String:
                case JsonType.Number:
                case JsonType.Boolean:
                    //JSon properties, get the value by converting it to string
                    string value = Convert.ToString(parsedJsonObject);
                    break;
                case JsonType.Array:
                    JsonArray jArray = parsedJsonObject as JsonArray;
                    // DataTable dt = new DataTable();
                    for (int index = 0; index < jArray.Count; ++index)
                    {
                        JsonValue jArrayItem = jArray[index];
                        Generate(jArrayItem.ToString(), roots, tablename);
                        break;
                    } // ds.Tables.Add(dt);
                    break;
                case JsonType.Object:
                    JsonObject jObject = parsedJsonObject as JsonObject;
                    foreach (string key in jObject.Keys)
                    {
                        if (roots.Contains(key))
                        {
                            JsonValue jSubObject = jObject[key];
                            // ds.Tables.Add(dt);
                            // entities.Add(new SchemaEntity { Name = key });
                            Generate(jSubObject.ToString(), roots, key);
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(tablename))//&& entities.FirstOrDefault(p => p.Name == tablename) == null)
                            {
                                // DataTable dt1 = new DataTable(tablename);
                                // ds.Tables.Add(dt1);
                                //  entities.Add(new SchemaEntity { Name = tablename });
                            }
                            else if (string.IsNullOrEmpty(tablename))
                            {
                                //  DataTable dt1 = new DataTable("External Integration Service");
                                //ds.Tables.Add(dt1);
                                tablename = "External Integration Service";
                                //entities.Add(new SchemaEntity { Name = "External Integration Service" });
                            }
                            //  var ent = entities.FirstOrDefault(p => p.Name == tablename);
                            try
                            {
                                //    ent.Properties.Add(new SchemaProperty { Name = key });
                            }
                            catch
                            { }
                            // dt.Columns.Add(key);
                            //Now recursively parse, each usb item. i.e jSubObject
                        }
                    }
                    break;
            }
        }
    }
    public static class ExternalService
    {
        public async static Task<T> ExecuteAPIGetRequest<T>(string url, Dictionary<string, string> parameters)
        {
            try
            {
                System.Net.WebRequest req = System.Net.WebRequest.Create(url);
                req.UseDefaultCredentials = true;
                // req.Proxy = new System.Net.WebProxy(ProxyString, true); //true means no proxy
                System.Net.WebResponse resp = req.GetResponse();
                System.IO.StreamReader sr = new System.IO.StreamReader(resp.GetResponseStream());
                var result = sr.ReadToEnd().Trim();
                JsonValue parsedJsonObject = JsonObject.Parse(result);
                return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(parsedJsonObject.ToString());
            }
            catch { return default(T); }
        }
        public async static Task<T> ExecuteAPIGetRequest_old<T>(string url, Dictionary<string, string> parameters)
        {
            var client = new HttpClient(new HttpClientHandler { UseDefaultCredentials = true });
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2;WOW64; Trident/6.0)");
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            var response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                JsonValue parsedJsonObject = JsonObject.Parse(result);
                return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(parsedJsonObject.ToString());
            }
            else
                return default(T);
        }
        public async static Task<bool> ExecuteAPIDeleteRequest<T>(string url, object obj, Dictionary<string, string> parameters)
        {
            //Todo: delete item
            return false;
        }
        public async static Task<bool> ExecuteAPIPostRequest<T>(string url, object obj, Dictionary<string, string> parameters)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2;WOW64; Trident/6.0)");
            IDictionary<string, string> values = obj.ToDictionary();
            string toBeSend = JsonConvert.SerializeObject((T)obj);
            var content = new FormUrlEncodedContent(values);
            var response = await client.PostAsync(url, content);
            response.EnsureSuccessStatusCode();
            return false;
        }
    }
    public static class ObjectToDictionaryHelper
    {
        public static IDictionary<string, string> ToDictionary(this object source)
        {
            return source.ToDictionary<string>();
        }
        public static IDictionary<string, string> ToDictionary<T>(this object source)
        {
            if (source == null)
                ThrowExceptionWhenSourceArgumentIsNull();
            var dictionary = new Dictionary<string, string>();
            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(source))
                AddPropertyToDictionary<string>(property, source, dictionary);
            return dictionary;
        }
        private static void AddPropertyToDictionary<T>(PropertyDescriptor property, object source, Dictionary<string, string> dictionary)
        {
            object value = property.GetValue(source);
            // if (IsOfType<T>(value))
            dictionary.Add(property.Name, Convert.ToString(value));
        }
        private static bool IsOfType<T>(object value)
        {
            return value is T;
        }
        private static void ThrowExceptionWhenSourceArgumentIsNull()
        {
            throw new ArgumentNullException("source", "Unable to convert object to a dictionary. The source object is null.");
        }
    }
}