using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Json;
using System.Net.Http;
using System.Threading.Tasks;
using System.Linq;
using System.Web.Script.Serialization;
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
        public async static Task<T> ExecuteAPIGetRequest<T>(string EntityName, Dictionary<string, string> parameters)
        {
            try
            {
                var url = getEntityDataSource(EntityName, "JSON", "GET");
                if (string.IsNullOrEmpty(url))
                    return default(T);
                if (parameters != null && parameters.Count() > 0)
                {
                    foreach (var item in parameters)
                    {
                        url += "(" + item.Value + ")";
                    }
                }
                System.Net.WebRequest req = System.Net.WebRequest.Create(url);
                req.UseDefaultCredentials = true;
                // req.Proxy = new System.Net.WebProxy(ProxyString, true); //true means no proxy
                System.Net.WebResponse resp = req.GetResponse();
                System.IO.StreamReader sr = new System.IO.StreamReader(resp.GetResponseStream());
                var result = sr.ReadToEnd().Trim();
                JsonValue parsedJsonObject = JsonObject.Parse(result);

                return JsonValueAfterMapping<T>(EntityName, parsedJsonObject.ToString(), "JSON", "GET");

                //return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(jsonObjectAfterMapping);

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
        public async static Task<bool> ExecuteAPIPostRequest<T>(string EntityName, object obj, Dictionary<string, string> parameters)
        {
            var url = getEntityDataSource(EntityName, "JSON", "POST");
            if (string.IsNullOrEmpty(url))
                return false;

            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2;WOW64; Trident/6.0)");
            IDictionary<string, string> values = obj.ToDictionary(EntityName, "JSON");
            
            var content = new FormUrlEncodedContent(values);
            var response = await client.PostAsync(url, content);
            response.EnsureSuccessStatusCode();
            return false;
        }
        private static T JsonValueAfterMapping<T>(string EntityName, string data, string sourceType, string methodType)
        {
            Dictionary<string, string> mapping = new Dictionary<string, string>();
            var result = string.Empty;
            ApplicationContext db = new ApplicationContext(new SystemUser());
            var EntityDatasource = db.EntityDataSources.FirstOrDefault(p => p.EntityName == EntityName && p.SourceType == sourceType);
            if (EntityDatasource != null)
            {
                foreach (var map in EntityDatasource.entitypropertymapping)
                {
                    mapping.Add(map.PropertyName, map.DataName);
                }
            }
            var settings = new JsonSerializerSettings();
            settings.ContractResolver = new CustomContractResolver(mapping);
            return JsonConvert.DeserializeObject<T>(data, settings);
        }
        private static string JsonPropertyMapping(string JsonData, string EntityName, string sourceType, string methodType)
        {
            var result = string.Empty;
            ApplicationContext db = new ApplicationContext(new SystemUser());
            var EntityDatasource = db.EntityDataSources.FirstOrDefault(p => p.EntityName == EntityName && p.SourceType == sourceType);
            if (EntityDatasource == null)
                return string.Empty;
            else
            {
                foreach (var map in EntityDatasource.entitypropertymapping)
                {
                    if (methodType == "GET")
                        JsonData = JsonData.Replace("\"" + map.DataName + "\"", "\"" + map.PropertyName + "\"");
                    else
                        JsonData = JsonData.Replace("\"" + map.PropertyName + "\"", "\"" + map.DataName + "\"");
                }
            }
            result = JsonData;
            return result;
        }
        private static string getEntityDataSource(string EntityName, string sourceType, string methodType)
        {
            var result = string.Empty;
            ApplicationContext db = new ApplicationContext(new SystemUser());
            var EntityDatasource = db.EntityDataSources.FirstOrDefault(p => p.EntityName == EntityName && p.SourceType == sourceType && p.MethodType == methodType);
            if (EntityDatasource == null)
                return string.Empty;
            else
                result = EntityDatasource.DataSource;
            return result;
        }
    }
    public static class ObjectToDictionaryHelper
    {
        public static IDictionary<string, string> ToDictionary(this object source, string EntityName, string sourceType)
        {
            return source.ToDictionary<string>(EntityName, sourceType);
        }
        public static IDictionary<string, string> ToDictionary<T>(this object source, string EntityName, string sourceType)
        {
            if (source == null)
                ThrowExceptionWhenSourceArgumentIsNull();
            var dictionary = new Dictionary<string, string>();
            ApplicationContext db = new ApplicationContext(new SystemUser());
            var EntityDatasource = db.EntityDataSources.FirstOrDefault(p => p.EntityName == EntityName && p.SourceType == sourceType);
            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(source))
                AddPropertyToDictionary<string>(property, source, dictionary, EntityDatasource);
            return dictionary;
        }
        private static void AddPropertyToDictionary<T>(PropertyDescriptor property, object source, Dictionary<string, string> dictionary, EntityDataSource EntityDatasource)
        {
            object value = property.GetValue(source);
            if (EntityDatasource != null)
            {
                var mapping = EntityDatasource.entitypropertymapping.FirstOrDefault(p => p.PropertyName == property.Name);
                if (mapping != null)
                {
                    dictionary.Add(mapping.DataName, Convert.ToString(value));
                    return;
                }
            }
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
    public class CustomContractResolver : Newtonsoft.Json.Serialization.DefaultContractResolver
    {
        private Dictionary<string, string> PropertyMappings { get; set; }

        public CustomContractResolver(Dictionary<string, string> mapping)
        {
            this.PropertyMappings = mapping;
        }

        protected override string ResolvePropertyName(string propertyName)
        {
            string resolvedName = null;
            var resolved = this.PropertyMappings.TryGetValue(propertyName, out resolvedName);
            return (resolved) ? resolvedName : base.ResolvePropertyName(propertyName);
        }
    }
}