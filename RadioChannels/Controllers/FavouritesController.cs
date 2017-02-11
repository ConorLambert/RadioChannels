using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using RadioChannels.DAL;
using RadioChannels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using static RadioChannels.App_Start.IdentityConfig;
using Microsoft.AspNet.Identity;
using System.Net.Http;
using System.Net;

namespace RadioChannels.Controllers
{
    public class FavouritesController : Controller
    {        
        private WebApiAccess access = new WebApiAccess();        
        private RadioContext context;
        private ApplicationUserManager userManager;

        // GET FAVOURTIES
        [HttpGet]
        public async Task<ActionResult> Index()
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

            return PartialView("Favourites", channels);            
        }


        // ADD FAVOURITES
        [HttpPost]
        public ActionResult AddFavourite(string id)
        {
            setContextProperties();
            // get the current user logged in
            string user = User.Identity.GetUserId();

            if (user == null)
            {                
                return Json(new { success = false, responseText = "You must be logged in to use this feature" }, JsonRequestBehavior.AllowGet);
            }

            // add channel to Favourties database based on user id
            context.Favourite.Add(new Favourite { UserId = user, ChannelName = id });
            context.SaveChanges();
            
            //  Send "Success"
            return Json(new { success = true, responseText = "Successfully added " + id + " to Favourites" }, JsonRequestBehavior.AllowGet);            
        }


        public void setContextProperties()
        {
            this.context = HttpContext.GetOwinContext().Get<RadioContext>();
            this.userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
        }
    }
}