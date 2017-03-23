using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
namespace GeneratorBase.MVC.Models
{
    public class UserDefinePagesRoleContext : DbContext
    {
        public UserDefinePagesRoleContext()
            : base("DefaultConnection")
        {
        }
        public DbSet<UserDefinePagesRole> UserDefinePagesRoles { get; set; }
        public override int SaveChanges()
        {
			DoBusinessRules();
            return base.SaveChanges();
        }
		private void DoBusinessRules()
		{
			//Business Rules goes here.....
		}
    }
}