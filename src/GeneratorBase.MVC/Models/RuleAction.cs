using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.ComponentModel;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Web.Mvc;
namespace GeneratorBase.MVC.Models
{
    [Table("tbl_RuleAction")]
    public class RuleAction
    {
        public RuleAction()
        {
            this.actionarguments = new HashSet<ActionArgs>();
        }
        [Key]
        public long Id { get; set; }
        [DisplayName("Action Name")]
        [Required]
        public string ActionName { get; set; }
        [DisplayName("Send Email To")]
        [AllowHtml]
        public string ErrorMessage { get; set; }
        [DisplayName("Is Else Action")]
        public Boolean IsElseAction { get; set; }
        [DisplayName("Business Rule")]
        public Nullable<long> RuleActionID { get; set; }
        //  [ForeignKey("RuleActionID")]
        public virtual BusinessRule ruleaction { get; set; }
        [DisplayName("Action Type")]
        public Nullable<long> AssociatedActionTypeID { get; set; }
        //  [ForeignKey("AssociatedActionTypeID")]
        public virtual ActionType associatedactiontype { get; set; }
        public virtual ICollection<ActionArgs> actionarguments { get; set; }
        [DisplayName("DisplayValue")]
        public string DisplayValue { get; set; }
        public string getDisplayValue() { return Convert.ToString(this.ActionName); }
        public string getRolesNameById(string RolesIds)
        {
            if (RolesIds == "0")
                return "All";
            string result = "";
            string[] RolesIdstr = RolesIds.Split(",".ToCharArray());
            foreach (var item in RolesIdstr)
            {
                using (var usersContext = new ApplicationDbContext(true))
                {
                    string rolename = usersContext.Roles.FirstOrDefault(r => item == r.Id).Name;
                    result += rolename + ",";
                }
            }
            return result;
        }
    }
}

