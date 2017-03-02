namespace RadioChannels.Migrations
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using RadioChannels.Models;
    using System.Data.Entity.Migrations;

    internal sealed class Configuration : DbMigrationsConfiguration<RadioChannels.DAL.RadioContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;

            /*
            var radioContext = HttpContext.Current.GetOwinContext().Get<RadioContext>();
            Seed(radioContext);
            */
        }

        protected override void Seed(RadioChannels.DAL.RadioContext context)
        {
            var userStore = new UserStore<User>(context);
            var userManager = new UserManager<User>(userStore);

            if (userManager.FindByEmail("john_doe@hotmail.com") == null)
            {
                var user = new User()
                {
                    FirstName = "John",
                    LastName = "Doe",
                    UserName = "john_doe@hotmail.com",
                    Email = "john_doe@hotmail.com"
                };
                userManager.Create(user, "P@ssw0rd!");
            }

            if (userManager.FindByEmail("some_chap@gmail.com") == null)
            {
                var user = new User()
                {
                    FirstName = "Some",
                    LastName = "Chap",
                    UserName = "some_chap@gmail.com",
                    Email = "some_chap@gmail.com"
                };
                userManager.Create(user, "Password@123");
            }

            if (userManager.FindByEmail("another_guy@hotmail.com") == null)
            {
                var user = new User()
                {
                    FirstName = "Another",
                    LastName = "Guy",
                    UserName = "another_guy@hotmail.com",
                    Email = "another_guy@hotmail.com"
                };
                userManager.Create(user, "Password@321");
            }

            var john_doe_id = userManager.FindByEmail("john_doe@hotmail.com").Id;
            var some_chap_id = userManager.FindByEmail("some_chap@gmail.com").Id;
            var another_guy_id = userManager.FindByEmail("another_guy@hotmail.com").Id;

            context.Channel.AddOrUpdate(new Channel { Id = "908641", Name = "Deep House Network - Streaming Deep House & Soulful", MediaType = "audio/mpeg", BitRate = "192", CurrentTrack = "German Brigante - So Good D Nox and Beckers Remix", Logo = "", LC = "258", Genre = "House", Playlist = "/sbin/tunein-station.m3u" });
            context.Channel.AddOrUpdate(new Channel { Id = "128609", Name = "54house.fm - The Heartbeat Of House Music", MediaType = "audio/mpeg", BitRate = "192", CurrentTrack = "Kaiserdisco in the Mix on 54house.fm - KD Music RadioShow", Logo = "", LC = "193", Genre = "House", Playlist = "/sbin/tunein-station.m3u" });
            context.Channel.AddOrUpdate(new Channel { Id = "230521", Name = "Dogglounge Deep House Radio", MediaType = "audio/mpeg", BitRate = "128", CurrentTrack = "Shur-I-Kan - The Light (Original Mix)", Logo = "", LC = "117", Genre = "House", Playlist = "/sbin/tunein-station.m3u" });
            context.Channel.AddOrUpdate(new Channel { Id = "1292811", Name = "electroradio.fm", MediaType = "audio/mpeg", BitRate = "192", CurrentTrack = "Max Graham live in the Mix - Cycles Radio", Logo = "", LC = "105", Genre = "House", Playlist = "/sbin/tunein-station.m3u" });
            context.SaveChanges();

            context.Favourite.AddOrUpdate(new Favourite { UserId = john_doe_id, ChannelName = "Dogglounge Deep House Radio", ChannelId = "230521" });
            context.Favourite.AddOrUpdate(new Favourite { UserId = john_doe_id, ChannelName = "54house.fm - The Heartbeat Of House Music", ChannelId = "128609" });
            context.Favourite.AddOrUpdate(new Favourite { UserId = john_doe_id, ChannelName = "Deep House Network - Streaming Deep House & Soulful", ChannelId = "908641" });
            context.SaveChanges();
        }
    }
}
