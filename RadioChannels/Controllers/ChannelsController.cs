using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Xml;
using RadioChannels.Models;
using RadioChannels.DAL;
using Newtonsoft.Json;
using System.Web.Services.Description;
using System.Text;

namespace RadioChannels.Controllers
{
    public class ChannelsController : ApiController
    {
        string api_key;
        string urlTemplate;
        string url;
        HttpClient httpClient;        

        public ChannelsController()
        {
            api_key = "sJXVu3hXGmKjJiIx";
            httpClient = new HttpClient();
        }

        public static XmlDocument MakeRequest(string requestUrl)
        {
            try
            {
                HttpWebRequest request = WebRequest.Create(requestUrl) as HttpWebRequest;
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;

                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(response.GetResponseStream());
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

            XmlNodeList stationElements = response.SelectNodes("//station");
            List<Channel> channels = new List<Channel>();

            foreach (XmlNode station in stationElements)    // for each station
            {
                XmlAttributeCollection details = station.Attributes;
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

        // GET api/<controller>
        [HttpGet]
        public IEnumerable<Channel> GetAllChannels()
        {
            System.Diagnostics.Debug.WriteLine("In Get All Channels");
            urlTemplate = "http://api.shoutcast.com/legacy/Top500?k={0}&br=64";
            url = string.Format(urlTemplate, api_key);
            //Create the REST Services 'Find Location by Query' request            
            XmlDocument response = MakeRequest(url);
            List<Channel> channels = ProcessResponse(response);

            // MAKE REQUEST TO SHOUTCAST
            return channels;
        }

        // GET api/<controller>/20
        // get all channels that have the associated genre and apply pagination
        [HttpGet]
        public List<Channel> GetChannels(string some_var, int index)
        {
            var limit = 20; // pagination limit
            urlTemplate = "http://api.shoutcast.com/legacy/genresearch?k={0}&genre={1}&limit={2},{3}";
            url = string.Format(urlTemplate, api_key, some_var, index, limit);
            XmlDocument response = MakeRequest(url);
            List<Channel> channels = ProcessResponse(response);
            return channels;
        }                  
             
        public Channel GetChannel(string name)
        {            
            urlTemplate = "http://api.shoutcast.com/legacy/stationsearch?k={0}&search={1}";
            url = string.Format(urlTemplate, api_key, name);
            XmlDocument response = MakeRequest(url);
            List<Channel> channels = ProcessResponse(response);
            return channels.ElementAt(0); // JsonConvert.SerializeObject(channels.ElementAt(0));
        }
    }
}