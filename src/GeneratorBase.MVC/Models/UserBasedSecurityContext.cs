using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
namespace GeneratorBase.MVC.Models
{
    public class UserBasedSecurityContext : DbContext
    {
        public UserBasedSecurityContext()
            : base("DefaultConnection")
        {
        }
        public DbSet<UserBasedSecurity> UserBasedSecurities { get; set; }
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
