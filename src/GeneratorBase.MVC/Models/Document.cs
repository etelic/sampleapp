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
    [Table("tbl_Document")]
    public class Document:IEntity
    {
        [Key]
        public long Id { get; set; }
        [Timestamp]
        [ConcurrencyCheck]
        public byte[] ConcurrencyKey { get; set; }
        [DisplayName("DocumentName")]
        public string DocumentName { get; set; }
        [DisplayName("DateCreated")]
        public DateTime DateCreated { get; set; }
        [DisplayName("DateLastUpdated")]
        public DateTime DateLastUpdated { get; set; }
        [DisplayName("Description")]
        public string Description { get; set; }
        [DisplayName("DisplayValue")]
        public string DisplayValue { get; set; }
        [DisplayName("FileExtension")]
        public string FileExtension { get; set; }
        [DisplayName("FileName")]
        public string FileName { get; set; }
        [DisplayName("FileSize")]
        public long FileSize { get; set; }
        [DisplayName("MIMEType")]
        public string MIMEType { get; set; }
        [DisplayName("SearchableText")]
        public string SearchableText { get; set; }
        [DisplayName("Byte")]
        public byte[] Byte { get; set; }
        public string getDisplayValue() { return Convert.ToString(this.DocumentName); }
    }
}

