using RadioChannels.DAL;
using RadioChannels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace RadioChannels.Controllers
{
    public class HomeController : Controller
    {
        private RadioContext db = new RadioContext();
        private WebApiAccess access = new WebApiAccess();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult SignIn()
        {
            ViewBag.Message = "Sign In";
            return View("SignIn");
        }

        public ActionResult Register()
        {
            ViewBag.Message = "Registration";
            return View("Register");
        }
        
        public async Task<ActionResult> Favourites()
        {
            ViewBag.Message = "Favourites";

            // get the user id
            var id = 1; // TEST DATA

            // get the list of favourites related to our current user based on their id
            List<Favourite> favourites = db.Favourite.Where(x => x.UserId == id).ToList();

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
    }
}