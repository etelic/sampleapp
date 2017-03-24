using GeneratorBase.MVC.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using System;
using System.Configuration;
using Microsoft.Owin.Security.Facebook;
using Owin.Security.Providers.Yahoo;
using Owin.Security.Providers.OpenID;
using Owin.Security.Providers;
using Microsoft.Owin.Security.Google;
using Microsoft.Web.WebPages.OAuth;
using Owin.Security.Providers.GooglePlus;
using System.Security.Claims;
using System.Xml.Linq;
using System.Web;
using System.Web.Configuration;
using System.Linq;
using Microsoft.Owin.Security.DataProtection;
namespace GeneratorBase.MVC
{
    public partial class Startup
    {
        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
		internal static IDataProtectionProvider DataProtectionProvider { get; private set; }
        public void ConfigureAuth(IAppBuilder app)
        {
		DataProtectionProvider = app.GetDataProtectionProvider();
		var UseActiveDirectory = System.Configuration.ConfigurationManager.AppSettings["UseActiveDirectory"]; //CommonFunction.Instance.UseActiveDirectory();
        if (!Convert.ToBoolean(UseActiveDirectory))
        {
				// Enable the application to use a cookie to store information for the signed in user

				//set session time out value from appsetting
				ApplicationContext db = new ApplicationContext(new SystemUser());
                var appSettings = db.AppSettings;
                string timeout = appSettings.Where(p => p.Key == "ApplicationSessionTimeOut").FirstOrDefault().Value;
				 string AppUrl = appSettings.Where(p => p.Key == "AppURL").FirstOrDefault().Value;
                Int64 TimeOutValue = 525600;
                if (!string.IsNullOrEmpty(timeout))
                {
                    if (Int64.TryParse(timeout, out TimeOutValue))
                    {
                        if (TimeOutValue == 0)
                            TimeOutValue = Convert.ToInt64(525600);
                    }
                    else
                        TimeOutValue = 525600;
                }
				//
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                CookieName = AppUrl,
				ExpireTimeSpan = TimeSpan.FromMinutes(TimeOutValue),
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login")
            });
           // Use a cookie to temporarily store information about a user logging in with a third party login provider
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);
            ThirdPartyLoginRepository thirdPartyLoginRepository = new ThirdPartyLoginRepository();
            ThirdPartyLogin thirdPartyLogin = thirdPartyLoginRepository.GetThirdPartyLogin();
            //Google
			// Uncomment the following lines to enable logging in with third party login providers
            GooglePlusAuthenticationOptions google = new GooglePlusAuthenticationOptions();
            google.ClientId = thirdPartyLogin.GooglePlusId;
            google.ClientSecret = thirdPartyLogin.GooglePlusSecretKey;
            //commmented to use google authentication from GooleAuthApp
            app.UseGooglePlusAuthentication(google);
			//Yahoo
			OAuthWebSecurity.RegisterYahooClient();
            app.UseOpenIDAuthentication("/open.login.yahooapis.com/openid/op/auth", "Yahoo");
			//Facebook
			 var x = new FacebookAuthenticationOptions();
                x.Scope.Add("email");
                x.Scope.Add("friends_about_me");
                x.Scope.Add("friends_photos");
                x.AppId = thirdPartyLogin.FacebookId;
                x.AppSecret = thirdPartyLogin.FacebookSecretKey;
                x.Provider = new FacebookAuthenticationProvider()
                {
                    OnAuthenticated = async context =>
                    {
                        //Get the access token from FB and store it in the database and
                        //use FacebookC# SDK to get more information about the user
                        context.Identity.AddClaim(
                        new System.Security.Claims.Claim("FacebookAccessToken", context.AccessToken));
                    }
                };
                x.SignInAsAuthenticationType = DefaultAuthenticationTypes.ExternalCookie;
                app.UseFacebookAuthentication(x);
		}
      }
    }
}
