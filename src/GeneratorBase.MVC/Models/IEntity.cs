using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
namespace GeneratorBase.MVC.Models
{
    public interface IEntity
    {
        long Id { get; set; }
        byte[] ConcurrencyKey { get; set; }
        string DisplayValue { get; set; }
    }
}