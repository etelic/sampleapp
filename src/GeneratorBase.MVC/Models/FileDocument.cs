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
    [Table("tbl_FileDocument"),DisplayName("Document")]
	public class FileDocument : Entity
    {	

		 
		 
		[DisplayName("Document Name"), Column("DocumentName")] [Required] 
		
        public string DocumentName { get; set; }

		 
		 
		[DisplayName("Description"), Column("Description")]  
		
        public string Description { get; set; }

		 
		 
		[DisplayName("Attach Document"), Column("AttachDocument")]  
		
        public string AttachDocument { get; set; }

		[DataType(DataType.DateTime)][DisplayFormat(DataFormatString = "{0:MM/dd/yyyy hh:mm tt}", ApplyFormatInEditMode = true)] 
		 
		[DisplayName("Created"), Column("DateCreated")] [Required] 
		
        public DateTime DateCreated { get; set; }

		[DataType(DataType.DateTime)][DisplayFormat(DataFormatString = "{0:MM/dd/yyyy hh:mm tt}", ApplyFormatInEditMode = true)] 
		 
		[DisplayName("Last Updated"), Column("DateLastUpdated")] [Required] 
		
        public DateTime DateLastUpdated { get; set; }
		 public  string getDisplayValue() { 
		 try{
			var dispValue = (this.DocumentName != null ?Convert.ToString(this.DocumentName)+" " : Convert.ToString(this.DocumentName)); 
			dispValue = dispValue!=null?dispValue.TrimEnd(" ".ToCharArray()):"";
			this.m_DisplayValue = dispValue;
			return dispValue;
			}catch{ return "";}
		 }
		  public override string getDisplayValueModel() { 
			try{
			 if (!string.IsNullOrEmpty(m_DisplayValue))
                 return m_DisplayValue;
			var dispValue = (this.DocumentName != null ?Convert.ToString(this.DocumentName)+" " : Convert.ToString(this.DocumentName)); 
			return dispValue!=null?dispValue.TrimEnd(" ".ToCharArray()):"";
			}catch{ return "";}
		 }
		 public void setCalculation(){ try{  }catch{}}
    }
}

