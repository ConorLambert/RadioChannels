using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using RadioChannels.Models;

namespace RadioChannels.Controllers
{
	public class WebApiAccess
	{
        HttpClient httpClient;
        string base_uri;

        public WebApiAccess()
        {
            httpClient = new HttpClient();
            if(!String.IsNullOrEmpty(Environment.GetEnvironmentVariable("WEBSITE_SITE_NAME")))
                base_uri = "http://tunage.azurewebsites.net/api/channels/"; // base_uri = "http://localhost:55555/api/channels/values?name="; 
            else
                base_uri = "http://localhost:55555/api/channels/";           
        }

        public async Task<Channel> GetChannelAsync(string id, string name)
        {
            Channel channel = null;            
            string uri = base_uri + "values?id=" + id + "&name=" + Uri.EscapeDataString(name);
            try {
                channel = JsonConvert.DeserializeObject<Channel>(await httpClient.GetStringAsync(uri));                
            } catch(Exception e) {
                Console.WriteLine(e.Message);
            }
            return channel;
        }

        public async Task<List<Channel>> GetChannelsAsync(string genre, int count)
        {
            List<Channel> channels = null;                        
            string uri = base_uri + genre + '/' + count.ToString(); 
            try
            {
                channels = JsonConvert.DeserializeObject<List<Channel>>(await httpClient.GetStringAsync(uri));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return channels;
        }

    }
}