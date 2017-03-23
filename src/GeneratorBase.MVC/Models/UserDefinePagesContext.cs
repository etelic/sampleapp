using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
namespace GeneratorBase.MVC.Models
{
    public class UserDefinePagesContext : DbContext
    {
        public UserDefinePagesContext()
            : base("DefaultConnection")
        {
        }
        public DbSet<UserDefinePages> UserDefinePagess { get; set; }
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
