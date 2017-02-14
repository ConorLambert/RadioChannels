using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using RadioChannels.DAL;
using RadioChannels.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using static RadioChannels.App_Start.IdentityConfig;

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
            return await GetChannelsAsync("partial");
        }

        [HttpGet]
        public async Task<ActionResult> All()
        {
            return await GetChannelsAsync("full");
        }

        [HttpGet]
        public ActionResult IndexPartial()
        {
            return PartialView("FavouritesPartial");
        }

        public async Task<ActionResult> GetChannelsAsync(string portion)
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
                channels.Add(await access.GetChannelAsync(item.ChannelId, item.ChannelName));
            }

            if (portion == "partial")
                return PartialView("Favourites", channels);
            else
                return View("Favourites", channels);
        }


        // ADD FAVOURITES
        [HttpPost]
        public ActionResult AddFavourite(string id, string name)
        {
            setContextProperties();
            // get the current user logged in
            string user = User.Identity.GetUserId();

            if (user == null)
            {                
                return Json(new { success = false, responseText = "You must be logged in to use this feature" }, JsonRequestBehavior.AllowGet);
            }

            // add channel to Favourties database based on user id
            context.Favourite.Add(new Favourite { UserId = user, ChannelName = name, ChannelId = id });
            context.SaveChanges();
            
            //  Send "Success"
            return Json(new { success = true, responseText = "Successfully added " + name + " to Favourites" }, JsonRequestBehavior.AllowGet);            
        }


        [HttpPut]
        public ActionResult RemoveFavourite(string id, string name)
        {
            setContextProperties();

            // get the current user logged in
            string user = User.Identity.GetUserId();

            if (user == null)
            {
                return Json(new { success = false, responseText = "You must be logged in to use this feature" }, JsonRequestBehavior.AllowGet);
            }

            var myCurrent = context.Favourite.Where(u => u.ChannelName == name && u.UserId == user && u.ChannelId == id).FirstOrDefault(); // .Select(u => u.Id);
            //var myCurrent = new Favourite { Id = prim., UserId = user, ChannelName = id };

            var entry = context.Entry(myCurrent);
            if (entry.State == EntityState.Detached)
                context.Favourite.Attach(myCurrent);
            context.Favourite.Remove(myCurrent);

            try
            {
                context.SaveChanges();
            }
            catch (OptimisticConcurrencyException)
            {
                (((IObjectContextAdapter)context).ObjectContext).Refresh(RefreshMode.ClientWins, context.Favourite);
                context.SaveChanges();
            }

            // add channel to Favourties database based on user id
            //context.Favourite.Remove(new Favourite { UserId = user, ChannelName = id });
            //context.SaveChanges();

            //  Send "Success"
            return Json(new { success = true, responseText = "Successfully removed " + name + " from Favourites" }, JsonRequestBehavior.AllowGet);
        }


        public void setContextProperties()
        {
            this.context = HttpContext.GetOwinContext().Get<RadioContext>();
            this.userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
        }
    }
}