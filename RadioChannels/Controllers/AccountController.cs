using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using RadioChannels.DAL;
using RadioChannels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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

        // GET: Account
        public ActionResult Index()
        {
            return PartialView();
        }

        [HttpGet]
        public ActionResult LogIn(string returnUrl)
        {
            setContextProperties();
            var model = new User
            {
                ReturnUrl = returnUrl
            };

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

            var user = await userManager.FindAsync(model.Email, model.PasswordHash); // var user = await userManager.FindAsync(model.Email, model.PasswordHash);

            if (user != null)
            {
                await SignIn(user);
                return Redirect(Url.Action("index", "home")); //GetRedirectUrl(model.ReturnUrl));
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