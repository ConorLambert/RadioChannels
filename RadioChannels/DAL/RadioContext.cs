using Microsoft.AspNet.Identity.EntityFramework;
//using RadioChannels.Migrations;
using RadioChannels.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace RadioChannels.DAL
{
    public class RadioContext : IdentityDbContext<User>
    {
        public RadioContext() : base("RadioContext")
        {
            Database.SetInitializer<RadioContext>(new DropCreateDatabaseAlways<RadioContext>());
            //Database.SetInitializer(new MigrateDatabaseToLatestVersion<RadioContext>, Configuration>());
        }
        
        public DbSet<Favourite> Favourite { get; set; }
        // public DbSet<Channel> Channel { get; set; } // MAY NOT EVEN NEED THIS

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            base.OnModelCreating(modelBuilder);            
        }

        /*
        public DbSet<User> User { get; set; }

        

        public class DropCreateIfChangeInitializer : DropCreateDatabaseIfModelChanges<RadioContext>
        {
            protected override void Seed(RadioContext context)
            {
                context.Seed(context);

                base.Seed(context);
            }
        }      

        protected void Seed(RadioContext context)
        {
        */

        /*
        if(_userManager.FindByEmail("john_doe@hotmail.com") == null)
        {
            var user = new User()
            {
                UserName = "JohnDoe",
                Email = "john_doe@hotmail.com"
            };

            _userManager.Create(user, "P@ssw0rd!");
        }
        */

        /*
        context.Channel.Add(new Channel { Id = "908641", Name = "Deep House Network - Streaming Deep House & Soulful", MediaType = "audio/mpeg", BitRate = "192", CurrentTrack = "German Brigante - So Good D Nox and Beckers Remix", Logo = "", LC = "258", Genre = "House", Playlist = "/sbin/tunein-station.m3u" });
        context.Channel.Add(new Channel { Id = "128609", Name = "54house.fm - The Heartbeat Of House Music", MediaType = "audio/mpeg", BitRate = "192", CurrentTrack = "Kaiserdisco in the Mix on 54house.fm - KD Music RadioShow", Logo = "", LC = "193", Genre = "House", Playlist = "/sbin/tunein-station.m3u" });
        context.Channel.Add(new Channel { Id = "230521", Name = "Dogglounge Deep House Radio", MediaType = "audio/mpeg", BitRate = "128", CurrentTrack = "Shur-I-Kan - The Light (Original Mix)", Logo = "", LC = "117", Genre = "House", Playlist = "/sbin/tunein-station.m3u" });
        context.Channel.Add(new Channel { Id = "1292811", Name = "electroradio.fm", MediaType = "audio/mpeg", BitRate = "192", CurrentTrack = "Max Graham live in the Mix - Cycles Radio", Logo = "", LC = "105", Genre = "House", Playlist = "/sbin/tunein-station.m3u" });
        context.SaveChanges();

        context.Favourite.Add(new Favourite { UserId = 1, ChannelName = "Dogglounge Deep House Radio" });
        context.Favourite.Add(new Favourite { UserId = 1, ChannelName = "54house.fm - The Heartbeat Of House Music" });
        context.Favourite.Add(new Favourite { UserId = 1, ChannelName = "Deep House Network - Streaming Deep House & Soulful" });
        context.SaveChanges();      

        if (!(context.Users.Any(u => u.UserName == "dj@dj.com")))
            {
                var userStore = new UserStore<User>(context);
                var userManager = new UserManager<User>(userStore);
                var userToInsert = new User { UserName = "dj@dj.com", PhoneNumber = "0797697898" };
                userManager.Create(userToInsert, "Password@123");
            }

    }
    */
    }
}