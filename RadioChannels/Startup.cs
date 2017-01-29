using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using RadioChannels.DAL;
using RadioChannels.Models;
using System;

namespace RadioChannels
{
    public class Startup
    {     
        public void Configuration(IAppBuilder app)
        {      
            // this is the same as before
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/auth/login")
            });
            
        }
    }
}