using Microsoft.AspNet.Identity;
using RadioChannels.DAL;
using RadioChannels.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.Owin.Security;
using System.Web;
using Microsoft.AspNet.Identity.EntityFramework;

namespace RadioChannels.Controllers
{
    //[RequireHttps]
    public class HomeController : Controller
    {
        //private RadioContext context;
        private WebApiAccess access = new WebApiAccess();
        private readonly UserManager<User> userManager;
        private RadioContext context;

        public HomeController()
        {
            userManager = createUserManager();
        }

        // configure the user manager
        private UserManager<User> createUserManager()
        {
            context = new RadioContext();   
            var usermanager = new UserManager<User>(new UserStore<User>(context));
            // allow alphanumeric characters in username
            usermanager.UserValidator = new UserValidator<User>(usermanager)
            {
                AllowOnlyAlphanumericUserNames = false
            };

            return usermanager;
        }

        
        // VIEWS

        public ActionResult Index()
        {
            return View();
        }        

        [HttpGet]
        public ActionResult LogIn(string returnUrl)
        {
            var model = new User
            {
                ReturnUrl = returnUrl
            };

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> LogIn(User model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var user = await userManager.FindAsync(model.Email, model.PasswordHash);

            if (user != null)
            {
                await SignIn(user);
                return Redirect(Url.Action("index", "home")); //GetRedirectUrl(model.ReturnUrl));
            }

            // user authN failed
            ModelState.AddModelError("", "Invalid email or password");
            return View();
        }
      
    
        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Register(User model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var result = await userManager.CreateAsync(model, model.PasswordHash);

            if (result.Succeeded)
            {
                await SignIn(model);
                return RedirectToAction("index", "home");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }

            return View();
        }

        public async Task<ActionResult> Favourites()
        {
            ViewBag.Message = "Favourites";

            // get the user id
            var user = userManager.FindById(""); // current User Id


            // get the list of favourites related to our current user based on their id
            List<Favourite> favourites = context.Favourite.Where(x => x.UserId == user.Id).ToList();            
            if (favourites == null)
            {
                return HttpNotFound();
            }

            List<Channel> channels = new List<Channel>();
            foreach (var item in favourites)
            {
                channels.Add(await access.GetChannelAsync(item.ChannelName));
            }

            return View("Favourites", channels);
        }



        // HELPER METHODS

        private IAuthenticationManager GetAuthenticationManager()
        {
            var ctx = Request.GetOwinContext();
            return ctx.Authentication;
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