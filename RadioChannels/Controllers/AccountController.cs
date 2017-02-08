using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using RadioChannels.DAL;
using RadioChannels.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using static RadioChannels.App_Start.IdentityConfig;

namespace RadioChannels.Controllers
{
    public class AccountController : Controller
    {
        private WebApiAccess access = new WebApiAccess();
        private RadioContext context;
        private ApplicationUserManager userManager;

        
        public ActionResult Index()
        {
            return PartialView();
        }
        

        // EXTERNAL LOGIN/REGISTER

        public ActionResult LoginGoogle()
        {
            HttpContext.GetOwinContext().Authentication.Challenge(new Microsoft.Owin.Security.AuthenticationProperties
            {
                RedirectUri = "/Account/ExternalLinkLoginCallback"
            }, "Google");
            return new HttpUnauthorizedResult();
        }

        public ActionResult LoginFacebook()
        {
            HttpContext.GetOwinContext().Authentication.Challenge(new Microsoft.Owin.Security.AuthenticationProperties
            {
                RedirectUri = "/Account/ExternalLinkLoginCallback"
            }, "Facebook");
            return new HttpUnauthorizedResult();
        }

        public ActionResult LoginTwitter()
        {
            HttpContext.GetOwinContext().Authentication.Challenge(new Microsoft.Owin.Security.AuthenticationProperties
            {
                RedirectUri = "/Account/ExternalLinkLoginCallback"
            }, "Twitter");
            return new HttpUnauthorizedResult();
        }

        public ActionResult LoginExternal(string provider)
        {
            HttpContext.GetOwinContext().Authentication.Challenge(new Microsoft.Owin.Security.AuthenticationProperties
            {
                RedirectUri = "/Account/ExternalLinkLoginCallback"
            }, provider);
            return new HttpUnauthorizedResult();
        }

        public async Task<ActionResult> ExternalLinkLoginCallback()
        {
            setContextProperties();
            
            // Handle external Login Callback
            var loginInfo = await AuthenticationManagerExtensions.GetExternalLoginInfoAsync(HttpContext.GetOwinContext().Authentication);
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }           

            // check if the user already exists            
            var user = new User { UserName = loginInfo.Email, Email = loginInfo.Email };
            var result = await userManager.CreateAsync(user);
            if (result.Succeeded) // if not
            {
                result = await userManager.AddLoginAsync(user.Id, loginInfo.Login);
                if (result.Succeeded)
                {
                    await new SignInManager<User, string>(userManager, HttpContext.GetOwinContext().Authentication).ExternalSignInAsync(loginInfo, isPersistent: false);
                    return RedirectToAction("index", "home");
                } else {
                    ModelState.AddModelError("", "Invalid email or password");
                    return RedirectToAction("Register", ModelState);
                }
            }
            else  // else its a basic external login
            {
                await new SignInManager<User, string>(userManager, HttpContext.GetOwinContext().Authentication).ExternalSignInAsync(loginInfo, isPersistent: false);
                return RedirectToAction("index", "home");
            }
        }    


        // LOCAL LOGIN/REGISTER

        [HttpGet]
        public ActionResult LogIn(string returnUrl)
        {
            setContextProperties();
            var model = new User
            {
                ReturnUrl = returnUrl
            };

            var providers = HttpContext.GetOwinContext()
                .Authentication.GetAuthenticationTypes(x => !string.IsNullOrEmpty(x.Caption))
                .ToList();
            //model.AuthProviders - providers;

            return PartialView(model);
        }

        [HttpPost]
        public async Task<ActionResult> LogIn(User model)
        {
            setContextProperties();
            if (!ModelState.IsValid)
            {
                return View();
            }
            var user = await userManager.FindAsync(model.Email, model.PasswordHash); 
            if (user != null)
            {
                await SignIn(user);
                return Redirect(Url.Action("index", "home")); // GetRedirectUrl(model.ReturnUrl));
            }

            // user authN failed
            ModelState.AddModelError("", "Invalid email or password");
            return PartialView();
        }

        [HttpGet]
        public ActionResult Register()
        {
            setContextProperties();
            return PartialView();
        }

        [HttpPost]
        public async Task<ActionResult> Register(User model)
        {
            setContextProperties();
            if (!ModelState.IsValid)
            {
                return View();
            }

            model.UserName = model.Email;
            var result = await userManager.CreateAsync(model, model.PasswordHash); // var result = await userManager.CreateAsync(model, model.PasswordHash);

            if (result.Succeeded)
            {
                await SignIn(model);
                return RedirectToAction("index", "home");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }

            return PartialView();
        }        
        
        public ActionResult LogOut()
        {
            var ctx = Request.GetOwinContext();
            var authManager = ctx.Authentication;

            authManager.SignOut("ApplicationCookie");
            return RedirectToAction("index", "home");
        }

        private async Task SignIn(User user)
        {
            var identity = await userManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
            GetAuthenticationManager().SignIn(identity);
        }



        // HELPER METHODS
         
        public void setContextProperties()
        {
            this.context = HttpContext.GetOwinContext().Get<RadioContext>();
            this.userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
        }

        private IAuthenticationManager GetAuthenticationManager()
        {
            var ctx = Request.GetOwinContext();
            return ctx.Authentication;
        }

        private string GetRedirectUrl(string returnUrl)
        {
            if (string.IsNullOrEmpty(returnUrl) || !Url.IsLocalUrl(returnUrl))
            {
                return Url.Action("index", "home");
            }

            return returnUrl;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && userManager != null)
            {
                userManager.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}