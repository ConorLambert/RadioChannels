﻿using RadioChannels.DAL;
using RadioChannels.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web;
using Microsoft.AspNet.Identity.Owin;
using static RadioChannels.App_Start.IdentityConfig;


namespace RadioChannels.Controllers
{
    //[RequireHttps]
    public class HomeController : Controller
    {
        private WebApiAccess access = new WebApiAccess();
        private RadioContext context;
        private ApplicationUserManager userManager;

        public HomeController()
        {            
        }      
        
        // VIEWS

        public ActionResult Index()
        {
            // if the current user is logged in, then redirect them to their favourites
            if ((System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated)
                return Redirect(Url.Action("all", "favourites"));
            else // return index page
                return View();
        }

        // AJAX request for index page
        public ActionResult IndexPartial()
        {
            return PartialView("Index");
        }

        public async Task<ActionResult> IndexAsync(string id, int index)
        {            
            List<Channel> channels = await access.GetChannelsAsync(id, index);
            return PartialView("_Stations", channels);
        }


        public void setContextProperties()
        {
            this.context = HttpContext.GetOwinContext().Get<RadioContext>();
            this.userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
        }
    }
}