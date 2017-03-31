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
    [Table("tbl_City"),CustomDisplayName("T_City", "T_City.resx", "City")]
	public class T_City : Entity
    {	
		public T_City()
        {
            this.t_addresscity = new HashSet<T_Address>();
        }

		 
		 
		[CustomDisplayName("T_Name","T_City.resx","Name"), Column("Name")] [Required] 
		
        public string T_Name { get; set; }

		 
		 
		[CustomDisplayName("T_Description","T_City.resx","Description"), Column("Description")]  
		
        public string T_Description { get; set; }

		[CustomDisplayName("T_CityCountryID","T_City.resx","City Country"), Column("CityCountryID")]
        public Nullable<long> T_CityCountryID { get; set; }
		
        public virtual T_Country t_citycountry { get; set;}
		
		[CustomDisplayName("T_CityStateID","T_City.resx","City State"), Column("CityStateID")]
        public Nullable<long> T_CityStateID { get; set; }
		
        public virtual T_State t_citystate { get; set;}
				
        public virtual ICollection<T_Address> t_addresscity { get; set; }
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

