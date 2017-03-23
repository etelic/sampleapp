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
    public class ThirdPartyLogin
    {
        public ThirdPartyLogin()
        {
            this.GooglePlusId = "";
            this.GooglePlusSecretKey = "";
            this.FacebookId = "";
            this.FacebookSecretKey = "";
        }
        public ThirdPartyLogin(string goolgleplusid, string googeplussecretkey, string facebookid, string facebooksecretkey)
        {
            this.GooglePlusId = goolgleplusid;
            this.GooglePlusSecretKey = googeplussecretkey;
            this.FacebookId = facebookid;
            this.FacebookSecretKey = facebooksecretkey;
        }
        [DisplayName("GooglePlus Id")]
        public string GooglePlusId { get; set; }
        [DisplayName("GooglePlus SecretKey")]
        public string GooglePlusSecretKey { get; set; }
        [DisplayName("Facebook Id")]
        public string FacebookId { get; set; }
        [DisplayName("Facebook SecretKey")]
        public string FacebookSecretKey { get; set; }
    }
    public interface IThirdPartyLoginRepository
    {
        void EditThirdPartyLogin(ThirdPartyLogin cp);
        ThirdPartyLogin GetThirdPartyLogin();
    }
}