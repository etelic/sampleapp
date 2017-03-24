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
    [Table("tbl_State"),CustomDisplayName("T_State", "T_State.resx", "State")]
	public class T_State : Entity
    {	
		public T_State()
        {
            this.t_citystate = new HashSet<T_City>();
            this.t_addressstate = new HashSet<T_Address>();
        }

		 
		 
		[CustomDisplayName("T_Name","T_State.resx","Name"), Column("Name")] [Required] 
		
        public string T_Name { get; set; }

		 
		 
		[CustomDisplayName("T_Description","T_State.resx","Description"), Column("Description")]  
		
        public string T_Description { get; set; }

		[CustomDisplayName("T_StateCountryID","T_State.resx","State Country"), Column("StateCountryID")]
        public Nullable<long> T_StateCountryID { get; set; }
		
        public virtual T_Country t_statecountry { get; set;}
				
        public virtual ICollection<T_City> t_citystate { get; set; }
		
        public virtual ICollection<T_Address> t_addressstate { get; set; }
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

