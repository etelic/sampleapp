using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
namespace GeneratorBase.MVC.Models
{
    public abstract class Entity : IEntity
    {
        [Key]
        public long Id { get; set; }
        [Timestamp]
        [ConcurrencyCheck]
        public byte[] ConcurrencyKey { get; set; }
        [NotMapped]
        public string m_DisplayValue = "";
        [DisplayName("DisplayValue")]
        public string DisplayValue
        {
            get { return getDisplayValueModel(); }
            set { value = m_DisplayValue; }
        }
 	[NotMapped]
        public TimeSpan m_OffSet
        {

            get
            {
                TimeSpan offset = TimeSpan.FromMinutes(0);
                if (HttpContext.Current != null)
                {
                    var timeZoneCookie = HttpContext.Current.Request.Cookies["_timeZoneOffset"];
                    if (timeZoneCookie != null)
                    {
                        double offsetMinutes = 0;
                        if (double.TryParse(timeZoneCookie.Value, out offsetMinutes))
                        {
                            offset = TimeSpan.FromMinutes(offsetMinutes);
                        }
                    }
                }
                return offset;
            }
            set { }
        }
        public virtual string getDisplayValueModel() { return m_DisplayValue; }
    }
}