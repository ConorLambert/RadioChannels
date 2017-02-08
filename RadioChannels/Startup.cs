using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using RadioChannels.DAL;
using static RadioChannels.App_Start.IdentityConfig;
using Microsoft.Owin.Security.Google;
using RadioChannels.Controllers;

namespace RadioChannels
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.CreatePerOwinContext(RadioContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);

            // this is the same as before
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Home/login")
            });

            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            app.UseGoogleAuthentication(
                new GoogleOAuth2AuthenticationOptions()
                {
                    ClientId = "357587284095-gim85juuaahqnpeau7f3tuuokn2bmob8.apps.googleusercontent.com",
                    ClientSecret = "53UOuDwOm4gDlM9Hj2dFcq9T"
                });

            app.UseFacebookAuthentication(new Microsoft.Owin.Security.Facebook.FacebookAuthenticationOptions
            {
                AppId = "1243205902424539",
                AppSecret = "7ccefdfe49196eeb74a05f1710b324d5",
                Scope = { "email" },
                BackchannelHttpHandler = new FacebookBackChannelHandler(),
                UserInformationEndpoint = "https://graph.facebook.com/v2.4/me?fields=id,name,email,first_name,last_name"
            });
            
            app.UseTwitterAuthentication(new Microsoft.Owin.Security.Twitter.TwitterAuthenticationOptions
            {
                ConsumerKey = "9VFMBCdIKdpWrZmiKmZFBAp9s",
                ConsumerSecret = "frC783FH7gQxun2g0l3CzJut1WGHDevEwbjAvW5unH4uZXICog",              
                BackchannelCertificateValidator = null ,
                Provider = new LinqToTwitterAuthenticationProvider()
            });            
        }
    }
}