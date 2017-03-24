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
    [Table("tbl_PropertyMapping"),DisplayName("Property Mapping")]
	public class PropertyMapping : Entity
    {	

		 
		 
		[DisplayName("Property Name"), Column("PropertyName")] [Required] 
		
        public string PropertyName { get; set; }

		 
		 
		[DisplayName("Data Name"), Column("DataName")]  
		
        public string DataName { get; set; }

		 
		 
		[DisplayName("Data Source"), Column("DataSource")]  
		
        public string DataSource { get; set; }

		 
		 
		[DisplayName("Source Type"), Column("SourceType")]  
		
        public string SourceType { get; set; }

		 
		 
		[DisplayName("Method Type"), Column("MethodType")]  
		
        public string MethodType { get; set; }

		 
		 
		[DisplayName("Action"), Column("Action")]  
		
        public string Action { get; set; }

		[DisplayName("Entity Data Source"), Column("EntityPropertyMappingID")]
        public Nullable<long> EntityPropertyMappingID { get; set; }
		
        public virtual EntityDataSource entitypropertymapping { get; set;}
				 public  string getDisplayValue() { 
		 try{
			var dispValue = (this.PropertyName != null ?Convert.ToString(this.PropertyName)+"  " : Convert.ToString(this.PropertyName)); 
			dispValue = dispValue!=null?dispValue.TrimEnd("  -  ".ToCharArray()):"";
			this.m_DisplayValue = dispValue;
			return dispValue;
			}catch{ return "";}
		 }
		  public override string getDisplayValueModel() { 
			try{
			 if (!string.IsNullOrEmpty(m_DisplayValue))
                 return m_DisplayValue;
			var dispValue = (this.PropertyName != null ?Convert.ToString(this.PropertyName)+"  " : Convert.ToString(this.PropertyName)); 
			return dispValue!=null?dispValue.TrimEnd("  -  ".ToCharArray()):"";
			}catch{ return "";}
		 }
		 public void setCalculation(){ try{  }catch{}}
    }
}

