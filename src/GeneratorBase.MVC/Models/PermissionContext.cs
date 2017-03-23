using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
namespace GeneratorBase.MVC.Models
{
    public class PermissionContext : DbContext
    {
        public PermissionContext()
            : base("DefaultConnection")
        {
        }
        public DbSet<Permission> Permissions { get; set; }
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
