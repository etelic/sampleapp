using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace GeneratorBase.MVC.Models
{
        public interface IFilter<T> where T : DbContext
        {
            T DbContext { get; set; }
            void ApplyBasicSecurity();
            void ApplyMainSecurity();
	    void ApplyUserBasedSecurity();
        }
}
