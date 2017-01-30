using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using RadioChannels.DAL;
using RadioChannels.Models;

namespace RadioChannels.App_Start
{
    public class IdentityConfig
    {
        public class ApplicationUserManager : UserManager<User>
        {
            public ApplicationUserManager(IUserStore<User> store) : base(store)
            {
            }

            public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
            {
                var manager = new ApplicationUserManager(new UserStore<User>(context.Get<RadioContext>()));
                return manager;
            }
        }
    }
}