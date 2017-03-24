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
    [Table("tbl_DataSourceParameters"),DisplayName("Data Source Parameters")]
	public class DataSourceParameters : Entity
    {	

		 
		 
		[DisplayName("Argument Name"), Column("ArgumentName")] [Required] 
		
        public string ArgumentName { get; set; }

		 
		 
		[DisplayName("Argument Value"), Column("ArgumentValue")]  
		
        public string ArgumentValue { get; set; }

		 
		 
		[DisplayName("Hosting Entity"), Column("HostingEntity")]  
		
        public string HostingEntity { get; set; }

		 
		 
		[DisplayName("flag"), Column("flag")]  
		
        public Boolean? flag { get; set; }

		 
		 
		[DisplayName("other"), Column("other")]  
		
        public string other { get; set; }

		[DisplayName("Entity Data Source"), Column("EntityDataSourceParametersID")]
        public Nullable<long> EntityDataSourceParametersID { get; set; }
		
        public virtual EntityDataSource entitydatasourceparameters { get; set;}
				 public  string getDisplayValue() { 
		 try{
			var dispValue = (this.ArgumentName != null ?Convert.ToString(this.ArgumentName)+"  " : Convert.ToString(this.ArgumentName)); 
			dispValue = dispValue!=null?dispValue.TrimEnd("  -  ".ToCharArray()):"";
			this.m_DisplayValue = dispValue;
			return dispValue;
			}catch{ return "";}
		 }
		  public override string getDisplayValueModel() { 
			try{
			 if (!string.IsNullOrEmpty(m_DisplayValue))
                 return m_DisplayValue;
			var dispValue = (this.ArgumentName != null ?Convert.ToString(this.ArgumentName)+"  " : Convert.ToString(this.ArgumentName)); 
			return dispValue!=null?dispValue.TrimEnd("  -  ".ToCharArray()):"";
			}catch{ return "";}
		 }
		 public void setCalculation(){ try{  }catch{}}
    }
}

