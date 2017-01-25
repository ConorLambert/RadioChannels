using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RadioChannels.Models
{
    public class Channel
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("mediatype")]
        public string MediaType { get; set; }
        [JsonProperty("bitrate")]
        public string BitRate { get; set; }
        [JsonProperty("currenttrack")]
        public string CurrentTrack { get; set; }
        [JsonProperty("logo")]
        public string Logo { get; set; }
        [JsonProperty("lc")]
        public string LC { get; set; }
        [JsonProperty("genre")]
        public string Genre { get; set; }
        [JsonProperty("playlist")]
        public string Playlist { get; set; }       
    }
}