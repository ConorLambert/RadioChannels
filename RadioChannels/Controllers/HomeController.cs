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
using Microsoft.AspNet.Identity.Owin;
using static RadioChannels.App_Start.IdentityConfig;


namespace RadioChannels.Controllers
{
    //[RequireHttps]
    public class HomeController : Controller
    {
        //private RadioContext context;
        private WebApiAccess access = new WebApiAccess();
        // private readonly UserManager<User> userManager;        
        private RadioContext context;
        private ApplicationUserManager userManager;

        public HomeController()
        {            
            //userManager = createUserManager();
        }
        
        public HomeController(ApplicationUserManager userManager)
        {
            UserManager = userManager;
        }
        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        // configure the user manager
        private UserManager<User> createUserManager()
        {
            var usermanager = new UserManager<User>(new UserStore<User>(context)); //context = HttpContext.GetOwinContext().Get<RadioContext>(); // new RadioContext();               
            // allow alphanumeric characters in username
            usermanager.UserValidator = new UserValidator<User>(usermanager)
            {
                AllowOnlyAlphanumericUserNames = false
            };

            //context.SeedData(userManager);

            return usermanager;
        }

        
        // VIEWS

        public ActionResult Index()
        {
            setContextProperties();
            return View();
        }        

        [HttpGet]
        public ActionResult LogIn(string returnUrl)
        {
            setContextProperties();
            var model = new User
            {
                ReturnUrl = returnUrl
            };

            return View(model);
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
                return Redirect(Url.Action("index", "home")); //GetRedirectUrl(model.ReturnUrl));
            }

            // user authN failed
            ModelState.AddModelError("", "Invalid email or password");
            return View();
        }
      
    
        [HttpGet]
        public ActionResult Register()
        {
            setContextProperties();
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Register(User model)
        {
            setContextProperties();
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

        [HttpGet]
        public async Task<ActionResult> Favourites()
        {
            setContextProperties();
            ViewBag.Message = "Favourites";

            // get the user id
            var currentUserId = User.Identity.GetUserId(); // current User Id

            // get the list of favourites related to our current user based on their id
            List<Favourite> favourites = context.Favourite.Where(x => x.UserId == currentUserId).ToList();            
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