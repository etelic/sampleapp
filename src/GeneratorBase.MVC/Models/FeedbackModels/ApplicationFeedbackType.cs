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
    [Table("tbl_ApplicationFeedbackType")]
	public class ApplicationFeedbackType : Entity
    {	
		public ApplicationFeedbackType()
        {
            this.associatedapplicationfeedbacktype = new HashSet<ApplicationFeedback>();
        }
		[DisplayName("Name")] [Required] 
        public string Name { get; set; }
		[DisplayName("Description")]  
        public string Description { get; set; }
        public virtual ICollection<ApplicationFeedback> associatedapplicationfeedbacktype { get; set; }
		 public  string getDisplayValue() { 
			var dispValue = Convert.ToString(this.Name);
            dispValue = dispValue.TrimEnd(" - ".ToCharArray());
            this.m_DisplayValue = dispValue;
            return dispValue;
		 }
         public override string getDisplayValueModel()
         {
             if (!string.IsNullOrEmpty(m_DisplayValue))
                 return m_DisplayValue;
             var dispValue = Convert.ToString(this.Name);
             return dispValue.TrimEnd(" - ".ToCharArray());
         }
		 public void setCalculation(){ try{  }catch{}}
    }
}

