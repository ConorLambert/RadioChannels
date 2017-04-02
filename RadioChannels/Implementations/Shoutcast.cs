using RadioChannels.Interfaces.WebApi;
using System;
using System.Collections.Generic;
using RadioChannels.Models;
using System.Xml;
using System.Net;
using System.IO;
using System.Net.Http;

namespace RadioChannels.Implementations
{
    public class Shoutcast : IDirectories
    {
        string base_uri = "http://api.shoutcast.com/legacy/";
        string shoutcast_api_key; 
        int limit;
        IRequest request;

        public Shoutcast()
        {
            request = new XmlRequest();
            shoutcast_api_key = "sJXVu3hXGmKjJiIx";
            limit = 20;
        }

        // get channels of type genre starting at index
        public List<Channel> GetChannels(string genre, int index)
        {          
            string url = string.Format(base_uri + "genresearch?k={0}&genre={1}&limit={2},{3}", shoutcast_api_key, genre, index, limit);
            return request.ApiRequestChannels(url);
        }

        // get me channels with name
        public List<Channel> GetChannels(string name)
        {
            string url = string.Format("http://api.shoutcast.com/legacy/stationsearch?k={0}&search={1}", shoutcast_api_key, name);
            return request.ApiRequestChannels(url);            
        }
    }
}