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
        public virtual string getDisplayValueModel() { return m_DisplayValue; }
    }
}