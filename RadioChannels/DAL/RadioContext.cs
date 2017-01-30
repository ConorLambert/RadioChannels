using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using RadioChannels.Models;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace RadioChannels.DAL
{
    public class RadioContext : IdentityDbContext<User>
    {
        public RadioContext() : base("RadioContext")
        {
            //Database.SetInitializer(new MigrateDatabaseToLatestVersion<RadioContext, RadioChannels.Migrations.Configuration>());
            
            //Database.SetInitializer<RadioContext>(new MigrateDatabaseToLatestVersion<RadioContext>());
            //Database.SetInitializer(new MigrateDatabaseToLatestVersion<RadioContext>, Configuration>());
        }
        
        public DbSet<Favourite> Favourite { get; set; }
        public DbSet<Channel> Channel { get; set; } // MAY NOT EVEN NEED THIS

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            base.OnModelCreating(modelBuilder);            
        }

        public static RadioContext Create()
        {
            return new RadioContext();
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

        */
        /*
        public void SeedData(UserManager<User> userManager)
        {      
            if(userManager.FindByEmail("john_doe@hotmail.com") == null)
            {
                var user = new User()
                {
                    UserName = "JohnDoe",
                    Email = "john_doe@hotmail.com"
                };

                userManager.Create(user, "P@ssw0rd!");
            }

            if(userManager.FindByEmail("some_chap@gmail.com") == null)
            {
                var user = new User()
                {
                    UserName = "SomeChap",
                    Email = "some_chap@gmail.com"
                };

                userManager.Create(user, "Password@123");
            }

            if(userManager.FindByEmail("another_guy@hotmail.com") == null)
            {
                var user = new User()
                {
                    UserName = "AnotherGuy",
                    Email = "another_guy@hotmail.com"
                };

                userManager.Create(user, "Password@321");
            }

            var john_doe_id = userManager.FindByEmail("john_doe@hotmail.com").Id;
            var some_chap_id = userManager.FindByEmail("some_chap@gmail.com").Id;
            var another_guy_id = userManager.FindByEmail("another_guy@hotmail.com").Id;

            this.Channel.Add(new Channel { Id = "908641", Name = "Deep House Network - Streaming Deep House & Soulful", MediaType = "audio/mpeg", BitRate = "192", CurrentTrack = "German Brigante - So Good D Nox and Beckers Remix", Logo = "", LC = "258", Genre = "House", Playlist = "/sbin/tunein-station.m3u" });
            this.Channel.Add(new Channel { Id = "128609", Name = "54house.fm - The Heartbeat Of House Music", MediaType = "audio/mpeg", BitRate = "192", CurrentTrack = "Kaiserdisco in the Mix on 54house.fm - KD Music RadioShow", Logo = "", LC = "193", Genre = "House", Playlist = "/sbin/tunein-station.m3u" });
            this.Channel.Add(new Channel { Id = "230521", Name = "Dogglounge Deep House Radio", MediaType = "audio/mpeg", BitRate = "128", CurrentTrack = "Shur-I-Kan - The Light (Original Mix)", Logo = "", LC = "117", Genre = "House", Playlist = "/sbin/tunein-station.m3u" });
            this.Channel.Add(new Channel { Id = "1292811", Name = "electroradio.fm", MediaType = "audio/mpeg", BitRate = "192", CurrentTrack = "Max Graham live in the Mix - Cycles Radio", Logo = "", LC = "105", Genre = "House", Playlist = "/sbin/tunein-station.m3u" });
            this.SaveChanges();
            this.Favourite.Add(new Favourite { UserId = john_doe_id, ChannelName = "Dogglounge Deep House Radio" });
            this.Favourite.Add(new Favourite { UserId = john_doe_id, ChannelName = "54house.fm - The Heartbeat Of House Music" });
            this.Favourite.Add(new Favourite { UserId = john_doe_id, ChannelName = "Deep House Network - Streaming Deep House & Soulful" });
            this.SaveChanges();                 
        }  
        */
    }
}