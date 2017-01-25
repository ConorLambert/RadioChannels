using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using RadioChannels.Models;

namespace RadioChannels.DAL
{
    public class RadioInitializer : System.Data.Entity.DropCreateDatabaseIfModelChanges<RadioContext>
    {            
        protected override void Seed(RadioContext context)
        {
            var channels = new List<Channel>
            {
                new Channel{ Id = "908641", Name = "Deep House Network - Streaming Deep House & Soulful", MediaType = "audio/mpeg", BitRate = "192", CurrentTrack = "German Brigante - So Good D Nox and Beckers Remix", Logo = "", LC = "258", Genre = "House", Playlist = "/sbin/tunein-station.m3u"},
                new Channel{ Id = "128609", Name = "54house.fm - The Heartbeat Of House Music", MediaType = "audio/mpeg", BitRate = "192", CurrentTrack = "Kaiserdisco in the Mix on 54house.fm - KD Music RadioShow", Logo = "", LC = "193", Genre = "House", Playlist = "/sbin/tunein-station.m3u"},
                new Channel{ Id = "230521", Name = "Dogglounge Deep House Radio", MediaType = "audio/mpeg", BitRate = "128", CurrentTrack = "Shur-I-Kan - The Light (Original Mix)", Logo = "", LC = "117", Genre = "House", Playlist = "/sbin/tunein-station.m3u" },
                new Channel{ Id = "1292811", Name = "electroradio.fm", MediaType = "audio/mpeg", BitRate = "192", CurrentTrack = "Max Graham live in the Mix - Cycles Radio", Logo = "", LC = "105", Genre = "House", Playlist = "/sbin/tunein-station.m3u" }                
            };
            channels.ForEach(c => context.Channel.Add(c));
            context.SaveChanges();

            var users = new List<User>
            {
                new User{ ID = 1, Username = "JohnDoe", Email = "john_doe@hotmail.com"},
                new User{ ID = 1, Username = "SomeChap", Email = "some_chap@hotmail.com"},
                new User{ ID = 1, Username = "FredMerc", Email = "fred_merc@hotmail.com"},
                new User{ ID = 1, Username = "Paddy", Email = "john_doe@hotmail.com"}
            };
            users.ForEach(u => context.User.Add(u));
            context.SaveChanges();

            var favourites = new List<Favourite>
            {
                new Favourite{UserId = 1, ChannelName = "Dogglounge Deep House Radio"},
                new Favourite{UserId = 1, ChannelName = "54house.fm - The Heartbeat Of House Music"},
                new Favourite{UserId = 1, ChannelName = "Deep House Network - Streaming Deep House & Soulful"}
            };
            favourites.ForEach(f => context.Favourite.Add(f));
            context.SaveChanges();
        }
    }
}