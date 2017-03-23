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
    [Table("tbl_FeedbackResource")]
	public class FeedbackResource : Entity
    {	
		public FeedbackResource()
        {
            this.applicationfeedbackresource = new HashSet<ApplicationFeedback>();
        }
		[DisplayName("Resource Id")]  
        public Nullable<long> ResourceId { get; set; }
		[DisplayName("First Name")] [Required] 
        public string FirstName { get; set; }
		[DisplayName("Last Name")]  
        public string LastName { get; set; }
           [RegularExpression(@"^[\w\.-]{1,}\@([\da-zA-Z-]{1,}\.){1,}[\da-zA-Z-]+$", ErrorMessage = "Invalid Email")]
		[DisplayName("Email")]  
        public string Email { get; set; }
           [RegularExpression(@"^\d{3}\-?\d{3}\-?\d{4}$", ErrorMessage = "Invalid Phone No")]
		[DisplayName("Phone No")]  
        public string PhoneNo { get; set; }
        public virtual ICollection<ApplicationFeedback> applicationfeedbackresource { get; set; }
		 public string getDisplayValue() { 
			var dispValue = Convert.ToString(this.ResourceId)+" - "+Convert.ToString(this.FirstName); 
            dispValue = dispValue.TrimEnd(" - ".ToCharArray());
            this.m_DisplayValue = dispValue;
            return dispValue;
		 }
         public override string getDisplayValueModel()
         {
             if (!string.IsNullOrEmpty(m_DisplayValue))
                 return m_DisplayValue;
             var dispValue = Convert.ToString(this.ResourceId) + " - " + Convert.ToString(this.FirstName);
             return dispValue.TrimEnd(" - ".ToCharArray());
         }
		 public void setCalculation(){ try{  }catch{}}
    }
}

