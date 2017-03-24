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
    [Table("tbl_Country"),CustomDisplayName("T_Country", "T_Country.resx", "Country")]
	public class T_Country : Entity
    {	
		public T_Country()
        {
            this.t_statecountry = new HashSet<T_State>();
            this.t_citycountry = new HashSet<T_City>();
            this.t_addresscountry = new HashSet<T_Address>();
        }

		 
		 
		[CustomDisplayName("T_Name","T_Country.resx","Name"), Column("Name")] [Required] 
		
        public string T_Name { get; set; }

		 
		 
		[CustomDisplayName("T_Description","T_Country.resx","Description"), Column("Description")]  
		
        public string T_Description { get; set; }
		
        public virtual ICollection<T_State> t_statecountry { get; set; }
		
        public virtual ICollection<T_City> t_citycountry { get; set; }
		
        public virtual ICollection<T_Address> t_addresscountry { get; set; }
		 public  string getDisplayValue() { 
		 try{
			var dispValue = (this.T_Name != null ?Convert.ToString(this.T_Name)+" " : Convert.ToString(this.T_Name)); 
			dispValue = dispValue!=null?dispValue.TrimEnd(" ".ToCharArray()):"";
			this.m_DisplayValue = dispValue;
			return dispValue;
			}catch{ return "";}
		 }
		  public override string getDisplayValueModel() { 
			try{
			 if (!string.IsNullOrEmpty(m_DisplayValue))
                 return m_DisplayValue;
			var dispValue = (this.T_Name != null ?Convert.ToString(this.T_Name)+" " : Convert.ToString(this.T_Name)); 
			return dispValue!=null?dispValue.TrimEnd(" ".ToCharArray()):"";
			}catch{ return "";}
		 }
		 public void setCalculation(){ try{  }catch{}}
    }
}

