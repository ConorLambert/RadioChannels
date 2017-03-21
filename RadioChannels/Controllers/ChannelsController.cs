using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Xml;
using RadioChannels.Models;

namespace RadioChannels.Controllers
{
    public class ChannelsController : ApiController
    {
        string shoutcast_api_key;
        HttpClient httpClient;        

        public ChannelsController()
        {
            shoutcast_api_key = "sJXVu3hXGmKjJiIx";
            httpClient = new HttpClient();
            System.Net.ServicePointManager.Expect100Continue = false;
            System.Net.ServicePointManager.UseNagleAlgorithm = false;
            WebRequest.DefaultWebProxy = null;
        }

        public static XmlDocument MakeRequest(string requestUrl)
        {
            try
            {
                HttpWebRequest request = WebRequest.Create(requestUrl) as HttpWebRequest;
                request.Method = "GET";
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;

                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(response.GetResponseStream());
                response.Dispose();
                return (xmlDoc);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);

                Console.Read();
                return null;
            }
        }

        public static List<Channel> ProcessResponse(XmlDocument response)
        {
            //Create namespace manager
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(response.NameTable);
            nsmgr.AddNamespace("ch", "urn:channels-schema");

            // XPath query which returns all <station .../> entries
            XmlNodeList stationElements = response.SelectNodes("//station");
            List<Channel> channels = new List<Channel>();

            foreach (XmlNode station in stationElements)    // for each station
            {
                XmlAttributeCollection details = station.Attributes;

                // Get Playlist information
                Channel channel = new Channel();
                if (details["id"] != null)
                {
                    channel.Id = details["id"].InnerText;
                    var playlistRequest = WebRequest.Create(@"http://yp.shoutcast.com/sbin/tunein-station.m3u?id=" + channel.Id);
                    using (var playlistResponse = playlistRequest.GetResponse())
                    using (var content = playlistResponse.GetResponseStream())
                    using (var reader = new StreamReader(content))
                    {
                        channel.Playlist = reader.ReadToEnd();
                    }                    
                }

                if (details["name"] != null)
                    channel.Name = details["name"].InnerText;
                if (details["mt"] != null)
                    channel.MediaType = details["mt"].InnerText;
                if (details["br"] != null)
                    channel.BitRate = details["br"].InnerText;
                if (details["ct"] != null)
                    channel.CurrentTrack = details["ct"].InnerText;
                if (details["logo"] != null)
                    channel.Logo = details["logo"].InnerText;
                if (details["lc"] != null)
                    channel.LC = details["lc"].InnerText;
                if (details["genre"] != null)
                    channel.Genre = details["genre"].InnerText;

                channels.Add(channel);
            }
            
            return channels;
        }        
        

        // GET api/<controller>/20
        // get all channels that have the associated genre and apply pagination
        [HttpGet]
        public List<Channel> GetChannels(string genre, int index)
        {
            var limit = 20; // pagination limit            
            string url = string.Format("http://api.shoutcast.com/legacy/genresearch?k={0}&genre={1}&limit={2},{3}", shoutcast_api_key, genre, index, limit);
            XmlDocument response = MakeRequest(url);
            List<Channel> channels = ProcessResponse(response);          
            return channels;
        }                  
             
        public Channel GetChannel(string id, string name)
        {       
            string url = string.Format("http://api.shoutcast.com/legacy/stationsearch?k={0}&search={1}", shoutcast_api_key, name);
            XmlDocument response = MakeRequest(url);
            List<Channel> channels = ProcessResponse(response);
            foreach(var channel in channels)
            {
                if(channel.Id == id)
                    return channel;
            }
            return null; 
        }        
    }
}