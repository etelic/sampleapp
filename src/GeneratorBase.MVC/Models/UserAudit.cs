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
    [Table("LoginAttempts")]
    public class LoginAttempts
    {
        [Key]
        public long Id { get; set; }
        public string UserId { get; set; }
        public System.DateTime Date { get; set; }
        public string IPAddress { get; set; }
        public bool IsSuccessfull { get; set; }
    }
    [Table("PasswordHistory")]
    public class PasswordHistory
    {
        [Key]
        public long Id { get; set; }
        public string UserId { get; set; }
        public System.DateTime Date { get; set; }
        public string HashedPassword { get; set; }

    }
}

