using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
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
            base_uri = "http://localhost:55555/api/channels/values?name=";
        }

        public async Task<Channel> GetChannelAsync(string name)
        {
            Channel channel = null;
            //Uri uri = new Uri(base_uri + name);
            string uri = base_uri + Uri.EscapeDataString(name); // HttpUtility.UrlEncode(name);
            try {
                channel = JsonConvert.DeserializeObject<Channel>(await httpClient.GetStringAsync(uri));                
            } catch(Exception e) {
                Console.WriteLine(e.Message);
            }
            return channel;
        }

    }
}