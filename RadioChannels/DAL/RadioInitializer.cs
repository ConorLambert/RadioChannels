using RadioChannels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RadioChannels.DAL
{
    public class RadioInitializer : System.Data.Entity.DropCreateDatabaseIfModelChanges<RadioContext>
    {
        protected override void Seed(RadioContext context)
        {
            /*
            if (userManager.FindByEmail("john_doe@hotmail.com") == null)
            {
                var user = new User()
                {
                    UserName = "JohnDoe",
                    Email = "john_doe@hotmail.com"
                };

                userManager.Create(user, "P@ssw0rd!");
            }

            if (userManager.FindByEmail("some_chap@gmail.com") == null)
            {
                var user = new User()
                {
                    UserName = "SomeChap",
                    Email = "some_chap@gmail.com"
                };

                userManager.Create(user, "Password@123");
            }

            if (userManager.FindByEmail("another_guy@hotmail.com") == null)
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
            */

            context.Channel.Add(new Channel { Id = "908641", Name = "Deep House Network - Streaming Deep House & Soulful", MediaType = "audio/mpeg", BitRate = "192", CurrentTrack = "German Brigante - So Good D Nox and Beckers Remix", Logo = "", LC = "258", Genre = "House", Playlist = "/sbin/tunein-station.m3u" });
            context.Channel.Add(new Channel { Id = "128609", Name = "54house.fm - The Heartbeat Of House Music", MediaType = "audio/mpeg", BitRate = "192", CurrentTrack = "Kaiserdisco in the Mix on 54house.fm - KD Music RadioShow", Logo = "", LC = "193", Genre = "House", Playlist = "/sbin/tunein-station.m3u" });
            context.Channel.Add(new Channel { Id = "230521", Name = "Dogglounge Deep House Radio", MediaType = "audio/mpeg", BitRate = "128", CurrentTrack = "Shur-I-Kan - The Light (Original Mix)", Logo = "", LC = "117", Genre = "House", Playlist = "/sbin/tunein-station.m3u" });
            context.Channel.Add(new Channel { Id = "1292811", Name = "electroradio.fm", MediaType = "audio/mpeg", BitRate = "192", CurrentTrack = "Max Graham live in the Mix - Cycles Radio", Logo = "", LC = "105", Genre = "House", Playlist = "/sbin/tunein-station.m3u" });
            context.SaveChanges();
            /*
            context.Favourite.Add(new Favourite { UserId = john_doe_id, ChannelName = "Dogglounge Deep House Radio" });
            context.Favourite.Add(new Favourite { UserId = john_doe_id, ChannelName = "54house.fm - The Heartbeat Of House Music" });
            context.Favourite.Add(new Favourite { UserId = john_doe_id, ChannelName = "Deep House Network - Streaming Deep House & Soulful" });
            context.SaveChanges();
            */
        }
    }
}