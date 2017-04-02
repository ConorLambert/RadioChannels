using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Web.Http;
using RadioChannels.Models;
using RadioChannels.Interfaces.WebApi;
using RadioChannels.Implementations;

namespace RadioChannels.Controllers
{
    public class ChannelsController : ApiController
    {        
        IDirectories directories;   

        public ChannelsController()
        {
            directories = new Shoutcast();
        }         

        [HttpGet]
        public List<Channel> GetChannels(string genre, int index)
        {            
            return directories.GetChannels(genre, index);            
        }

        public Channel GetChannel(string id, string name)
        {             
            List<Channel> channels = directories.GetChannels(name);
            foreach (var channel in channels)
            {
                if(channel.Id == id)
                    return channel;
            }
            return null; 
        }        
    }
}