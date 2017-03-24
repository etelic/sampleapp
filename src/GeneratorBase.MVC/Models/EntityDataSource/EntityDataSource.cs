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
    [Table("tbl_EntityDataSource"),DisplayName("Entity Data Source")]
	public class EntityDataSource : Entity
    {	
		public EntityDataSource()
        {
            this.entitydatasourceparameters = new HashSet<DataSourceParameters>();
            this.entitypropertymapping = new HashSet<PropertyMapping>();
        }

		 
		 
		[DisplayName("Entity Name"), Column("EntityName")] [Required] 
		
        public string EntityName { get; set; }

		 
		 
		[DisplayName("Data Source"), Column("DataSource")]  
		
        public string DataSource { get; set; }

		 
		 
		[DisplayName("Source Type"), Column("SourceType")]  
		
        public string SourceType { get; set; }

		 
		 
		[DisplayName("Method Type"), Column("MethodType")]  
		
        public string MethodType { get; set; }

		 
		 
		[DisplayName("Action"), Column("Action")]  
		
        public string Action { get; set; }

		 
		 
		[DisplayName("Disable"), Column("flag")]  
		
        public Boolean? flag { get; set; }

		 
		 
		[DisplayName("APIKey"), Column("other")]  
		
        public string other { get; set; }
		
        public virtual ICollection<DataSourceParameters> entitydatasourceparameters { get; set; }
		
        public virtual ICollection<PropertyMapping> entitypropertymapping { get; set; }
		 public  string getDisplayValue() { 
		 try{
			var dispValue = (this.EntityName != null ?Convert.ToString(this.EntityName)+" - " : Convert.ToString(this.EntityName))+(this.SourceType != null ?Convert.ToString(this.SourceType)+" - " : Convert.ToString(this.SourceType))+(this.Action != null ?Convert.ToString(this.Action)+" - " : Convert.ToString(this.Action)); 
			dispValue = dispValue!=null?dispValue.TrimEnd(" - ".ToCharArray()):"";
			this.m_DisplayValue = dispValue;
			return dispValue;
			}catch{ return "";}
		 }
		  public override string getDisplayValueModel() { 
			try{
			 if (!string.IsNullOrEmpty(m_DisplayValue))
                 return m_DisplayValue;
			var dispValue = (this.EntityName != null ?Convert.ToString(this.EntityName)+" - " : Convert.ToString(this.EntityName))+(this.SourceType != null ?Convert.ToString(this.SourceType)+" - " : Convert.ToString(this.SourceType))+(this.Action != null ?Convert.ToString(this.Action)+" - " : Convert.ToString(this.Action)); 
			return dispValue!=null?dispValue.TrimEnd(" - ".ToCharArray()):"";
			}catch{ return "";}
		 }
		 public void setCalculation(){ try{  }catch{}}
    }
}

