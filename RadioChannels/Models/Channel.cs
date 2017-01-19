using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RadioChannels.Models
{
    public class Channel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string MediaType { get; set; }
        public string BitRate { get; set; }
        public string CurrentTrack { get; set; }
        public string Logo { get; set; }
        public string LC { get; set; }
        public string Genre { get; set; }      
       
    }
}