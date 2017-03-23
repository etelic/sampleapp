using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
namespace GeneratorBase.MVC.Models
{
    public class ReportsContext : DbContext
    {
        public ReportsContext()
            : base("DefaultConnection")
        {
        }
        public DbSet<Reports> Reportss { get; set; }
        public override int SaveChanges()
        {
			DoBusinessRules();
            var result = base.SaveChanges();
			return result;
        }
		private void DoBusinessRules()
		{
			//Business Rules goes here.....
		}
    }
}


